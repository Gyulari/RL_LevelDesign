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
        //BattleAgentController.OnDestroyed += KillAgent;
        _BattleStageEnvController = battleStageEnv.GetComponent<BattleStageEnvController>();
    }

    private void OnDisable()
    {
        //BattleAgentController.OnDestroyed -= KillAgent;
    }

    private void Update()
    {
        dB.text = destroyedBlue.ToString();
        dR.text = destroyedRed.ToString();
        /*
        if (totalAgentCount == destroyedBlue) {
            ResetKillCount();
            _BattleStageEnvController.EliminateEnemy(0);
        }
        else if(totalAgentCount == destroyedRed) {
            ResetKillCount();
            _BattleStageEnvController.EliminateEnemy(1);
        }
        */
    }

    public void AllAgentsDestroyed(BattleTeam team)
    {
        ResetKillCount();

        if(team == BattleTeam.Blue) {
            _BattleStageEnvController.EliminateEnemy(0);
        }
        else if(team == BattleTeam.Red) {
            _BattleStageEnvController.EliminateEnemy(1);
        }
    }

    /*
    private void KillAgent(GameObject killedAgent)
    {
        if (killedAgent.GetComponent<BattleAgentController>().isNotKilled)
            return;

        if (killedAgent.CompareTag("Blue")) {
            // _BattleStageEnvController.KillEnemyAgent(0);
            destroyedBlue++;
        }
        else {
            // _BattleStageEnvController.KillEnemyAgent(1);
            destroyedRed++;
        }
    }
    */

    public void ResetKillCount()
    {
        destroyedBlue = 0;
        destroyedRed = 0;
    }
}
