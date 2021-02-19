using System.Collections;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Statistics;
using MathNet.Numerics;
using UnityEngine;




public class Parameters
{
    /// <summary>
    /// Controls the number of agents in the system
    /// </summary>
    public int NumberAgents { get; set; }
    /// <summary>
    /// Controls the number of documents in the system
    /// </summary>
    public int NumberDocuments { get; set; }
    /// <summary>
    /// Controls how many like users the algorithm identifies to make a recommendation
    /// </summary>
    public int LikeAgentNum { get; set; }

    /// <summary>
    /// The type of metric that is used to filter
    /// </summary>
    public Metric metric { get; set; }

    /// <summary>
    /// Sets the system parameters according to the given arguments.
    /// </summary>
    /// <param name="agents">the number of agents in the system</param>
    /// <param name="docs">the number of documents in the system</param>
    public Parameters(int agents, int docs)
    {
        NumberAgents = agents;
        NumberDocuments = docs;
        metric = Metric.Cosine;
    }

}


public enum Metric
{
    Cosine,
    CorrectedCosine,
    Pearson,
    Euclidean
}


public class Recommender : MonoBehaviour
{

    public Parameters sysp { get; set; }

    List<Agent> Agents = new List<Agent>();

    // Start is called before the first frame update
    void Start()
    {

        sysp = new Parameters(10, 10);

        // get a list of the agents somehow

        for (int i = 0; i < sysp.NumberAgents; i++)
        {
            Agents.Add(new Agent());
        }


    }

    // Update is called once per frame
    void Update()
    {

        // Recalculate the matrix

        
    }
}
