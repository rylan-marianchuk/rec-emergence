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

    private void Start()
    {
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
    private GameObject CreateEdge(Vector2 dotA, Vector2 dotB, float opacity)
    {
        GameObject gameObject = new GameObject("edge", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().color = new Color(0.584f, 0, 0.639f, Mathf.Max(0.00f,opacity * 0.5f)); // make the bars slightly transparent
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

    public void DrawEdge(Edge e)
    {
        GameObject v1 = verts[e.Start];
        GameObject v2 = verts[e.End];

        Vector2 pos1 = v1.GetComponent<RectTransform>().anchoredPosition;
        Vector2 pos2 = v2.GetComponent<RectTransform>().anchoredPosition;

        edges.Add(CreateEdge(pos1, pos2, e.Value));

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
                }

            }
        }


        ClearEdges();
        foreach (Edge e in localEdges)
        {
            DrawEdge(e);
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

}
