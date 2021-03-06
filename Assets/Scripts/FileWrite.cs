using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileWrite
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    /**
     * 
     * Write all data for each agent as end results of simulation
     * 
     * m = 200 iterations of simulation
     * n = number of agents
     */
    public static void WriteSummary(List<float[]> documentHistory, List<float[]> finalState)
    {
        StreamWriter documentHistoryTXT = new StreamWriter("document-history.txt");
        StreamWriter finalStateTXT = new StreamWriter("final-state.txt");
        
        for (int i = 0; i < documentHistory.Count; i++)
        {
            string line1 = string.Join(",", documentHistory[i]);
            string line2 = string.Join(",", finalState[i]);
            documentHistoryTXT.WriteLine(line1);
            finalStateTXT.WriteLine(line2);
        }
        documentHistoryTXT.Flush();
        documentHistoryTXT.Close();
        finalStateTXT.Flush();
        finalStateTXT.Close();

        Debug.Log("Wrote to file successfully.");
    }
}
