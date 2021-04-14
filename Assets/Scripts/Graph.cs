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
    private float EDGE_THRESHOLD = 0.7f;
    [SerializeField] private Sprite circleSprite;
    private RectTransform graphContainer;

    private List<GameObject> verts = new List<GameObject>();

    private List<GameObject> edges = new List<GameObject>();

    private void Start()
    {
        graphContainer = transform.Find("graphContainer").GetComponent<RectTransform>();

        float r = 0.4;
        for (int i = 0; i < Settings.NumberAgents; i++)
        {
            float theta = Mathf.Deg2Rad * i * 360.0f / Settings.NumberAgents;
            Vector2 pos = new Vector2(r * Mathf.cos(theta), r * Mathf.Sin(theta));
            verts.Add(CreateCircle(pos)); // Add the 


        }
        // Draw all the edges
    }


    private GameObject CreateCircle(Vector2 anchoredPosition)
    {
        GameObject gameObject = new GameObject("circle", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().sprite = circleSprite;
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

    private void ShowGraph(List<Vector2> vertices, List<Edge> edges)
    {
        float graphHeight = graphContainer.sizeDelta.y;
        float graphWidth = graphContainer.sizeDelta.x;
        float yMax = 1f;
        float xMax = 1f;
        for (int i = 0; i < vertices.Count; i++)
        {
            float xPos = (vertices[i].x / xMax) * graphWidth;                            
            float yPos = (vertices[i].y / yMax) * graphHeight;
            CreateCircle(new Vector2(xPos, yPos)); // make sure to return GameObject so edges can be drawn
        }


    }
    private void CreateEdge(Vector2 dotA, Vector2 dotB)
    {
        GameObject gameObject = new GameObject("edge", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f); // make the bars slightly transparent
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        Vector2 dir = (dotB - dotA).normalized;
        float distance = Vector2.Distance(dotA,dotB);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.sizeDelta = new Vector2(distance, 3f);
        rectTransform.anchoredPosition = dotA + dir * distance  * 0.5f;
        rectTransform.localEulerAngles = new Vector3(0, 0, GetAngleFromVectorFloat(dir));
    }



    private void GenerateSimilarityGraph()
    {

        Matrix<float> similarity = GraphSimulation.Instance.recommender.similarity;

        if (similarity == null)
        {
            Debug.Log("Similarity matrix is null.");
            return;
        }

        // create graph representation of similarity matrix
        List<Vector2> vertices = new List<Vector2>();
        List<Edge> graph = new List<Edge>();

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
                    graph.Add(new Edge(i, j, sim));
                }

            }
        }



    }


    // Update is called once per frame
    void Update()
    {
      
        //GenerateDirectedGraph(Recommender.Instance.similarity);

    }

    public static float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        return n;
    }

}
