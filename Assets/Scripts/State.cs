using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
 * 
 * Each agent holds a state
 * 
 */
public class State : MonoBehaviour
{
    private List<Document> documents;
    private float alpha;
    private float beta;

    public float getAlpha() { return alpha; }
    public float getBeta() { return beta; }
    public List<Document> getDocuments() { return this.documents; }

    public void setAlpha(float v) { this.alpha = v; }
    public void setBeta(float v) { this.beta = v; }
    public void setDocuments(List<Document> v) { documents = new List<Document>(v); }
}
