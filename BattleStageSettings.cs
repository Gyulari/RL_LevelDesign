using UnityEngine;

public class BattleStageSettings : MonoBehaviour
{
    public float agentRunSpeed;

    // Count of agents
    public int totalAgentCount = 6;
    public static int retiredBlue = 0;
    public static int retiredRed = 0;

    // Script Component
    BattleStageEnvController _BattleStageEnvController;

    private void Awake()
    {
        // Script Component Initialization
        _BattleStageEnvController = FindObjectOfType<BattleStageEnvController>();
    }

    // Call the result handling function when all agents in one team retire
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
        retiredBlue = 0;
        retiredRed = 0;
    }
}
