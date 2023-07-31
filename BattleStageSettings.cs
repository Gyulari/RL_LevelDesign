using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.MLAgents;
using UnityEngine;

public class BattleStageSettings : MonoBehaviour
{
    public GameObject battleStageEnv;
    BattleStageEnvController _BattleStageEnvController;

    public Material blueMaterial;
    public Material redMaterail;
    public bool randomizePlayersTeamForTraining = false;
    public float agentRunSpeed;

    public static bool allAgentsDestroyed = false;

    public int totalAgentCount = 10;
    public static int destroyedBlue = 0;
    public static int destroyedRed = 0;

    public TMP_Text dB;
    public TMP_Text dR;

    private void OnEnable()
    {
        ResetKillCount();
        _BattleStageEnvController = battleStageEnv.GetComponent<BattleStageEnvController>();
    }

    private void OnDisable()
    {
        
    }

    private void Update()
    {
        dB.text = destroyedBlue.ToString();
        dR.text = destroyedRed.ToString();
    }

    public void AllAgentsDestroyed(BattleTeam team)
    {
        if(team == BattleTeam.Blue) {
            _BattleStageEnvController.EliminateEnemy(0);
        }
        else if(team == BattleTeam.Red) {
            _BattleStageEnvController.EliminateEnemy(1);
        }
    }

    public void ResetKillCount()
    {
        destroyedBlue = 0;
        destroyedRed = 0;
    }
}
