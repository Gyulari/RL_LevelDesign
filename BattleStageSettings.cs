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
    public int destroyedBlue = 0;
    public int destroyedRed = 0;

    private void OnEnable()
    {
        BattleAgentController.OnDestroyed += KillAgent;
        _BattleStageEnvController = battleStageEnv.GetComponent<BattleStageEnvController>();
        // _BattleStageEnvController = FindObjectOfType<BattleStageEnvController>();
    }

    private void Update()
    {
        if(totalAgentCount == destroyedBlue) {
            _BattleStageEnvController.EliminateEnemy(0);
            ResetKillCount();
        }
        else if(totalAgentCount == destroyedRed) {
            _BattleStageEnvController.EliminateEnemy(1);
            ResetKillCount();
        }
    }

    private void KillAgent(GameObject killedAgent)
    {
        if (killedAgent.GetComponent<BattleAgentController>().isNotKilled)
            return;

        if (killedAgent.CompareTag("Blue")) {
            destroyedBlue++;
        }
        else {
            destroyedRed++;
        }
    }

    private void ResetKillCount()
    {
        destroyedBlue = 0;
        destroyedRed = 0;
    }
}
