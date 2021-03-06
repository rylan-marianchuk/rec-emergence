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
    public int LikeAgentNum { get; set; }

    /// <summary>
    /// The type of metric that is used to filter
    /// </summary>
    public Metric metric { get; set; }
    public Matrix<double> similarity { get; set; }

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
        bool init = false;
        if (!init)
        {
            similarity = calculateSimilarityMatrix();
            init = true;
            Debug.Log("Similarity matrix: " + similarity.ToString());
        }

        // Recalculate the matrix
        
        
    }

    public Matrix<double> calculateSimilarityMatrix()
    {
        //Calculate the matrix!

        // Gather all of the agents in the scene
        GameObject[] agents = GameObject.FindGameObjectsWithTag("Agent");

        // Initialize a matrix according to system parameters
        double[,] matrix = new double[Simulation.instance.agents, Simulation.instance.categories];

        // Group the scores from each agent into primitive matrix
        for (int i = 0; i < agents.Length; i++)
        {
            int[] temp = agents[i].GetComponent<Agent>().Scores.ToArray();
            for (int j = 0; j < Simulation.instance.categories; j++)
            {
                matrix[i, j] = (double)temp[j];
            }
        }

        // Some initialization for Math.NET / matrix algebra
        var MatBuilder = Matrix<double>.Build;
        var VecBuilder = Vector<double>.Build;

        // Convert the primitive matrix of all of the agents preferences
        Matrix<double> M = DenseMatrix.OfArray(matrix);
        // creates a similarity matrix
        Matrix<double> simMat = MatBuilder.Dense(Simulation.instance.agents, Simulation.instance.agents);

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
    public static double CosineSimilarity(Vector<double> a, Vector<double> b)
    {
        double dot = a.DotProduct(b);
        double denom = a.L2Norm() * b.L2Norm();

        return dot / denom;
    }

    /// <summary>
    /// Pearson similarity calculates the similarity of two vectors. It returns a similarity value that ranges from -1.00 which is the most dissimilar, to 1.00 which is completely identical. 
    /// </summary>
    /// <param name="a">The first vector</param>
    /// <param name="b">The second vector</param>
    /// <returns></returns>
    public static double PearsonSimilarity(Vector<double> a, Vector<double> b)
    {
        double covariance = Statistics.Covariance(a, b);
        double denom = Statistics.StandardDeviation(a) * Statistics.StandardDeviation(b);

        return covariance / denom;
    }

    public static double EuclideanSimilarity(Vector<double> a, Vector<double> b)
    {
        double dist = Distance.Euclidean<double>(a, b);
        return 1 / (1 + dist);
    }

    /// <summary>
    /// Finds the other agents with the most similar score
    /// </summary>
    /// <param name="similarity">The simularity matrix to search</param>
    /// <param name="neighbours">The number of desired similar neighbours</param>
    /// <param name="agentNum"> The agent number to find similar agents to</param>
    /// <returns></returns>
    public static List<int> FindNearestNeighbours(Matrix<double> similarity, int neighbours, int agentNum)
    {

        // take the matrix row, and find the k closest values
        Vector<double> ratings = similarity.Row(agentNum);


        List<Tuple<int, double>> res = new List<Tuple<int, double>>();

        for (int i = 0; i < ratings.Count; i++)
        {
            if (i != agentNum)
            {
                Tuple<int, double> t = new Tuple<int, double>(i, ratings[i]);
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


    public List<Document> recommend(Agent a)
    {
        List<Document> l = new List<Document>();
        foreach (var document in a.state.getDocuments())
        {
            l.Add(new Document(UnityEngine.Random.Range(0f, 1f)));
        }
        return l;
    }
}
