using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MESManager : MonoBehaviour
{
    public BattleStageEnvController _BattleStageEnvController;

    public Image stepsBar;
    public TMP_Text steps;

    private void Update()
    {
        steps.text = _BattleStageEnvController.m_ResetTimer.ToString();
        stepsBar.GetComponent<Image>().fillAmount = (float)_BattleStageEnvController.m_ResetTimer / 10000;
    }
}
