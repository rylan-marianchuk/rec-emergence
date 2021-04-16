using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Statistics;
using MathNet.Numerics;
public class Edge
{
    public int Start { get; set; }
    public int End { get; set; }
    public float Value { get; set; }

    public Edge (int s, int e, float v)
    {
        Start = s;
        End = e;
        Value = v;
    }
}

public class Graph : MonoBehaviour
{
    public float EDGE_THRESHOLD = 0.9f;

    private RectTransform graphContainer;

    private List<GameObject> verts = new List<GameObject>();

    private List<GameObject> edges = new List<GameObject>();

    private List<Color> colors = new List<Color>();

    private void Start()
    {

        for (int i =0; i < 10000; i++)
        {
            colors.Add(new Color(Random.value, Random.value, Random.value));
        }

        graphContainer = transform.Find("graphContainer").GetComponent<RectTransform>();

        List<Vector2> LocalVerts = new List<Vector2>();

        float r = 0.4f;
        for (int i = 0; i < Settings.NumberAgents; i++)
        {
            float theta = Mathf.Deg2Rad * i * 360.0f / Settings.NumberAgents;
            Vector2 pos = new Vector2(0.5f + r * Mathf.Cos(theta), 0.5f + r * Mathf.Sin(theta));
            LocalVerts.Add(pos);
        }
        ShowGraph(LocalVerts);

    }


    private GameObject CreateCircle(Vector2 anchoredPosition)
    {
        GameObject gameObject = new GameObject("circle", typeof(Image));
        gameObject.GetComponent<Image>().color = new Color(0, 0, 0, 1.0f);
        gameObject.transform.SetParent(graphContainer, false);
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(11, 11);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);

        return gameObject;
    }


    private void ShowGraph(List<float> valueList)
    {
        float graphHeight = graphContainer.sizeDelta.y;
        float graphWidth = graphContainer.sizeDelta.x;
        float yMax = 1f;
        float xMax = 1f;
        float xSize = graphWidth / valueList.Count;
        for (int i = 0; i < valueList.Count; i++)
        {
            float xPos = i * xSize;
            float yPos = (valueList[i] / yMax) * graphHeight;
            CreateCircle(new Vector2(xPos, yPos)); // make sure to return GameObject so edges can be drawn

        }


    }

    private void ShowGraph(List<Vector2> vertices)
    {
        float graphHeight = graphContainer.sizeDelta.y;
        float graphWidth = graphContainer.sizeDelta.x;
        float yMax = 1f;
        float xMax = 1f;
        for (int i = 0; i < vertices.Count; i++)
        {
            float xPos = (vertices[i].x / xMax) * graphWidth;                            
            float yPos = (vertices[i].y / yMax) * graphHeight;
            verts.Add(CreateCircle(new Vector2(xPos, yPos))); // make sure to return GameObject so edges can be drawn
        }


    }
    private GameObject CreateEdge(Vector2 dotA, Vector2 dotB, Color c)
    {
        GameObject gameObject = new GameObject("edge", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().color = c; // make the bars slightly transparent
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        Vector2 dir = (dotB - dotA).normalized;
        float distance = Vector2.Distance(dotA,dotB);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.sizeDelta = new Vector2(distance, 3f);
        rectTransform.anchoredPosition = dotA + dir * distance  * 0.5f;
        rectTransform.localEulerAngles = new Vector3(0, 0, GetAngleFromVectorFloat(dir));
        return gameObject;
    }

    public void DrawEdge(Edge e, Color c)
    {
        GameObject v1 = verts[e.Start];
        GameObject v2 = verts[e.End];

        Vector2 pos1 = v1.GetComponent<RectTransform>().anchoredPosition;
        Vector2 pos2 = v2.GetComponent<RectTransform>().anchoredPosition;

        edges.Add(CreateEdge(pos1, pos2, c));

    }

    private void DrawSimilarityGraph()
    {

        Matrix<float> similarity = GraphSimulation.Instance.recommender.calculateSimilarityMatrix();

        if (similarity == null)
        {
            Debug.Log("Similarity matrix is null.");
            return;
        }
        Debug.Log(similarity);
        // create graph representation of similarity matrix
        List<Vector2> vertices = new List<Vector2>();
        List<Edge> localEdges = new List<Edge>();

        List<List<int>> adjacency = new List<List<int>>();


        for (int i = 0; i < similarity.RowCount; i++)
        {
            adjacency.Add(new List<int>());
        }

        // Gather edges from similarity matrix
        for (int i = 0; i < similarity.RowCount; i++)
        {
            for (int j = i+1; j < similarity.ColumnCount; j++)
            {
                float sim = similarity.At(i, j);
                if (sim > EDGE_THRESHOLD)
                {
                    // We want agents who are similar to have less distance
                    // so invert the similarity to get distance
                    localEdges.Add(new Edge(i, j, sim));
                    adjacency[i].Add(j);
                }

            }
        }


        List<List<int>> clusters = FindClusters(adjacency);


        ClearEdges();
        foreach (Edge e in localEdges)
        {
            Color col = new Color(0,0,0);
            for (int i = 0; i < clusters.Count; i++)
            {
                
                if (clusters[i].Contains(e.Start))
                {
                    Debug.Log(i);
                    col = colors[i];
                }
            }
            DrawEdge(e, col);
        }

    }

    
    public void ClearEdges()
    {
        foreach (var item in edges)
        {
            GameObject.Destroy(item);
        }
    }

    // Update is called once per frame
    void Update()
    {

        DrawSimilarityGraph();

    }

    public static float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        return n;
    }


    public static List<List<int>> FindClusters(List<List<int>> adj)
    {
        // Returns a list of edges in a cycle (cycles identify clusters) 

        List<List<int>> output = new List<List<int>>();

        List<bool> visited = new List<bool>();

        for (int i = 0; i < adj.Count; i++)
            visited.Add(false);

        for (int i = 0; i < adj.Count; i++)
        {
            List<int> cluster = BreadthFirstSearch(adj, i, visited);
            if (cluster.Count > 2)
                output.Add(cluster);
        }

        

        return output;


    }

    static List<int> BreadthFirstSearch(List<List<int>> adj, int v, List<bool> visited)
    {
        List<int> chain = new List<int>();
        Queue<int> queue = new Queue<int>();

        visited[v] = true;

        queue.Enqueue(v);

        while (!(queue.Count == 0))
        {
            v = queue.Dequeue();
            chain.Add(v);
            foreach (var neighbour in adj[v])
            {
                if (!visited[neighbour])
                {
                    visited[neighbour] = true;
                    queue.Enqueue(neighbour);
                }
            }

        }
        return chain;
    }


}
