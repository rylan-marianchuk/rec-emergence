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
    /// The type of metric that is used to filter
    /// </summary>
    public Metric metric { get; set; } = Metric.Pearson;
    public Matrix<float> similarity { get; set; }

    public Recommender()
    {

    }

    public Matrix<float> calculateSimilarityMatrix()
    {
        //Calculate the matrix!

        // Gather all of the agents in the scene
        List<Agent> aList = GraphSimulation.Instance.Agents;


        // Some initialization for Math.NET / matrix algebra
        var MatBuilder = Matrix<float>.Build;
        //var VecBuilder = Vector<float>.Build;



        Matrix<float> simMat = MatBuilder.Dense(Settings.NumberAgents, Settings.NumberAgents, 0.0f);

        for (int i = 0; i < Settings.NumberAgents; i++)
        {
            Vector<float> AgentI = aList[i].GetHistoryAvgVec();
            for (int j = 0; j < Settings.NumberAgents; j++)
            {
                Vector<float> AgentJ = aList[j].GetHistoryAvgVec();
                if (metric == Metric.Cosine)
                {
                    // calculate the cosine similarity of the agents!
                    simMat[i, j] = CosineSimilarity(AgentI, AgentJ);
                } else if (metric == Metric.Pearson)
                {
                    // calculate the pearson similarity of the agents!
                    simMat[i, j] = PearsonSimilarity(AgentI, AgentJ);
                } else if (metric == Metric.Euclidean)
                {
                    // calculate the euclidean similarity of the agents!
                    simMat[i, j] = EuclideanSimilarity(AgentI, AgentJ);
                }

            }
        }
        similarity = simMat;
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
        if (similarity == null)
        {
            return new List<int>();
        }
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

        if (similarity == null)
        {
            return a.MakeDocument();
        }

        // find the 3 most similar agent numbers
        List<int> likeAgentNums = FindNearestNeighbours(similarity,Settings.NumberSimilarAgents, a.ID);




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

        var simAgent = GraphSimulation.Instance.Agents[selectedAgent];
        var d = simAgent.GetHistory().First;
        int index = 0;
        while (index < Math.Min(simAgent.GetHistory().Count - 1, boundedRandom))
        {
            d = d.Next;
            index++;
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
