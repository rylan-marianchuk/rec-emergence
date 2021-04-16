﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Simulation : MonoBehaviour
{
    // Parameters
    public static Simulation instance;

    public int agents;
    public int documentsPerAgent;

    public int categories;

    public List<Agent> agentList;
    public List<Document> documentList;

    public float waitTime;
    public long iteration = 0;
    public long iterationCap;

    public GameObject agentGo;
    public GameObject documentGo;

    private Recommender recommender;
    private int counter;
    public bool shouldWriteResults = false;
    private bool writtenResults = false;

    private List<float[]> documentHistory;
    private List<float[]> agentEndState;

    private bool canStart = false;

    /// <summary>
    /// UI SECTION
    /// </summary>
    
    /// <summary>
    /// Number of agents.
    /// </summary>
    public Slider n;

    /// <summary>
    /// Documents per agent
    /// </summary>
    public Slider d;

    /// <summary>
    /// Time Cap
    /// </summary>
    public Slider T;


    /// <summary>
    /// Number of agents any given agents uses for its recommendation
    /// </summary>
    public int r;


    /// <summary>
    /// Recommender similarity metric: ('E') Euclidean Distance, ('P') Pearson
    /// </summary>
    public char R;

    /// <summary>
    /// Policy : ('F') Seeking Familiarity, ('D') Seeking Diversity
    /// </summary>
    public InputField P;



    public void Restart()
    {
        if (agentList != null)
        {
            // Destory them all and recreate them
            foreach (var item in agentList)
            {
                Destroy(item.gameObject);
            }
        }
        iteration = 0;
        this.documentsPerAgent = (int)d.value;
        this.agents = (int)n.value;
        this.iterationCap = (int)T.value;

        // Initialize agents & documents
        instance = this;
        recommender = new Recommender();

        documentHistory = new List<float[]>();
        agentEndState = new List<float[]>();

        //Parameters p = GameObject.FindGameObjectWithTag("Recommender").GetComponent<Recommender>().sysp;

        for (int i = 0; i < agents; i++)
        {
            // generate the agents dynamically
            GameObject created = Instantiate(agentGo, new Vector3(-100 + 6.5f*i, 1.5f, 0), Quaternion.identity);
            created.GetComponent<Agent>().state = new State();
            agentList.Add(created.GetComponent<Agent>());

            documentHistory.Add(new float[iterationCap]);
            // Documents for that agent
            // Generate all documents by a specified prior distribution
            for (int j = 0; j < documentsPerAgent; j++)
            {
                GameObject doc = Instantiate(documentGo, created.transform);
                doc.transform.position = created.transform.position + new Vector3(
                Mathf.Cos(2 * j * Mathf.PI / Simulation.instance.documentsPerAgent),
                0,
                Mathf.Sin(2 * j * Mathf.PI / Simulation.instance.documentsPerAgent)) * 2.4f;
                doc.transform.forward = (created.transform.position - doc.transform.position);
                doc.transform.Rotate(0, 90, 0);
                doc.GetComponent<Document>().value = Random.Range(0f, 1f);
                created.GetComponent<Agent>().state.addDocument(doc.GetComponent<Document>());
            }
        }
        canStart = true;
    }


    void FixedUpdate()
    {
        if (!canStart) return;
        if (iteration == iterationCap)
        {
            if (!writtenResults && shouldWriteResults)
            {
                foreach (Agent agent in agentList)
                {
                    agentEndState.Add(new float[2] { agent.GetComponent<Agent>().state.getAlpha(), agent.GetComponent<Agent>().state.getBeta() });
                }
                FileWrite.WriteSummary(documentHistory, agentEndState);
                writtenResults = true;
            }
            return;
        }

        if (counter == waitTime)
        {
            int i = 0;
            foreach (Agent agent in agentList)
            {
                // For each agent, choose a document or choose to post - add to recommender pool - RecSim corpus
                Document chosen = agent.chooseByPolicy(P.text.Equals("D"));
                documentHistory[i][iteration] = chosen.value;
                agent.AddToHistory(chosen);
                agent.GetComponent<Agent>().learn(chosen);
                i++;
            }
            //Debug.Log("First Wait");
        }
        else if (counter == waitTime*2)
        {
            foreach (Agent agent in agentList)
            {
                // Move agent after consumption Vector3 position
                agent.updatePosition();
            }

            //Debug.Log("Second Wait, updated position");

        }
        else if (counter == waitTime * 3)
        {

            foreach (Agent agent in agentList)
            {
                // Recommender replenish with a (all?) new document
                agent.state.setDocuments(recommender.recommend(agent));
            }

            //Debug.Log("Third Wait, updated documents by recommendation");
            counter = 0;
            iteration++;
        }
        counter++;

        foreach (Agent agent in agentList)
        {
            // For each agent, choose a document or choose to post - add to recommender pool - RecSim corpus

            agent.transform.position = Vector3.Lerp(agent.transform.position, agent.GetComponent<Agent>().goalPosition, Time.deltaTime*0.05f);
        }
    }
}
