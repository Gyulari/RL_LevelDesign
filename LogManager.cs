using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LogManager : MonoBehaviour
{
    public TMP_Text log1;
    public TMP_Text log2;
    public TMP_Text log3;

    private List<string> results = new List<string>();

    public void RecordEpisodeLog(int epiLog)
    {
        int scoreB = BattleStageSettings.retiredRed;
        int scoreR = BattleStageSettings.retiredBlue;

        string log = " B : " + scoreB + " / " + "R : " + scoreR;

        switch (epiLog){
            case -1:
                log = "[Red Win]" + log;
                break;
            case 0:
                log = "[Draw]" + log;
                break;
            case 1:
                log = "[Blue Win]" + log;
                break;
        }

        switch (results.Count) {
            case 0:
                results.Add(log);
                log1.text = log;
                break;
            case 1:
                results.Add(log);
                log2.text = log;
                break;
            case 2:
                results.Add(log);
                log3.text = log;
                break;
            case 3:
                results.RemoveAt(0);
                results.Add(log);
                log1.text = results[0];
                log2.text = results[1];
                log3.text = results[2];
                break;
        }
    }
}
