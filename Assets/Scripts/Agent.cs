using System.Collections;
using System.Collections.Generic;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics;
using UnityEngine;




public class Agent : MonoBehaviour
{

    
    public State state;

    public int ID { get; set; }
    public int HistorySize { get; set; } = 20;
    private LinkedList<Document> _history = new LinkedList<Document>();

    #region deprecated (replaced by multi-dim learning system)



    /**
     * A policy, as defined in the reinforcement learning literature, is a function pi( a  |  s ) that specifies the probability of taking action a if in state s.
     * 
     * The policy for each agent is as follows:
     *      1. sample from its rewward distribution as specified by alpha and beta (the distribution is a Beta pdf)
     *      2. use this sample and compare its value with all other document values.
     *      3. Choose the document that is closest to the sample. (Choosing a document IS the action.)
     *   return that chosen document
     */

    /*
    public Document chooseByPolicy()
    {
        
        var betaDist = new Beta(state.getAlpha(), state.getBeta());

        // generate a sample from the beta distribution specifying the user's beliefs
        float sample = (float)betaDist.Sample();

        Debug.Log("SAMPLE CHOSEN: " + sample);

        // Choose the document available to them closest to the sample
        Document chosen = state.getDocuments()[0];

        // the distance between the sample and the closest matching document
        float mindist = 2f;

        for (int i = 0; i < state.getDocuments().Count; i++)
        {
            // euclidean distance between the sample and the document
            float dist = Mathf.Abs(state.getDocuments()[i].value - sample);
            if (dist < mindist)
            {
                // update the minimum distance
                mindist = dist;
                // update the reference to closest document found so far
                chosen = state.getDocuments()[i];
            }
        }

        // return the closest match
        return chosen;
    }

    */
    /**
     * 
     * Upon consumption of a document, alpha and beta must update according to some rule.
     * 
     * Paradigm: Facilitated polared learning. Learning happens the most given extreme documents.
     * 
     * This is the so called ``learning'' undergone by each agent upon consuming a document
     * 
     */
    /*
    public void learn(Document consumed)
    {
        float nextAlpha = sampleGaussian(meanLearningAlpha(consumed.value), 0.2f) + this.state.getAlpha();
        if (nextAlpha <= 0)
            state.setAlpha(0.001f);
        else state.setAlpha(nextAlpha);


        float nextBeta = sampleGaussian(meanLearningBeta(consumed.value), 0.2f) + this.state.getBeta();
        if (nextBeta <= 0)
            state.setBeta(0.001f);
        else state.setBeta(nextBeta);
    }
    */

    #endregion

    public float meanLearningBeta(float documentRead)
    {
        return -0.215f * (documentRead - 0.5f);
    }

    private float meanLearningAlpha(float documentRead)
    {
        // alpha decrease for documents close to zero
        return 0.215f * (documentRead - 0.5f);
    }




    /**
     * 
     * Pull a single float from the specified Gaussian Distribution which uses Box-Muller transform.
     * 
     */
    private float sampleGaussian(float mean, float sd)
    {
        // Two uniform random variables
        float u1 = Random.Range(0.00001f, 0.99999f);
        float u2 = Random.Range(0.00001f, 0.99999f);

        // Box-Muller transform
        float z = Mathf.Sqrt(-2f * Mathf.Log(u1)) * Mathf.Cos(2 * Mathf.PI * u2);

        // Apply paramter scale
        return z * sd + mean;
    }




    #region helper functions for maintaining history properly

    public void AddToHistory(Document d)
    {
        _history.AddFirst(d);

        if (_history.Count == HistorySize + 1)
        {
            _history.RemoveLast();
        }

    }

    #endregion



    #region categorical learning model methods

    /// <summary>
    /// Handles learning for all of the categories, according to category alpha, category beta and 
    /// the value of the document being read.
    /// </summary>
    /// <param name="consumed"></param>
    public void BatchLearn(Document consumed)
    {
        for (int i = 0; i <Settings.Categories;++i)
        {
            // handle learning for category i

            // first, get the alpha and beta values for this specific category
            ABPair catAB = state.categoriesAB[i];

            // get the document 0-1 value for this value
            float DocumentCategoryValue = consumed.values[i];

            float nextA = sampleGaussian(meanLearningAlpha(DocumentCategoryValue), 0.2f) + catAB.alpha;
            
            if (nextA <=0)
            {
                catAB.alpha = 0.001f;
            } else
            {
                catAB.alpha = nextA;
            }

            float nextB = sampleGaussian(meanLearningBeta(DocumentCategoryValue), 0.2f) + catAB.beta;
            if (nextB <= 0)
            {
                catAB.beta = 0.001f;
            } else
            {
                catAB.beta = nextB;
            }

        }
    }

    /// <summary>
    /// Reimplementation of ChooseByPolicy to work with multidimensional documents
    /// </summary>
    /// <returns></returns>
    public Document BatchChooseByPolicy ()
    {

        List<float> samples = new List<float>();

        var VecBuilder = Vector<float>.Build;
        
        // calculate target vector of all categories

        for (int i = 0; i < Settings.Categories; ++i)
        {
            // get alpha and beta values for each category
            ABPair catAB = state.categoriesAB[i];

            var betaDist = new Beta(catAB.alpha, catAB.beta);

            float sample = (float)betaDist.Sample();

            samples.Add(sample);

        }

        // vector representing the ideal document according to this agents alpha / beta values
        var target = VecBuilder.DenseOfArray(samples.ToArray());

        //samples.Clear();

        Document chosen = state.documents[0];

        float minDist = 2f;

        for (int i = 0; i < state.documents.Count; ++i)
        {
            Document doc = state.documents[i];

            var docVec = VecBuilder.DenseOfArray(doc.values.ToArray());

            var distance = (float) Distance.Euclidean(target,docVec);

            // take euclidean distance between

            if (distance < minDist)
            {
                // update the minimum distance
                minDist = distance;
                // update the reference to closest document found so far
                chosen = doc;
            }

        }
        return chosen;
    }


    #endregion



    #region recommender helpers

    /// <summary>
    /// Computes the users interest in categories based on documents recently consumed by user
    /// </summary>
    /// <returns></returns>
    public Vector<float> GetHistoryAvgVec()
    {
        var VectorBuilder = Vector<float>.Build;

        if (_history.Count > 0)
        {

            var combined = VectorBuilder.Dense(_history.First.Value.values.Count);

            foreach (var d in _history)
            {
                combined += VectorBuilder.DenseOfArray(d.values.ToArray());
            }

            // divide combined by count
            return (combined / _history.Count);
        } else
        {
            return VectorBuilder.Dense(Settings.Categories, 0.0f);
        }


    }

    public LinkedList<Document> GetHistory()
    {
        return _history;
    }

    #endregion

    void Awake()
    {

        state = new State(1, 1);
        for (int j = 0; j < Settings.DocumentsPerAgent; j++)
        {
            List<float> vals = new List<float>();
            for (int k = 0; k < Settings.Categories; ++k)
            {
                vals.Add(Random.Range(0f, 1f));
            }
            Document d = new Document(vals);
            state.documents.Add(d);
        }

    }

    /// <summary>
    /// For the future: agents will be able to add documents to the system
    /// </summary>
    public void MakeDocument()
    {
        throw new System.NotImplementedException();
    }

}
