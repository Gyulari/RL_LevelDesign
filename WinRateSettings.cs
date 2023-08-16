using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;
using System;

public class WinRateSettings : MonoBehaviour
{
    public TMP_Text blueWinRate;
    public TMP_Text redWinRate;
    public Image blueWinBar;
    public Image redWinBar;
    
    public static int blueWin = 0;
    public static int redWin = 0;
    public static int draw = 0;

    void Update()
    {
        int epiCount = blueWin + redWin + draw;

        if(epiCount != 0) {
            blueWinRate.text = ((float)blueWin / epiCount * 100).ToString("F2") + "%";
            redWinRate.text = ((float)redWin / epiCount * 100).ToString("F2") + "%";

            blueWinBar.GetComponent<Image>().fillAmount = (float)blueWin / epiCount;
            redWinBar.GetComponent<Image>().fillAmount = (float)redWin / epiCount;
        }
    }
}
