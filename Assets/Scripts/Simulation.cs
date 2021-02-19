﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simulation : MonoBehaviour
{
    // Parameters

    void Start()
    {
        // Initialize agents & documents

        Parameters p = GameObject.FindGameObjectWithTag("Recommender").GetComponent<Recommender>().sysp;

        for (int i =0; i< p.NumberAgents;i++)
        {
            // generate the agents dynamically
            GameObject agent = new GameObject("Agent " + i);
            agent.tag = "Agent";
            agent.AddComponent<Agent>();

        }


        // Randomly assign agent parameters

    }

    void FixedUpdate()
    {
        // Wait
        // For each agent, choose a document or choose to post - add to recommender pool - RecSim corpus

        // Wait
        // Move agent after consumption Vector3 position

        // Wait
        // Recommender replenish with a new document
    }
}
