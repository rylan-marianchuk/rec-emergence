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


    private bool canStart = false;

    public Slider Categories;

    public Slider NumberAgents;

    public Slider DocumentsPerAgent;

    public Slider NumberSimilarAgents;

    public Slider NumberCategoriesSelected;

    public Slider Engagement;

    public Dropdown SimilarityMetric;

    public Dropdown Policy;


    public void Restart()
    {
        if (Agents != null)
        {
            foreach (var item in Agents)
            {
                Destroy(item.gameObject);
            }
        }

        Settings.Categories = (int)Categories.value;
        Settings.NumberAgents = (int)NumberAgents.value;
        Settings.DocumentsPerAgent = (int)DocumentsPerAgent.value;
        Settings.NumberSimilarAgents = (int)NumberSimilarAgents.value;


        Policy.options.Add(new Dropdown.OptionData("Diversity"));
        Policy.options.Add(new Dropdown.OptionData("Familiarity"));


        SimilarityMetric.options.Add(new Dropdown.OptionData("Pearson"));
        SimilarityMetric.options.Add(new Dropdown.OptionData("Euclidean"));
        SimilarityMetric.options.Add(new Dropdown.OptionData("Cosine"));

        Settings.Engagement = Engagement.normalizedValue;

        for (int i = 0; i < Settings.NumberAgents; i++)
        {
            // create the agent objects that correspond to models
            GameObject gameObject = new GameObject("Agent " + i);
            gameObject.AddComponent<Agent>();
            Agent a = gameObject.GetComponent<Agent>();
            a.ID = i;

            Agents.Add(a);


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
