using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphSimulation : Singleton<GraphSimulation>
{
    // Parameters

    public List<Agent> Agents;


    public Recommender recommender;

    public float waitTime;
    private int counter;

    void Awake()
    {

        for (int i = 0; i < Settings.NumberAgents; i++)
        {
            // create the agent objects that correspond to models
            GameObject gameObject = new GameObject("Agent " + i);
            gameObject.AddComponent<Agent>(); 
            Agent a = gameObject.GetComponent<Agent>();
            a.ID = i;

            Agents.Add(a);

            
        }
        // Initialize agents & documents
        recommender = new Recommender();


    }


    void FixedUpdate()
    {

            foreach (Agent agent in Agents)
            {

                // For each agent, choose a document or choose to consume from the list of 
                // documents recommended to them (random at start)
                Document chosen = agent.BatchChooseByPolicy();

                if (recommender.similarity != null)
                    agent.state.addDocument(recommender.targetedRecommend(agent));

                // add it to the user's history
                agent.AddToHistory(chosen);

            // tweak learning stats of agent

            agent.BatchLearn(chosen);


            if (Random.value < Settings.Engagement)
            {
                agent.AddToHistory(agent.MakeDocument());
            }


        }

    }

}
