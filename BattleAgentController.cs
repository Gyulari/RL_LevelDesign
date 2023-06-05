using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using UnityEngine.UIElements;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Actuators;
using JetBrains.Annotations;
using Unity.MLAgents.Sensors;
using UnityEngine.Events;

// Team ID
public enum BattleTeam
{
    Blue = 0,
    Red = 1
}

public class BattleAgentController : Agent
{
    // Destroy Event Handler
    public delegate void OnDestroyedEventHandler(GameObject destroyedObjcet);
    public static event OnDestroyedEventHandler OnDestroyed;

    // Team
    [HideInInspector] public BattleTeam team;

    // Existential Reward
    float m_Existential;

    // NotDestroy Signal for Reset
    public bool isNotKilled = true;

    // Fire Delay time & Fire hold time
    [SerializeField] float fireDelay = 1.5f;
    [SerializeField] float fireHold = 0.5f;
    bool canFire = true;
    bool isFiring = false;

    // Agent HP
    private const float maxHP = 100.0f;
    [SerializeField] private float curHP = 0.0f;
    public float CurrentHP
    {
        get
        {
            return curHP;
        }
        set
        {
            if(value <= 0.0f) {
                isNotKilled = false;
                Destroy(gameObject);
            }
            else {
                curHP = value;
            }
        }
    }

    // Agent Component
    [HideInInspector] public Rigidbody agentRb;

    // Script Component
    BattleStageSettings _BattleStageSettings;
    BehaviorParameters _BehaviorParameters;
    FireController _FireController;
    BattleStageEnvController _BattleStageEnvController;

    public override void Initialize()
    {
        isNotKilled = true;

        // Existential Reward Initialization
        _BattleStageEnvController = FindObjectOfType<BattleStageEnvController>();

        if (_BattleStageEnvController != null)
            m_Existential = 1f / _BattleStageEnvController.MaxEnvironmentSteps;
        else
            m_Existential = 1f / MaxStep;

        // Behavior Parameters Script Initialization
        _BehaviorParameters = gameObject.GetComponent<BehaviorParameters>();

        // Fire Controller Script Initialization
        _FireController = transform.GetChild(0).gameObject.GetComponent<FireController>();

        // Team Classification
        if (_BehaviorParameters.TeamId == (int)BattleTeam.Blue)
            team = BattleTeam.Blue;
        else
            team = BattleTeam.Red;

        // Stage Settings Script Initialization
        _BattleStageSettings = FindObjectOfType<BattleStageSettings>();

        // Rigidbody Component Initialization
        agentRb = GetComponent<Rigidbody>();

        // HP Initialization
        curHP = maxHP;
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        var h = Mathf.Clamp(actionBuffers.ContinuousActions[0], -1f, 1f);
        var v = Mathf.Clamp(actionBuffers.ContinuousActions[1], -1f, 1f);
        var fire = actionBuffers.DiscreteActions[0];

        if (!isFiring) {
            Vector3 dir = (Vector3.forward * v) + (Vector3.right * h);
            agentRb.AddForce(dir.normalized * _BattleStageSettings.agentRunSpeed, ForceMode.VelocityChange);

            if (canFire && fire == 1) {
                _FireController.Fire();
                StartCoroutine(FireDelayCoroutine());
                StartCoroutine(FireStayCoroutine());
            }
        }
        AddReward(-m_Existential);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        var discreteActionsOut = actionsOut.DiscreteActions;

        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");

        if (Input.GetKey(KeyCode.Space)) {
            discreteActionsOut[0] = 1;
        }
    }

    private IEnumerator FireDelayCoroutine()
    {
        canFire = false;
        yield return new WaitForSeconds(fireDelay);
        canFire = true;
    }

    private IEnumerator FireStayCoroutine()
    {
        isFiring = true;
        yield return new WaitForSeconds(fireHold);
        isFiring = false;
    }

    private void OnDestroy()
    {
        OnDestroyed?.Invoke(gameObject);
    }
}