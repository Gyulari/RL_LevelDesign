using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MEFManager : MonoBehaviour
{
    public BattleStageEnvController _BattleStageEnvController;

    public Image framesBar;
    public TMP_Text frames;

    private void Update()
    {
        frames.text = _BattleStageEnvController.m_ResetTimer.ToString();
        framesBar.GetComponent<Image>().fillAmount = (float)_BattleStageEnvController.m_ResetTimer / _BattleStageEnvController.MaxEnvironmentFrames;
    }
}
