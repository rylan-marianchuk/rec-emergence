using System.Collections;
using System.Collections.Generic;
using MathNet.Numerics.Distributions;
using UnityEngine;



public class Agent : MonoBehaviour
{
    public State state;
    public Vector3 goalPosition;

    public List<float> consumedHistory = new List<float>();
    /**
     * A policy, as defined in the reinforcement learning literature, is a function pi( a  |  s ) that specifies the probability of taking action a if in state s.
     * 
     * The policy for each agent is as follows:
     *      1. sample from its rewward distribution as specified by alpha and beta (the distribution is a Beta pdf)
     *      2. use this sample and compare its value with all other document values.
     *      3. Choose the document that is closest to the sample. (Choosing a document IS the action.)
     *   return that chosen document
     */
    public Document chooseByPolicy()
    {
        var betaDist = new Beta(state.getAlpha(), state.getBeta());
        float sample = (float)betaDist.Sample();

        Debug.Log("SAMPLE CHOSEN: " + sample);

        // Find the document closest to sample
        Document chosen = state.getDocuments()[0];

        float mindist = 2f;
        for (int i = 0; i < state.getDocuments().Count; i++)
        {
            float dist = Mathf.Abs(state.getDocuments()[i].value - sample);
            if (dist < mindist)
            {
                mindist = dist;
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
        float newAxisCoord = (state.getAlpha() / (state.getBeta() + state.getAlpha()) - 0.5f) * 50f;
        this.goalPosition = new Vector3(transform.position.x, transform.position.y, newAxisCoord);
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


    /// <summary>
    /// Should have some level of interest in categories. I was thinking these should all start around 5 on a scale of 1-10? 
    /// This way, the agents will have at least some bearing without starting them off with an already extreme profile.
    /// </summary>
    public List<int> Scores { get; set; }

    void Start()
    {
        /*
        // Creates the system parameters

        // GameObject containing the recommender
        //rObj = GameObject.FindGameObjectWithTag("Recommender");

        // get the script attached to the recommender object!
        //r = rObj.GetComponent<Recommender>();

        // Generate random set of interests
        // populate the agent's ratings
        // generates random values from 0-10 for now
        // @TODO: change for later!
        Scores = new List<int>();

        for (int i = 0; i < r.sysp.NumberCategories; i++)
        {
            Scores.Add(Random.Range(0, 10));
        }

        
        string log = "";
        foreach (var item in Scores)
        {
            log+=item.ToString() + " ";
        }
        Debug.Log("Agent initialized with preferences: " + log);

        */
    }

}
