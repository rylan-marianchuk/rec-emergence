using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphSimulation : Singleton<GraphSimulation>
{
    // Parameters

    public List<Agent> Agents;
    public List<Document> documentList;

    public Recommender recommender;
    public List<Document> DocumentPool { get; set; }

    public float waitTime;
    private int counter;

    void Start()
    {
        // Initialize agents & documents
        recommender = new Recommender();

        for (int i = 0; i < Settings.NumberAgents; i++)
        {
            // create the agent objects that correspond to models
            GameObject gameObject = new GameObject("Agent " + i);
            gameObject.AddComponent<Agent>(); 
            Agent a = gameObject.GetComponent<Agent>();


            // this should happen in the agent's startup
            // for now, we will just randomly generate the values
            a.state = new State(1, 1);
            for (int j = 0; j < Settings.DocumentsPerAgent; j++)
            {
                List<float> vals = new List<float>();
                for (int k = 0; k < Settings.Categories; ++k)
                {
                    vals.Add(Random.Range(0f, 1f));
                }
                Document d = new Document(vals);
                a.state.documents.Add(d);
            }

            Agents.Add(a);

            
        }



    }


    void FixedUpdate()
    {
        if (counter == waitTime)
        {

            foreach (Agent agent in Agents)
            {
                // For each agent, choose a document or choose to consume from the list of 
                // documents recommended to them (random at start)
                Document chosen = agent.BatchChooseByPolicy();
                // add it to the user's history
                agent.AddToHistory(chosen);
                // tweak learning stats of agent
                agent.GetComponent<Agent>().BatchLearn(chosen);
            }
            Debug.Log("First Wait");
        }
        else if (counter == waitTime * 2)
        {

            

        }
        else if (counter == waitTime * 3)
        {



            Debug.Log("Third Wait, updated documents by recommendation");
            counter = 0;

        }
        counter++;

    }

}
