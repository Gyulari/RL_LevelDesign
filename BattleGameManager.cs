using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

public class BattleGameManager : MonoBehaviour
{
    int accStepCount = 0;
    int curStepCount = 0;
    bool stepReset = false;

    private void OnEnable()
    {
        // Subscribe to the reset event function
        BattleStageEnvController.OnReset += ResetBattleStage;
    }

    private void OnDisable()
    {
        // Unsubscribe to the destroy event function
        BattleStageEnvController.OnReset -= ResetBattleStage;
    }

    private void FixedUpdate()
    {
        var statsRecorder = Academy.Instance.StatsRecorder;

        int totalStepCount = Academy.Instance.TotalStepCount;
        int stepCount = Academy.Instance.StepCount;

        /*
        if (stepCount < curStepCount) {
            stepReset = true;
        }

        if (!stepReset) {
            accStepCount -= curStepCount;
        }
        */

        if (stepCount >= curStepCount)
            accStepCount -= curStepCount;

        curStepCount = stepCount;
        accStepCount += stepCount;

        /*
        if (!stepReset) {   
            accStepCount -= curStepCount;
            curStepCount = stepCount;
            accStepCount += stepCount;
        }
        else {
            stepReset = false;
            curStepCount = stepCount;
            accStepCount += stepCount;
        }
        */


        // stepCount �ʱ�ȭ�� ���� ���� ��Ȳ
        // stepCount��ŭ acc�� �����ְ� ���������� �ʱ�ȭ �ȵ� ä�� ���� curStepCount��ŭ ���ְ� stepCount ���ϱ�

        // stepCount�� �ʱ�ȭ �� ��Ȳ
        // �״�� curStepCount��ŭ ������ ���� �״�� ����, ���� stepCount ���ϱ� ����

        /*
        curStepCount = stepCount;
        accStepCount += stepCount;


        if(stepCount >= curStepCount) {
            curStepCount = stepCount;
            stepReset = false;
        }
        else if (!stepReset) {
            stepReset = true;
            accStepCount += curStepCount;
            curStepCount = 0;
        }
        */

        statsRecorder.Add("Accurated Step Count", accStepCount);
        statsRecorder.Add("Total Step Count", totalStepCount);
    }

    private void ResetBattleStage(Vector3 resetPos, bool isReset)
    {
        if (!isReset)
            return;

        GameObject battleStage = Resources.Load<GameObject>("Prefab/BattleStage");
        Instantiate(battleStage, resetPos, Quaternion.identity);
    }
}
