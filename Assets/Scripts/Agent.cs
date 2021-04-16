using System.Collections;
using System.Collections.Generic;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics;
using UnityEngine;



public class Agent : MonoBehaviour
{
    public State state;
    public Vector3 goalPosition;
    private LinkedList<Document> _history = new LinkedList<Document>();

    public List<float> consumedHistory = new List<float>();
    public int HistorySize { get; set; } = 20;
    /**
     * A policy, as defined in the reinforcement learning literature, is a function pi( a  |  s ) that specifies the probability of taking action a if in state s.
     * 
     * The policy for each agent is as follows:
     *      1. sample from its rewward distribution as specified by alpha and beta (the distribution is a Beta pdf)
     *      2. use this sample and compare its value with all other document values.
     *      3. Choose the document that is closest to the sample. (Choosing a document IS the action.)
     *   return that chosen document
     *   
     *   bool diversity: 
     */
    public Document chooseByPolicy(bool diversity)
    {
        var betaDist = new Beta(state.getAlpha(), state.getBeta());
        var betaDist1 = new Beta(state.getAlpha(), state.getBeta());
        float sample = (float)betaDist.Sample();
        float sample1 = (float)betaDist1.Sample();

        //Debug.Log("SAMPLE CHOSEN: " + sample);

        // Find the document closest to sample
        Document chosen = state.getDocuments()[0];

        float mindist = 2f;
        float maxdist = -2f;
        for (int i = 0; i < state.getDocuments().Count; i++)
        {
            float dist = Mathf.Sqrt((state.getDocuments()[i].value - sample) * (state.getDocuments()[i].value - sample) + (state.getDocuments()[i].value1 - sample1) * (state.getDocuments()[i].value1 - sample1));
            if (dist < mindist && !diversity)
            {
                mindist = dist;
                chosen = state.getDocuments()[i];
            }
            else if (dist > maxdist && diversity)
            {
                maxdist = dist;
                chosen = state.getDocuments()[i];
            }
        }

        
        return chosen;
    }

    /**
     * 
     * Upon consumption of a document, alpha and beta must update according to some rule.
     * 
     * Paradigm: Facilitated polared learning. Learning happens the most given extreme documents.
     */
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

        float nextAlpha1 = sampleGaussian(meanLearningAlpha(consumed.value1), 0.2f) + this.state.getAlpha1();
        if (nextAlpha1 <= 0)
            state.setAlpha1(0.001f);
        else state.setAlpha1(nextAlpha1);


        float nextBeta1 = sampleGaussian(meanLearningBeta(consumed.value1), 0.2f) + this.state.getBeta1();
        if (nextBeta1 <= 0)
            state.setBeta1(0.001f);
        else state.setBeta1(nextBeta1);
    }


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
     * Agents are located in the space according to their alpha and beta instance values.
     * Alpha and beta are the parameters of the reward distribution for a given belief.
     * 
     */
    public void updatePosition()
    {
        float newAxisCoordx = (state.getAlpha() / (state.getBeta() + state.getAlpha()) - 0.5f) * 100f;
        float newAxisCoordz = (state.getBeta1() / (state.getBeta1() + state.getAlpha1()) - 0.5f) * 100f;
        this.goalPosition = new Vector3(transform.position.x, transform.position.y, newAxisCoordx);
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



    public LinkedList<Document> GetHistory()
    {
        return _history;
    }

    public void AddToHistory(Document d)
    {
        _history.AddFirst(d);

        if (_history.Count == HistorySize + 1)
        {
            _history.RemoveLast();
        }

    }

}
