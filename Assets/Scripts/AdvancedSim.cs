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

    private Recommender recommender;
    private int counter;
    public bool shouldWriteResults = false;
    private bool writtenResults = false;

    private List<float[]> documentHistory;
    private List<float[]> agentEndState;

    void Start()
    {
        // Initialize agents & documents
        instance = this;
        recommender = new Recommender();

        documentHistory = new List<float[]>();
        agentEndState = new List<float[]>();

        //Parameters p = GameObject.FindGameObjectWithTag("Recommender").GetComponent<Recommender>().sysp;


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
                
                // create the documents

            }


        }

    }


    void FixedUpdate()
    {
    }

}
