using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentCategoryModel : MonoBehaviour
{

    public Vector3 goalPosition;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // need to make so that this does not update if alpha/beta for agent corresponding to this model reached 1
        transform.Translate(new Vector3(0.0f,0.0f,goalPosition.z) * Time.deltaTime * 0.05f);
        //transform.position = Vector3.Lerp(transform.position, goalPosition, Time.deltaTime * 0.05f);
    }
}
