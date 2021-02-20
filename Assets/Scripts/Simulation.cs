using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simulation : MonoBehaviour
{
    // Parameters
    public static Simulation instance;

    public int agents;
    public int documents;
    public int documentsPerAgent;

    public int categories;

    public List<GameObject> agentList;
    public List<Document> documentList;

    public float waitTime;
    public long iteration = 0;

    private Recommender recommender;
    void Start()
    {
        // Initialize agents & documents
        instance = this;
        recommender = new Recommender();

        //Parameters p = GameObject.FindGameObjectWithTag("Recommender").GetComponent<Recommender>().sysp;

        for (int i = 0; i < agents; i++)
        {
            // generate the agents dynamically
            GameObject agent = new GameObject("Agent " + i);
            agent.tag = "Agent"; // add the proper tag, so other scripts can reference them. 
            agent.AddComponent<Agent>(); // add the Agent script to each agent

            agentList.Add(agent);
        }


        // Generate all documents by a specified prior distribution
        for (int i = 0; i < documents; i++)
        {

        }
    }

    void FixedUpdate()
    {
        foreach (GameObject agent in agentList)
        {
            // For each agent, choose a document or choose to post - add to recommender pool - RecSim corpus
            Document chosen = agent.GetComponent<Agent>().chooseByPolicy();
        }

        // Wait
        StartCoroutine(WaitCoroutine());

        foreach (GameObject agent in agentList)
        {
            // Move agent after consumption Vector3 position
            agent.GetComponent<Agent>().updatePosition();
        }


        // Wait
        StartCoroutine(WaitCoroutine());


        foreach (GameObject agent in agentList)
        {
            // Recommender replenish with a (all?) new document
        }


        // Wait
        StartCoroutine(WaitCoroutine());
        

        iteration++;
    }

    private IEnumerator WaitCoroutine()
    {
        yield return new WaitForSeconds(waitTime);
    }
}
