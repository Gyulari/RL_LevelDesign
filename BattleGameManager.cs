using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

public class BattleGameManager : MonoBehaviour
{
    /*   �ʿ� ���� ������ �����
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

    private void ResetBattleStage(Vector3 resetPos, bool isReset)
    {
        if (!isReset)
            return;

        GameObject battleStage = Resources.Load<GameObject>("Prefab/BattleStage");
        Instantiate(battleStage, resetPos, Quaternion.identity);
    }
    */
}
