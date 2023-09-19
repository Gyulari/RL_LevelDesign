using System.Collections;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Actuators;

// Team ID
public enum BattleTeam
{
    Blue = 0,
    Red = 1
}

public class BattleAgentController : Agent
{
    // Retire Event
    public delegate void OnRetiredEventHandler(GameObject retiredObj);
    public static event OnRetiredEventHandler OnRetired;

    // Send Information to Dashboard UI Event
    public delegate void SendInfoEventHandler(string name, int h, int v, int fire);
    public static event SendInfoEventHandler SendInfo;

    // Team
    [HideInInspector] public BattleTeam team;

    // Fire Delay time & Fire hold time
    [SerializeField] float fireDelay = 1.5f;
    [SerializeField] float fireHold = 0.5f;
    bool canFire = true;
    bool isFiring = false;

    // Agent HP
    private const float maxHP = 100.0f;
    private float curHP = 0.0f;
    
    public float HP
    {
        get
        {
            return curHP;
        }
        set
        {
            if (value <= 0.0f)
                HandleRetire(team);
            else
                curHP = value;
        }
    }

    // Object Component
    Rigidbody agentRb;
    GameObject shootRange;

    // Script Component
    BattleStageSettings _BattleStageSettings;
    BehaviorParameters _BehaviorParameters;
    FireController _FireController;
    BattleStageEnvController _BattleStageEnvController;

    public override void Initialize()
    {
        // Object Component Initialization
        agentRb = GetComponent<Rigidbody>();
        shootRange = transform.GetChild(0).gameObject;

        // Script Component Initialization
        _BattleStageSettings = FindObjectOfType<BattleStageSettings>();
        _BattleStageEnvController = FindObjectOfType<BattleStageEnvController>();
        _BehaviorParameters = gameObject.GetComponent<BehaviorParameters>();
        _FireController = shootRange.GetComponent<FireController>();

        // Team Classification
        if (_BehaviorParameters.TeamId == (int)BattleTeam.Blue)
            team = BattleTeam.Blue;
        else
            team = BattleTeam.Red;        

        curHP = maxHP;
    }

    // Specify agent behavior at every step, based on the provided action.
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Type of behavior [h : Horizontal movement value / v : Vertical movement value / fire : Whether to fire]
        var h = actionBuffers.DiscreteActions[0] - 1;
        var v = actionBuffers.DiscreteActions[1] - 1;
        var fire = actionBuffers.DiscreteActions[2];

        if(fire == 1) {
            if(isFiring || !canFire || _FireController.inRangeObjects.Count == 0)
                _BattleStageEnvController.EvaluateFireDecision(team, false);
            else if (!isFiring && canFire && _FireController.inRangeObjects.Count != 0)
                _BattleStageEnvController.EvaluateFireDecision(team, true);
        }

        // If agent is not on firing (to prevent firing while on moving)
        if (!isFiring) {
            Vector3 dir = (Vector3.forward * v) + (Vector3.right * h);
            agentRb.AddForce(dir.normalized * _BattleStageSettings.agentRunSpeed, ForceMode.VelocityChange);

            // If agent is not on fire delay
            // Fire Delay : Delay after firing until next firing is possible (to prevent uninterrupted continuous firing)
            if (canFire && fire == 1 && _FireController.inRangeObjects.Count != 0) {
                _FireController.Fire();
                StartCoroutine(FireDelayCoroutine());
                StartCoroutine(FireStayCoroutine());
            }
        }

        // Send information about behaviors taken by agents
        SendInfoToDashboard(gameObject.name, h, v, fire);
    }

    // Choose an action for the agent using a custom heuristic
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        var discreteActionsOut = actionsOut.DiscreteActions;

        // Actions for movement
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");

        // Actions for firing
        if (Input.GetKey(KeyCode.Space))
            discreteActionsOut[0] = 1;
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

    // Handle decision to retire agent
    private void HandleRetire(BattleTeam team)
    {
        // Clear list of enemy agents in fire range
        transform.GetChild(0).gameObject.GetComponent<FireController>().inRangeObjects.Clear();

        // If the retired agent is not the last agent of the team
        if (team == BattleTeam.Blue) {
            if (BattleStageSettings.retiredBlue++ < 5) {
                gameObject.SetActive(false);
                OnRetire(true);
                return;
            }
        }
        if (team == BattleTeam.Red) {
            if (BattleStageSettings.retiredRed++ < 5) {
                gameObject.SetActive(false);
                OnRetire(true);
                return;
            }
        }

        // If the retired agent is the last agent of the team
        OnRetire(false);
        _BattleStageSettings.AllAgentsDestroyed(team);
    }

    // Retire Event Function
    private void OnRetire(bool iskilled)
    {
        curHP = maxHP;
        isFiring = false;
        canFire = true;

        if (iskilled)
            _BattleStageEnvController.KillAgent(team);

        OnRetired?.Invoke(gameObject);
    }

    // Send Information to Dashboard UI Event Function
    private void SendInfoToDashboard(string name, int h, int v, int fire)
    {
        SendInfo?.Invoke(name, h, v, fire);
    }
}