using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public GameObject agentGo;
    public GameObject documentGo;

    private Recommender recommender;
    private int counter;
    void Start()
    {
        // Initialize agents & documents
        instance = this;
        recommender = new Recommender();

        //Parameters p = GameObject.FindGameObjectWithTag("Recommender").GetComponent<Recommender>().sysp;

        for (int i = 0; i < agents; i++)
        {
            // generate the agents dynamically
            GameObject created = Instantiate(agentGo, new Vector3(0, 4*i, 0), Quaternion.identity);
            created.GetComponent<Agent>().state = new State(1, 1);
            
            agentList.Add(created.GetComponent<Agent>());
            // Documents for that agent
            // Generate all documents by a specified prior distribution
            for (int j = 0; j < documentsPerAgent; j++)
            {
                // Instantiate a unity object for each document, in a circle around the agent. 
                // TODO make these move relative (vertically) to the agent
                GameObject doc = Instantiate(documentGo, created.transform);
                doc.transform.position = created.transform.position + new Vector3(
                Mathf.Cos(2 * i * Mathf.PI / Simulation.instance.documentsPerAgent),
                created.transform.position.y,
                Mathf.Sin(2 * i * Mathf.PI / Simulation.instance.documentsPerAgent)) * 3;

                // Create each document
                doc.GetComponent<Document>().value = Random.Range(0f, 1f); // assign value to the document
                // Add the documents to the state of a given agent
                created.GetComponent<Agent>().state.addDocument(doc.GetComponent<Document>());
            }
        }
    }

    
    void FixedUpdate()
    {
        if (counter == waitTime)
        {

            foreach (Agent agent in agentList)
            {
                // For each agent, choose a document or choose to post - add to recommender pool - RecSim corpus
                Document chosen = agent.chooseByPolicy();
                agent.GetComponent<Agent>().learn(chosen);
            }
            Debug.Log("First Wait");
        }
        else if (counter == waitTime*2)
        {
            foreach (Agent agent in agentList)
            {
                // Move agent after consumption Vector3 position
                agent.updatePosition();
            }

            Debug.Log("Second Wait, updated position");

        }
        else if (counter == waitTime * 3)
        {

            foreach (Agent agent in agentList)
            {
                // Recommender replenish with a (all?) new document
                //agent.state.setDocuments(recommender.recommend(agent));
            }

            Debug.Log("Third Wait, updated documents by recommendation");
            counter = 0;
            iteration++;
        }
        counter++;
    }
    /*

    private IEnumerator ChooseByPolicyCoroutine()
    {
        foreach (Agent agent in agentList)
        {
            // For each agent, choose a document or choose to post - add to recommender pool - RecSim corpus
            Document chosen = agent.chooseByPolicy();
            agent.GetComponent<Agent>().learn(chosen);
        }

        Debug.Log("First Wait");
        // Wait
        yield return new WaitForSeconds(waitTime);
    }

    private IEnumerator UpdatePosition()
    {
        foreach (Agent agent in agentList)
        {
            // Move agent after consumption Vector3 position
            agent.updatePosition();
        }

        Debug.Log("Second Wait, updated position");
        yield return new WaitForSeconds(waitTime);
    }

    private IEnumerator Recommend()
    {
        foreach (Agent agent in agentList)
        {
            // Recommender replenish with a (all?) new document
            agent.state.setDocuments(recommender.recommend(agent));
        }

        Debug.Log("Third Wait, updated documents by recommendation");
        // Wait
        yield return new WaitForSeconds(waitTime);
    }
    */
}
