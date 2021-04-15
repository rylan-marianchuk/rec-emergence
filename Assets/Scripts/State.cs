using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
 * 
 * Each agent holds a state
 * 
 */
public class State
{
    private List<Document> documents;
    private float alpha;
    private float beta;

    // Extending to the 2 dimensional case
    private float alpha1;
    private float beta1;

    public State()
    {
        this.alpha = 1;
        this.alpha1 = 1;
        this.beta = 1;
        this.beta1 = 1;
        documents = new List<Document>();
    }

    public float getAlpha() { return alpha; }
    public float getBeta() { return beta; }
    public float getAlpha1() { return alpha1; }
    public float getBeta1() { return beta1; }
    public List<Document> getDocuments() { return this.documents; }

    public void setAlpha(float v) { this.alpha = v; }
    public void setBeta(float v) { this.beta = v; }
    public void setAlpha1(float v) { this.alpha1 = v; }
    public void setBeta1(float v) { this.beta1 = v; }
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
