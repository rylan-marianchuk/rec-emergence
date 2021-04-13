using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileWrite : MonoBehaviour
{
    /**
     * 
     * Write all data for each agent as end results of simulation
     * 
     * m = 200 iterations of simulation
     * n = number of agents
     */
    public static void WriteSummary(List<float[]> documentHistory, List<float[]> finalState)
    {
        //StreamWriter documentHistoryTXT = new StreamWriter(Application.dataPath + "document-history.txt");
        //StreamWriter finalStateTXT = new StreamWriter(Application.dataPath + "final-state.txt");



        /*
        for (int i = 0; i < documentHistory.Count; i++)
        {
            string line1 = string.Join(",", documentHistory[i]);
            string line2 = string.Join(",", finalState[i]);
            documentHistoryTXT.WriteLine(line1);
            finalStateTXT.WriteLine(line2);
        }
        documentHistoryTXT.Flush();
        finalStateTXT.Flush();
        */

        string documentHistoryTXT = "";
        string finalStateTXT = "";

        for (int i = 0; i < documentHistory.Count; i++)
        {
            documentHistoryTXT += string.Join(",", documentHistory[i]) + "\n";
            finalStateTXT += string.Join(",", finalState[i]) + "\n";
        }

        System.IO.File.WriteAllText(Application.dataPath + "/document-history.txt", documentHistoryTXT);
        System.IO.File.WriteAllText(Application.dataPath + "/final-state.txt", finalStateTXT);
        Debug.Log("Wrote to files successfully.");
    }
}
