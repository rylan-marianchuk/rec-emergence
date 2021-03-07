using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Helper for state
/// </summary>
public class ABPair
{
    public float alpha { get; set; }
    public float beta { get; set; }

    public ABPair (float alpha, float beta)
    {
        this.alpha = alpha;
        this.beta = beta;
    }

}

/**
 * 
 * Each agent holds a state
 * 
 */
public class State
{
    /// <summary>
    /// Represents the alpha and beta values for each category
    /// </summary>
    public List<ABPair> categoriesAB { get; set; }

    public List<Document> documents { get; set; }
    private float alpha;
    private float beta;

    public State(float _alpha, float _beta, List<Document> _documents)
    {
        this.alpha = _alpha;
        this.beta = _beta;
        this.documents = new List<Document>(_documents);
    }

    public State(float _alpha, float _beta)
    {
        this.alpha = _alpha;
        this.beta = _beta;
        this.documents = new List<Document>();
    }


    public float getAlpha() { return alpha; }
    public float getBeta() { return beta; }
    public List<Document> getDocuments() { return this.documents; }

    public void setAlpha(float v) { this.alpha = v; }
    public void setBeta(float v) { this.beta = v; }
    public void setDocuments(List<Document> v) 
    {
        this.documents = new List<Document>();
        foreach (var doc in v)
        {
            this.documents.Add(doc);
        }
    }

    public void addDocument(Document d) { documents.Add(d); }



}
