using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using UnityEditor.UIElements;
using Unity.VisualScripting;
using Unity.MLAgents.Policies;
using static UnityEditor.Progress;
using System.Linq;

public class BattleStageEnvController : MonoBehaviour
{
    public delegate void OnClearFireRangeEventHandler();
    public static event OnClearFireRangeEventHandler OnClearFireRange;

    [System.Serializable]
    public class BattleAgentInfo
    {
        public BattleAgentController Agent;
        [HideInInspector] public Rigidbody Rb;
    }

    public int MaxEnvironmentSteps = 10000;

    public List<BattleAgentInfo> agentsList = new();

    public GameObject battleStage;

    private BattleStageSettings _BattleStageSettings;
    private WinRateSettings _WinRateSettings;
    private LogManager _LogManager;

    public SimpleMultiAgentGroup m_BlueAgentGroup;
    public SimpleMultiAgentGroup m_RedAgentGroup;
    public static float blueTotalHP;
    public static float redTotalHP;

    public int m_ResetTimer;

    StatsRecorder statsRecorder;

    private void Start()
    {
        statsRecorder = Academy.Instance.StatsRecorder;

        // Initialize stage settings
        _BattleStageSettings = battleStage.GetComponent<BattleStageSettings>();
        _WinRateSettings = FindObjectOfType<WinRateSettings>();
        _LogManager = FindObjectOfType<LogManager>();

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
    }

    private void FixedUpdate()
    {
        m_ResetTimer += 1;

        if(m_ResetTimer >= MaxEnvironmentSteps && MaxEnvironmentSteps > 0) {
            m_BlueAgentGroup.AddGroupReward(-10.0f);
            m_RedAgentGroup.AddGroupReward(-10.0f);
            // RewardGraphManager.m_DrawReward += -2.0f;
            // RewardGraphManager.sig_reachMax = true;
            // RewardGraphManager.sig_RewardRecord = true;
            // _LogManager.RecordEpisodeLog(0, 6 - BattleStageSettings.destroyedBlue, 6 - BattleStageSettings.destroyedRed);
            _BattleStageSettings.ResetKillCount();
            OnClearFireRangeList();
            m_BlueAgentGroup.GroupEpisodeInterrupted();
            m_RedAgentGroup.GroupEpisodeInterrupted();
            ResetScene();
        }
    }

    public void EliminateEnemy(int eliminatedTeam)
    {
        if (eliminatedTeam == (int)Team.Blue) {
            m_RedAgentGroup.AddGroupReward(1 - (float)m_ResetTimer / MaxEnvironmentSteps);
            m_BlueAgentGroup.AddGroupReward(-1);
            // RewardGraphManager.m_RedGroupReward += 1 - (float)m_ResetTimer / MaxEnvironmentSteps;
            // RewardGraphManager.m_BlueGroupReward += -1;
            // _LogManager.RecordEpisodeLog(-1, 6 - BattleStageSettings.destroyedBlue, 6 - BattleStageSettings.destroyedRed);
            _WinRateSettings.redWin++;
        }
        else {
            m_BlueAgentGroup.AddGroupReward(1 - (float)m_ResetTimer / MaxEnvironmentSteps);
            m_RedAgentGroup.AddGroupReward(-1);
            // RewardGraphManager.m_BlueGroupReward += 1 - (float)m_ResetTimer / MaxEnvironmentSteps;
            // RewardGraphManager.m_RedGroupReward += -1;
            // _LogManager.RecordEpisodeLog(1, 6 - BattleStageSettings.destroyedBlue, 6 - BattleStageSettings.destroyedRed);
            _WinRateSettings.blueWin++;
        }

        statsRecorder.Add("WinRate(Blue)", (float)_WinRateSettings.blueWin / (_WinRateSettings.blueWin + _WinRateSettings.redWin) * 100);
        
        RewardGraphManager.sig_RewardRecord = true;

        _BattleStageSettings.ResetKillCount();
        OnClearFireRangeList();

        m_BlueAgentGroup.EndGroupEpisode();
        m_RedAgentGroup.EndGroupEpisode();
        ResetScene();
    }

    public void KillAgent(BattleTeam killedAgentTeam)
    {
        if(killedAgentTeam == BattleTeam.Blue) {
            m_RedAgentGroup.AddGroupReward(1.0f / 6.0f);
            // RewardGraphManager.m_RedGroupReward += 1.0f / 6.0f;
        }
        else if (killedAgentTeam == BattleTeam.Red) {
            m_BlueAgentGroup.AddGroupReward(1.0f / 6.0f);
            // RewardGraphManager.m_BlueGroupReward += 1.0f / 6.0f;
        }
    }

    public void ResetScene()
    {
        m_ResetTimer = 0;

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
            agentsList[i].Agent.CurrentHP = 100.0f;
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

        foreach(var item in m_BlueAgentGroup.GetRegisteredAgents()) {
            Debug.Log(item);
        }
        foreach (var item in m_RedAgentGroup.GetRegisteredAgents()) {
            Debug.Log(item);
        }
        Debug.Log("EPISODE END");
    }

    private void OnClearFireRangeList()
    {
        OnClearFireRange?.Invoke();
    }
}
