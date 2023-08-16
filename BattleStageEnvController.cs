using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using UnityEditor.UIElements;
using Unity.VisualScripting;
using Unity.MLAgents.Policies;
using static UnityEditor.Progress;
using System.Linq;
using System;
using UnityEditor.Build;

public class BattleStageEnvController : MonoBehaviour
{
    // Clear List of Enemy Agents in Fire Range Evnet
    public delegate void ClearFireRangeEventHandler();
    public static event ClearFireRangeEventHandler ClearFireRange;

    [Serializable]
    public class BattleAgentInfo
    {
        public BattleAgentController Agent;
        [HideInInspector] public Rigidbody Rb;
    }

    // Maximum number of frames allowed within one episode
    public int MaxEnvironmentFrames = 20000;
    public int m_ResetTimer;

    // Attenuation Factor that reduces reward by the frames used during the episode   
    float AF;

    public float drawReward;

    // List of agents in environment
    public List<BattleAgentInfo> agentsList = new();

    // Agent group for each team
    public SimpleMultiAgentGroup m_BlueAgentGroup;
    public SimpleMultiAgentGroup m_RedAgentGroup;

    StatsRecorder statsRecorder;

    // Script Component
    BattleStageSettings _BattleStageSettings;
    WinRateSettings _WinRateSettings;
    LogManager _LogManager;

    private void Awake()
    {
        statsRecorder = Academy.Instance.StatsRecorder;

        // Script Component Initialization
        _BattleStageSettings = FindObjectOfType<BattleStageSettings>();
        _WinRateSettings = FindObjectOfType<WinRateSettings>();
        _LogManager = FindObjectOfType<LogManager>();

        m_BlueAgentGroup = new SimpleMultiAgentGroup();
        m_RedAgentGroup = new SimpleMultiAgentGroup();

        // Register agent information
        foreach (var item in agentsList) {
            item.Rb = item.Agent.GetComponent<Rigidbody>();

            if (item.Agent.team == BattleTeam.Blue)
                m_BlueAgentGroup.RegisterAgent(item.Agent);
            else if (item.Agent.team == BattleTeam.Red)
                m_RedAgentGroup.RegisterAgent(item.Agent);
        }
    }

    private void FixedUpdate()
    {
        m_ResetTimer += 1;
        AF = (float)m_ResetTimer / MaxEnvironmentFrames;

        // If the maximum number of frames allowed has been reached (Draw)
        if (m_ResetTimer >= MaxEnvironmentFrames) {
            m_BlueAgentGroup.AddGroupReward(drawReward);
            m_RedAgentGroup.AddGroupReward(drawReward);
            SetRewardGraphInfo();
            _LogManager.RecordEpisodeLog(0);
            WinRateSettings.draw++;

            m_BlueAgentGroup.GroupEpisodeInterrupted();
            m_RedAgentGroup.GroupEpisodeInterrupted();
            ResetScene();
        }
    }

    // If all the agents in one team are retired
    // Add [+group reward] to win team, [-group reward] to defeat team
    public void EliminateEnemy(int eliminatedTeam)
    {
        if (eliminatedTeam == (int)Team.Blue) {
            m_RedAgentGroup.AddGroupReward(1 - AF);
            m_BlueAgentGroup.AddGroupReward(-1);
            SetRewardGraphInfo(1 - AF, -1, BattleTeam.Red, true);
            _LogManager.RecordEpisodeLog(-1);
            WinRateSettings.redWin++;
        }
        else {
            m_BlueAgentGroup.AddGroupReward(1 - AF);
            m_RedAgentGroup.AddGroupReward(-1);
            SetRewardGraphInfo(1 - AF, -1, BattleTeam.Blue, true);
            _LogManager.RecordEpisodeLog(1);
            WinRateSettings.blueWin++;
        }

        statsRecorder.Add("WinRate(Blue)", (float)WinRateSettings.blueWin / (WinRateSettings.blueWin + WinRateSettings.redWin + WinRateSettings.draw) * 100);
        statsRecorder.Add("WinRate(Red)", (float)WinRateSettings.redWin / (WinRateSettings.blueWin + WinRateSettings.redWin + WinRateSettings.draw) * 100);
        statsRecorder.Add("DrawRate", (float)WinRateSettings.draw / (WinRateSettings.blueWin + WinRateSettings.redWin + WinRateSettings.draw) * 100);

        m_BlueAgentGroup.EndGroupEpisode();
        m_RedAgentGroup.EndGroupEpisode();
        ResetScene();
    }

