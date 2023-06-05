using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using UnityEditor.UIElements;
using Unity.VisualScripting;

public class BattleStageEnvController : MonoBehaviour
{
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
    public List<Vector3> blueAgentInitPos = new();
    public List<Vector3> redAgentInitPos = new();

    public GameObject battleStage;
    private BattleStageSettings _BattleStageSettings;
    private WinRateSettings _WinRateSettings;

    private SimpleMultiAgentGroup m_BlueAgentGroup;
    private SimpleMultiAgentGroup m_RedAgentGroup;

    private int m_ResetTimer;
    private Vector3 m_ResetPos;

    public bool reset = false;

    private void Start()
    {
        // Initialize stage settings
        _BattleStageSettings = battleStage.GetComponent<BattleStageSettings>();
        // _BattleStageSettings = FindObjectOfType<BattleStageSettings>();
        _WinRateSettings = FindObjectOfType<WinRateSettings>();

        // Initialize TeamManager
        m_BlueAgentGroup = new SimpleMultiAgentGroup();
        m_RedAgentGroup = new SimpleMultiAgentGroup();

        // InitAgentInfo();

        foreach (var item in agentsList) {
            item.Rb = item.Agent.GetComponent<Rigidbody>();

            if (item.Agent.team == BattleTeam.Blue)
                m_BlueAgentGroup.RegisterAgent(item.Agent);
            else
                m_RedAgentGroup.RegisterAgent(item.Agent);
        }

        m_ResetPos = transform.position;
        // ResetScene();
    }

    private void FixedUpdate()
    {
        m_ResetTimer += 1;
        if(m_ResetTimer >= MaxEnvironmentSteps && MaxEnvironmentSteps > 0) {
            m_BlueAgentGroup.GroupEpisodeInterrupted();
            m_RedAgentGroup.GroupEpisodeInterrupted();
            ResetScene();
        }
    }

    public void EliminateEnemy(int eliminatedTeam)
    {
        if(eliminatedTeam == (int)Team.Blue) {
            m_BlueAgentGroup.AddGroupReward(-1);
            m_RedAgentGroup.AddGroupReward(1 - (float)m_ResetTimer / MaxEnvironmentSteps);
            _WinRateSettings.redWin++;
        }
        else {
            m_BlueAgentGroup.AddGroupReward(1 - (float)m_ResetTimer / MaxEnvironmentSteps);
            m_RedAgentGroup.AddGroupReward(-1);
            _WinRateSettings.blueWin++;
        }

        var statsRecorder = Academy.Instance.StatsRecorder;
        statsRecorder.Add("WinRate(Blue)", (float)_WinRateSettings.blueWin / (_WinRateSettings.blueWin + _WinRateSettings.redWin) * 100);

        m_BlueAgentGroup.EndGroupEpisode();
        m_RedAgentGroup.EndGroupEpisode();
        ResetScene();
    }


    public void ResetScene()
    {
        m_ResetTimer = 0;

        reset = true;
        Destroy(gameObject);

        /*
        agentsList.Clear();

        // InitAgentInfo();

        foreach(var item in agentsList) {
            item.Rb.velocity = Vector3.zero;
            item.Rb.angularVelocity = Vector3.zero;
        }
        */
    }
    
    private void InitAgentInfo()
    {
        for (int i = 0; i < _BattleStageSettings.totalAgentCount; i++) {
            BattleAgentInfo agentInfoB = new();
            GameObject agentB = Resources.Load<GameObject>("AgentPrefab/BlueAgent");
            agentInfoB.Agent = agentB.GetComponent<BattleAgentController>();
            agentInfoB.Rb = agentInfoB.Agent.GetComponent<Rigidbody>();
            agentsList.Add(agentInfoB);
            Instantiate(agentB, blueAgentInitPos[i], Quaternion.identity);

            BattleAgentInfo agentInfoR = new();
            GameObject agentR = Resources.Load<GameObject>("AgentPrefab/RedAgent");
            agentInfoR.Agent = agentR.GetComponent<BattleAgentController>();
            agentInfoR.Rb = agentInfoR.Agent.GetComponent<Rigidbody>();
            agentsList.Add(agentInfoR);
            Instantiate(agentR, redAgentInitPos[i], Quaternion.identity);
        }
    }

    private void OnDestroy()
    {
        OnReset?.Invoke(m_ResetPos, reset);
    }
}
