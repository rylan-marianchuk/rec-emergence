using System.Collections;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Statistics;
using MathNet.Numerics;
using UnityEngine;
using System;

/// <summary>
/// Emum for selecting which metric is to be used for calculating the simularity matrix used by the system
/// </summary>
public enum Metric
{
    Cosine,
    //CorrectedCosine,
    Pearson,
    Euclidean
}


public class Recommender
{
    /// <summary>
    /// The parameters wrapper for the recommender system 
    /// </summary>
    //public Parameters sysp { get; set; }

    /// <summary>
    /// Controls how many like users the algorithm identifies to make a recommendation
    /// </summary>
    public int LikeAgentNum { get; set; } = 3;

    /// <summary>
    /// The type of metric that is used to filter
    /// </summary>
    public Metric metric { get; set; } = Metric.Pearson;
    public Matrix<float> similarity { get; set; }

    /// <summary>
    /// Instantiation of objects that other scripts/objects rely on
    /// </summary>
    void Awake()
    {
        // Must initialize the parameters before the agents are initialized! 
        //sysp = new Parameters(10, 10);
        //Debug.Log("System parameters set");
    }

    private void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        // Recalculate the matrix
        bool init = false;
        if (!init)
        {
            similarity = calculateSimilarityMatrix();
            init = true;
            Debug.Log("Similarity matrix: " + similarity.ToString());
        }

        
        
        
    }

    public Matrix<float> calculateSimilarityMatrix()
    {
        //Calculate the matrix!

        // Gather all of the agents in the scene
        List<Agent> aList = Simulation.instance.agentList;


        List<Vector<float>> agentData = new List<Vector<float>>();

        // Group the scores from each agent into primitive matrix
        for (int i = 0; i < aList.Count; i++)
        {
            agentData.Add(aList[i].GetHistoryAvgVec());
        }

        // Some initialization for Math.NET / matrix algebra
        var MatBuilder = Matrix<float>.Build;
        var VecBuilder = Vector<float>.Build;

        // Convert the primitive matrix of all of the agents preferences
        Matrix<float> M = MatBuilder.DenseOfRows(agentData);

        // creates a similarity matrix
        Matrix<float> simMat = MatBuilder.Dense(Simulation.instance.agents, Simulation.instance.agents);

        for (int i = 0; i < Simulation.instance.agents; i++)
        {
            for (int j = 0; j < Simulation.instance.agents; j++)
            {

                if (metric == Metric.Cosine)
                {
                    // calculate the cosine similarity of the agents!
                    simMat[i, j] = CosineSimilarity(M.Row(i), M.Row(j));
                } else if (metric == Metric.Pearson)
                {
                    // calculate the pearson similarity of the agents!
                    simMat[i, j] = PearsonSimilarity(M.Row(i), M.Row(j));
                } else if (metric == Metric.Euclidean)
                {
                    // calculate the euclidean similarity of the agents!
                    simMat[i, j] = EuclideanSimilarity(M.Row(i), M.Row(j));
                }

            }
        }

        return simMat;
    }



    /// <summary>
    /// Calculates the cosine simularity of two vectors
    /// </summary>
    /// <param name="a">The first vector</param>
    /// <param name="b">The second vector</param>
    /// <returns></returns>
    public static float CosineSimilarity(Vector<float> a, Vector<float> b)
    {
        float dot = a.DotProduct(b);
        float denom = ( (float) a.L2Norm() ) * ((float)b.L2Norm());

        return dot / denom;
    }

    /// <summary>
    /// Pearson similarity calculates the similarity of two vectors. It returns a similarity value that ranges from -1.00 which is the most dissimilar, to 1.00 which is completely identical. 
    /// </summary>
    /// <param name="a">The first vector</param>
    /// <param name="b">The second vector</param>
    /// <returns></returns>
    public static float PearsonSimilarity(Vector<float> a, Vector<float> b)
    {
        float covariance = (float) Statistics.Covariance(a, b);
        float denom = ((float) Statistics.StandardDeviation(a)) * ((float)Statistics.StandardDeviation(b));

        return covariance / denom;
    }

    public static float EuclideanSimilarity(Vector<float> a, Vector<float> b)
    {
        float dist = (float) Distance.Euclidean(a, b);
        return 1 / (1 + dist);
    }

    /// <summary>
    /// Finds the other agents with the most similar score
    /// </summary>
    /// <param name="similarity">The simularity matrix to search</param>
    /// <param name="neighbours">The number of desired similar neighbours</param>
    /// <param name="agentNum"> The agent number to find similar agents to</param>
    /// <returns></returns>
    public static List<int> FindNearestNeighbours(Matrix<float> similarity, int neighbours, int agentNum)
    {

        // take the matrix row, and find the k closest values
        Vector<float> ratings = similarity.Row(agentNum);


        List<Tuple<int, float>> res = new List<Tuple<int, float>>();

        for (int i = 0; i < ratings.Count; i++)
        {
            if (i != agentNum)
            {
                Tuple<int, float> t = new Tuple<int, float>(i, ratings[i]);
                res.Add(t);
            }
        }

        // sort list according to double, extract first
        res.Sort((x, y) => y.Item2.CompareTo(x.Item2));

        List<int> closestAgents = new List<int>();
        // add the agentNum closest matches to results
        for (int i = 0; i < neighbours; i++)
        {
            // add the result to 
            closestAgents.Add(res[i].Item1);
        }

        return closestAgents;
    }


    /// <summary>
    /// Recommends a document to a user based off of agents similar to them 
    /// </summary>
    /// <param name="a"></param>
    /// <returns></returns>
    public Document targetedRecommend(Agent a)
    {
        // find the 3 most similar agent numbers
        List<int> likeAgentNums = FindNearestNeighbours(similarity,LikeAgentNum, a.ID);




        // History metric comparison
        /*
         * Concept: compare the recent histories of users, adding the distance between 
         * to create a calculate history difference metric.
         * 
         * Then, the recommender will select at random one of the most recent items it has not yet read?
         * 
         */


        // Current metric: completely random document-document filtering

        // Choose one of the similar users at random, choose one of their recently consumed documents at random
        
        var selectedAgent = likeAgentNums[UnityEngine.Random.Range(0, likeAgentNums.Count)];

        int boundedRandom = UnityEngine.Random.Range(1, 6); // between 1 and 5

        var simAgent = Simulation.instance.agentList[selectedAgent];
        var d = simAgent.GetHistory().First;
        int index = 0;
        while (index < Math.Min(simAgent.GetHistory().Count, boundedRandom))
        {
            d = d.Next;
        }

        return d.Value; // return the document attached to the node

    }

    /* DEPRECATED: uniform recommender
    public List<Document> recommend(Agent a)
    {
        List<Document> l = new List<Document>();
        foreach (var document in a.state.getDocuments())
        {
            l.Add(new Document(UnityEngine.Random.Range(0f, 1f)));
        }
        return l;
    }*/
}
