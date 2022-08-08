using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using System.Collections;

public class PlayerAgent : Agent
{
    private Transform tr;
    private Rigidbody rb;

    public Transform targetTr;
    public Renderer floorRd;

    private Material originMt;
    public Material CFloor;
    public Material WFloor;

    public override void Initialize()
    {
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        originMt = floorRd.material;

        MaxStep = 1000;
    }

    public override void OnEpisodeBegin()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        tr.localPosition = new Vector3(Random.Range(-4.0f, 4.0f), 0.05f, Random.Range(-4.0f, 4.0f));
        targetTr.localPosition = new Vector3(Random.Range(-4.0f, 4.0f), 0.55f, Random.Range(-4.0f, 4.0f));

        StartCoroutine(RevertMaterial());
    }


    public override void CollectObservations(Unity.MLAgents.Sensors.VectorSensor sensor)
    {
        sensor.AddObservation(targetTr.localPosition);
        sensor.AddObservation(tr.localPosition);
        sensor.AddObservation(rb.velocity.x);
        sensor.AddObservation(rb.velocity.z);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        //데이터를 정규화
        float h = Mathf.Clamp(actionBuffers.ContinuousActions[0], -1.0f, 1.0f);
        float v = Mathf.Clamp(actionBuffers.ContinuousActions[1], -1.0f, 1.0f);
        Vector3 dir = (Vector3.forward * v) + (Vector3.right * h);
        rb.AddForce(dir.normalized * 50.0f);

        //지속적으로 이동을 이끌어내기 위한 마이너스 보상
        SetReward(-0.001f);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
        Debug.Log($"[0]={continuousActionsOut[0]} [1]={continuousActionsOut[1]}");
    }

    void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.CompareTag("OUTLINE"))
        {
            floorRd.material = WFloor;

            SetReward(-1.0f);
            EndEpisode();
        }

        if (coll.collider.CompareTag("TARGET"))
        {
            floorRd.material = CFloor;

            SetReward(+1.0f);
            EndEpisode();
        }
    }

    IEnumerator RevertMaterial()
    {
        yield return new WaitForSeconds(0.2f);
        floorRd.material = originMt;
    }
}