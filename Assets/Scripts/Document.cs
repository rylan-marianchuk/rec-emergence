using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Document : MonoBehaviour
{

    #region deprecated (single values)
    //public float value;

    //public Document(float val) { this.value = val; }
    #endregion



    #region multi-dimensional implementation

    /// <summary>
    /// The 0-1 values of each category
    /// </summary>
    public List<float> values { get; set; } = new List<float>();

    /// <summary>
    /// Used to construct a list of 0-1 pairs
    /// </summary>
    /// <param name="val"></param>
    public Document(List<float> val)
    {
        values = val;
    }

    public Document()
    {

    }

    /// <summary>
    /// Multiplies the ratings by a scale factor to work with recommender?
    /// 
    /// - Alternatively, I think we should use the history of each user to calculate their preferences
    /// (see Agent)
    /// 
    /// </summary>
    public List<float> convertToRatings (int scaleFactor)
    {
        List<float> valuesConverted = new List<float>();

        foreach (var f in values) {
            valuesConverted.Add(f * 5);
        }

        return valuesConverted;
    }

    #endregion


}
