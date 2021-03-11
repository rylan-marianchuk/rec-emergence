using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedSim : MonoBehaviour
{
    // Parameters
    public static AdvancedSim instance;

    public int agents;
    public int documentsPerAgent;

    public int categories;

    public List<Agent> agentList;
    public List<Document> documentList;

    public float waitTime;
    public long iteration = 0;
    public long iterationCap = 200;

    public GameObject agentGo;
    public GameObject room;
    public GameObject documentGo;
    public GameObject agentController;

    private Recommender recommender;
    public List<Document> DocumentPool { get; set; }


    private int counter;
    public bool shouldWriteResults = false;
    private bool writtenResults = false;

    private List<float[]> documentHistory;
    private List<float[]> agentEndState;

    public Dictionary<int, List<GameObject>> agentModelMap = new Dictionary<int, List<GameObject>>();

    void Start()
    {
        // Initialize agents & documents
        instance = this;
        recommender = new Recommender();

        documentHistory = new List<float[]>();
        agentEndState = new List<float[]>();

        //Parameters p = GameObject.FindGameObjectWithTag("Recommender").GetComponent<Recommender>().sysp;


        for (int i = 0; i < agents; i++)
        {
            // create the agent objects that correspond to models
            GameObject aControl = Instantiate(agentController,new Vector3(),Quaternion.identity);
            Agent a = aControl.GetComponent<Agent>();


            // for now, we will just randomly generate the values
            a.state = new State(1, 1);
            for (int j = 0; j < documentsPerAgent; j++)
            {
                List<float> vals = new List<float>();
                for (int k = 0; k < categories; ++k)
                {
                    vals.Add(Random.Range(0f, 1f));
                }
                Document d = new Document(vals);
                a.state.documents.Add(d);
            }

            agentList.Add(a);
            
            agentModelMap.Add(i, new List<GameObject>());
        }


        for (int i = 0; i < categories; i++)
        {
            
            // central position of each room
            Vector3 roomNexus = new Vector3(
                Mathf.Cos(2 * i * Mathf.PI / categories),
                0,
                Mathf.Sin(2 * i * Mathf.PI / categories)) * 50f;

            GameObject localRoom = Instantiate(room, roomNexus, Quaternion.identity) ;
            Vector3 pos = localRoom.transform.position;
            pos.x = pos.x - 5f;
            pos.y += 1f;
            // for each room, we want n "agent" models represented
            float delta = 10 / agents;
            pos.x -= delta * 0.5f;
            for (int n = 0; n < agents; n++)
            {
                // create each agent along the axis of the room (currently positive X direction)
                // z axis will vary
                

                // everything is 10 wide

                pos.x += delta;
                //Vector3 RelativeLocation = new Vector3((pos.x -5f)+ (10f/(n + 1)), pos.y + 0.3f, pos.z);
                Vector3 RelativeLocation = pos;

                
                GameObject agentClone = Instantiate(agentGo, RelativeLocation, Quaternion.identity);
                agentClone.transform.localScale = (Vector3.one * 0.3f);

                agentModelMap[n].Add(agentClone); // represents the nth agents ith category

                // create the documents


            }


        }

    }


    void FixedUpdate()
    {
        if (counter == waitTime)
        {

            foreach (Agent agent in agentList)
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

            for (int i =0; i <agentList.Count; i++)
            {
                List<GameObject> agentModels = agentModelMap[i];
                List<ABPair> abs = agentList[i].state.categoriesAB;

                for (int j = 0; j < categories; j++)
                {

                    float newAxisCoord = (abs[i].alpha / abs[i].beta + abs[i].alpha);
                    newAxisCoord = (newAxisCoord - 0.5f) * 10.0f;
                    Debug.Log(newAxisCoord);
                    Vector3 goalPos = new Vector3(transform.position.x, transform.position.y, newAxisCoord);
                    agentModelMap[i][j].GetComponent<AgentCategoryModel>().goalPosition = goalPos;
                }

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

}
