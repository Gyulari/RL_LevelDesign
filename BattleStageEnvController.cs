using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using UnityEditor.UIElements;
using Unity.VisualScripting;
using Unity.MLAgents.Policies;

public class BattleStageEnvController : MonoBehaviour
{
    // Reset Event Handler
    public delegate void OnResetEventHandler(Vector3 resetPos, bool isReset);
    public static event OnResetEventHandler OnReset;

    [System.Serializable]
    public class BattleAgentInfo
    {
        public BattleAgentController Agent;
        [HideInInspector] public Rigidbody Rb;
    }

    public int MaxEnvironmentSteps = 25000;

    public List<BattleAgentInfo> agentsList = new();

    public GameObject battleStage;

    private BattleStageSettings _BattleStageSettings;
    private WinRateSettings _WinRateSettings;

    public SimpleMultiAgentGroup m_BlueAgentGroup;
    public SimpleMultiAgentGroup m_RedAgentGroup;
    public static float blueTotalHP;
    public static float redTotalHP;

    private int m_ResetTimer;
    private Vector3 m_ResetPos;

    public bool reset = false;

    StatsRecorder statsRecorder;

    private void Start()
    {
        statsRecorder = Academy.Instance.StatsRecorder;

        // Initialize stage settings
        _BattleStageSettings = battleStage.GetComponent<BattleStageSettings>();
        _WinRateSettings = FindObjectOfType<WinRateSettings>();

        // Initialize TeamManager
        m_BlueAgentGroup = new SimpleMultiAgentGroup();
        m_RedAgentGroup = new SimpleMultiAgentGroup();

        foreach (var item in agentsList) {
            item.Rb = item.Agent.GetComponent<Rigidbody>();

            if (item.Agent.team == BattleTeam.Blue)
                m_BlueAgentGroup.RegisterAgent(item.Agent);
            else
                m_RedAgentGroup.RegisterAgent(item.Agent);
        }

        m_ResetPos = transform.position;
    }

    private void FixedUpdate()
    {
        m_ResetTimer += 1;
        if(m_ResetTimer >= MaxEnvironmentSteps && MaxEnvironmentSteps > 0) {
            RewardGraphManager.sig_RewardRecord = true;
            m_BlueAgentGroup.GroupEpisodeInterrupted();
            m_RedAgentGroup.GroupEpisodeInterrupted();
            ResetScene();
        }
        /*
        foreach(var agent in agentsList) {
            if(agent.Agent != null) {
                if (agent.Agent.team == BattleTeam.Blue) {
                    // Debug.Log("Blue : " + agent.Agent.GetCumulativeReward());
                }
                if (agent.Agent.team == BattleTeam.Red) {
                    // Debug.Log("Red : " + agent.Agent.GetCumulativeReward());
                }
            }
        }
        */
    }

    public void EliminateEnemy(int eliminatedTeam)
    {
        if(eliminatedTeam == (int)Team.Blue) {
            m_RedAgentGroup.AddGroupReward(1 - (float)m_ResetTimer / MaxEnvironmentSteps);
            m_BlueAgentGroup.AddGroupReward(-1);
            RewardGraphManager.m_RedGroupReward = 1 - (float)m_ResetTimer / MaxEnvironmentSteps;
            RewardGraphManager.m_BlueGroupReward = -1;
            _WinRateSettings.redWin++;
        }
        else {
            m_BlueAgentGroup.AddGroupReward(1 - (float)m_ResetTimer / MaxEnvironmentSteps);
            m_RedAgentGroup.AddGroupReward(-1);
            RewardGraphManager.m_BlueGroupReward = 1 - (float)m_ResetTimer / MaxEnvironmentSteps;
            RewardGraphManager.m_RedGroupReward = -1;
            _WinRateSettings.blueWin++;
        }

        statsRecorder.Add("WinRate(Blue)", (float)_WinRateSettings.blueWin / (_WinRateSettings.blueWin + _WinRateSettings.redWin) * 100);
        
        RewardGraphManager.sig_RewardRecord = true;
        
        m_BlueAgentGroup.EndGroupEpisode();
        m_RedAgentGroup.EndGroupEpisode();
        ResetScene();
    }

    public void KillEnemyAgent(int elimintatedAgent)
    {
        if(elimintatedAgent == (int)Team.Blue) {
            m_RedAgentGroup.AddGroupReward(1.0f);
        }
        else {
            m_BlueAgentGroup.AddGroupReward(1.0f);
        }
    }

    public void ResetScene()
    {
        m_ResetTimer = 0;

        reset = true;
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        OnReset?.Invoke(m_ResetPos, reset);
    }
}