    // If agent kills enemy agent
    // Add [+group reward] to agent which killed enemy agent
    public void KillAgent(BattleTeam killedAgentTeam)
    {
        if(killedAgentTeam == BattleTeam.Blue) {
            m_RedAgentGroup.AddGroupReward(1.0f);
            SetRewardGraphInfo(1.0f, 0.0f, BattleTeam.Red, false);
        }
        else if (killedAgentTeam == BattleTeam.Red) {
            m_BlueAgentGroup.AddGroupReward(1.0f);
            SetRewardGraphInfo(1.0f, 0.0f, BattleTeam.Blue, false);
        }
    }

    // Reset scene(Battle Stage)
    // Reset timer, kill count, fire range list, position of agents, state of agents
    // Re-register agents which was inactive to agent group
    public void ResetScene()
    {
        m_ResetTimer = 0;

        _BattleStageSettings.ResetKillCount();
        ResetFireRange();

        List<List<Vector3>> posList = new();

        List<Vector3> bluePos = new List<Vector3> {new Vector3(-10, 1, -10),
                                                    new Vector3(-10, 1, -7.5f),
                                                    new Vector3(-7.5f, 1, -10),
                                                    new Vector3(-10, 1, -5),
                                                    new Vector3(-5, 1, -10),
                                                    new Vector3(-7.5f, 1, -7.5f)};
        List<Vector3> redPos = new List<Vector3> {new Vector3(10, 1, 10),
                                                    new Vector3(10, 1, 7.5f),
                                                    new Vector3(7.5f, 1, 10),
                                                    new Vector3(10, 1, 5),
                                                    new Vector3(5, 1, 10),
                                                    new Vector3(7.5f, 1, 7.5f)};

        posList.Add(bluePos);
        posList.Add(redPos);

        for(int i=0; i < agentsList.Count; i++) {
            agentsList[i].Agent.gameObject.SetActive(true);
            if(i % 2 == 0) {
                agentsList[i].Agent.gameObject.transform.position = posList[0][i / 2];
            }
            else {
                agentsList[i].Agent.gameObject.transform.position = posList[1][i / 2];
            }
            agentsList[i].Agent.HP = 100.0f;
        }

        foreach (var item in agentsList) {
            if(item.Agent.team == BattleTeam.Blue) {
                if (m_BlueAgentGroup.GetRegisteredAgents().Contains(item.Agent)) {
                    continue;
                }
                m_BlueAgentGroup.RegisterAgent(item.Agent);
            }
            else {
                if (m_RedAgentGroup.GetRegisteredAgents().Contains(item.Agent)) {
                    continue;
                }
                m_RedAgentGroup.RegisterAgent(item.Agent);
            }
        }
    }

    // Clear List of Enemy Agents in Fire Range Evnet Function
    private void ResetFireRange()
    {
        ClearFireRange?.Invoke();
    }

    // Set reward graph data [Draw]
    private void SetRewardGraphInfo()
    {
        RewardGraphManager.m_BlueGroupReward += drawReward;
        RewardGraphManager.m_RedGroupReward += drawReward;

        RewardGraphManager.sig_Draw = true;
        RewardGraphManager.sig_RewardRecord = true;
    }

    // Set reward graph data [not Draw]
    // If record is true, dot the data in the reward graph
    private void SetRewardGraphInfo(float pReward, float nReward, BattleTeam pTeam, bool record)
    {
        RewardGraphManager.m_BlueGroupReward += (pTeam == BattleTeam.Blue) ? pReward : nReward;
        RewardGraphManager.m_RedGroupReward += (pTeam == BattleTeam.Red) ? pReward : nReward;
        
        RewardGraphManager.sig_RewardRecord = record;
    }
}
