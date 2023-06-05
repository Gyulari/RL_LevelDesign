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


        // stepCount 초기화가 되지 않은 상황
        // stepCount만큼 acc에 더해주고 다음번에도 초기화 안된 채로 오면 curStepCount만큼 빼주고 stepCount 더하기

        // stepCount가 초기화 된 상황
        // 그대로 curStepCount만큼 빼주지 말고 그대로 진행, 새로 stepCount 더하기 시작

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
