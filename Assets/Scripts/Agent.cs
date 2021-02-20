using System.Collections;
using System.Collections.Generic;


using UnityEngine;



public class Agent : MonoBehaviour
{
    public State state;

    public Document chooseByPolicy()
    {
        float sample = Probability.samplefromBeta(state.getAlpha(), state.getBeta());

        // Find the document closest to sample
        return new Document();
    }



    public void learn(Document consumed)
    {

    }


    public void updatePosition()
    {

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

    void Update()
    {
        
    }



}
