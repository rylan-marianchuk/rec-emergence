using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    /// <summary>
    /// Number of agents.
    /// </summary>
    public int n;

    /// <summary>
    /// Documents per agent
    /// </summary>
    public int d;

    /// <summary>
    /// Time Cap
    /// </summary>
    public int T;


    /// <summary>
    /// Number of agents any given agents uses for its recommendation
    /// </summary>
    public int r;


    /// <summary>
    /// Recommender similarity metric: ('E') Euclidean Distance, ('P') Pearson
    /// </summary>
    public char R;

    /// <summary>
    /// Policy : ('F') Seeking Familiarity, ('D') Seeking Diversity
    /// </summary>
    public char P;

}
