using System.Collections;
using System.Collections.Generic;
using MathNet.Numerics.Distributions;
using UnityEngine;



public class Agent : MonoBehaviour
{
    public State state;

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



    public void learn(Document consumed)
    {
        state.setAlpha(5);
        state.setBeta(5);
    }


    public void updatePosition()
    {
        transform.position = new Vector3(state.getBeta() * 10, transform.position.y, state.getAlpha() * 10);
        // Move bubble and documents with it

        for (int i = 0; i < Simulation.instance.documentsPerAgent; i++)
        {
            state.getDocuments()[i].transform.position = transform.position + new Vector3(
                Mathf.Cos( 2 * i * Mathf.PI / Simulation.instance.documentsPerAgent), 
                transform.position.y, 
                Mathf.Sin( 2 * i * Mathf.PI / Simulation.instance.documentsPerAgent))*3;
        }
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
