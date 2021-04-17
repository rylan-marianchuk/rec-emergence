using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphSimulation : Singleton<GraphSimulation>
{
    // Parameters

    public List<Agent> Agents;


    public Recommender recommender;

    public float waitTime;
    private int counter;

    private float resetTimer = 0.5f;
    private float elapsed = 0.0f;

    private bool canStart = true;

    public Slider Categories;

    public Slider NumberAgents;

    public Slider DocumentsPerAgent;

    public Slider NumberSimilarAgents;

    public Slider NumberCategoriesSelected;

    public Slider Engagement;

    public Dropdown SimilarityMetric;

    public Dropdown Policy;

    public Button Reset;

    public void Restart()
    {
        if (Agents != null)
        {
            foreach (var item in Agents)
            {
                Destroy(item.gameObject);
            }
            Agents.Clear();
        }



        Settings.Categories = (int)Categories.value;
        Settings.NumberAgents = (int)NumberAgents.value;
        Settings.DocumentsPerAgent = (int)DocumentsPerAgent.value;
        Settings.NumberSimilarAgents = (int)NumberSimilarAgents.value;
        Settings.NumberCategoriesSelected = (int)NumberCategoriesSelected.value;
        Settings.Engagement = Engagement.value;

        int p = Policy.value;

        if (p == 0)
        {
            Settings.diversity = false;
        } else
        {
            Settings.diversity = true;
        }

        int m = SimilarityMetric.value;

        if (m == 0)
        {
            Settings.metric = Metric.Pearson;
        } else if (m == 1)
        {
            Settings.metric = Metric.Cosine;
        } else
        {
            Settings.metric = Metric.Euclidean;
        }


        

        for (int i = 0; i < Settings.NumberAgents; i++)
        {
            // create the agent objects that correspond to models
            GameObject gameObject = new GameObject("Agent " + i);
            gameObject.AddComponent<Agent>();
            Agent a = gameObject.GetComponent<Agent>();
            a.ID = i;

            Agents.Add(a);


        }



        recommender = new Recommender();
        recommender.similarity = null;
        canStart = false;

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



        }


    }

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


    private void Start()
    {
        Reset.onClick.AddListener(Restart);
    }

    void FixedUpdate()
    {

        if (!canStart)
        {
            if (elapsed < resetTimer)
            {
                elapsed += Time.deltaTime;

            } else
            {
                canStart = true;
            }

        } else
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

}
