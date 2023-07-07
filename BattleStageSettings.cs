using System.Collections;
using System.Collections.Generic;
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

    public int totalAgentCount = 10;
    public static int destroyedBlue = 0;
    public static int destroyedRed = 0;

    private void OnEnable()
    {
        ResetKillCount();
        BattleAgentController.OnDestroyed += KillAgent;
        _BattleStageEnvController = battleStageEnv.GetComponent<BattleStageEnvController>();
    }

    private void OnDisable()
    {
        BattleAgentController.OnDestroyed -= KillAgent;
    }

    private void Update()
    {
        if (totalAgentCount == destroyedBlue) {
            ResetKillCount();
            _BattleStageEnvController.EliminateEnemy(0);
        }
        else if(totalAgentCount == destroyedRed) {
            ResetKillCount();
            _BattleStageEnvController.EliminateEnemy(1);
        }
    }

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

    private void ResetKillCount()
    {
        destroyedBlue = 0;
        destroyedRed = 0;
    }
}
