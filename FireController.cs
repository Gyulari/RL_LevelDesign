using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireController : MonoBehaviour
{
    // Type of enemy agents
    private enum TargetType { Blue, Red };
    [SerializeField] private TargetType target;

    // List of enemy agents in fire range
    public List<GameObject> inRangeObjects = new();

    private void OnEnable()
    {
        // Subscribe to the destroy event function
        BattleAgentController.OnRetired += RemoveRetiredObjects;
        BattleStageEnvController.ClearFireRange += ClearFireRangeList;
    }

    private void OnDisable()
    {
        // Unsubscribe to the destroy event function
        BattleAgentController.OnRetired -= RemoveRetiredObjects;
        BattleStageEnvController.ClearFireRange -= ClearFireRangeList;
    }

    public void Fire()
    {
        GameObject enemy = GetNearestEnemy(inRangeObjects);

        // Fire to nearest enemy agnets
        if (enemy != null) {
            DrawFireLine(enemy);
            enemy.GetComponent<BattleAgentController>().HP -= 10;
        }
    }

    // Get nearest enemy agents in fire range
    private GameObject GetNearestEnemy(List<GameObject> objList)
    {
        if (objList.Count == 0)
            return null;

        float nearestDistance = float.MaxValue;
        float dist;
        GameObject nearestEnemy = null;

        foreach (GameObject obj in objList) {
            dist = Vector3.Distance(transform.parent.position, obj.transform.position);

            if(dist < nearestDistance) {
                nearestDistance = dist;
                nearestEnemy = obj;
            }
        }

        return nearestEnemy;
    }

    // Visualizing of firing using DrawLine
    private void DrawFireLine(GameObject enemy)
    {
        if (enemy == null)
            return;

        Vector3 startPoint = transform.position;
        Vector3 endPoint = enemy.transform.position;
        Color color;

        if (target == TargetType.Red)
            color = Color.blue;
        else
            color = Color.red;

        Debug.DrawLine(startPoint, endPoint, color, 0.5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Add enemy agents within the fire range to the list
        if (!inRangeObjects.Contains(other.gameObject) && other.gameObject.transform.CompareTag(target.ToString())) {
            inRangeObjects.Add(other.gameObject.transform.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Remove enemy agents outside the fire range from the list
        if (inRangeObjects.Contains(other.gameObject) && other.gameObject.transform.CompareTag(target.ToString())) {
            inRangeObjects.Remove(other.gameObject.transform.gameObject);
        }
    }

    private void RemoveRetiredObjects(GameObject retiredObjects)
    {
        if (inRangeObjects.Contains(retiredObjects)) {
            inRangeObjects.Remove(retiredObjects);
        }
    }

    private void ClearFireRangeList()
    {
        inRangeObjects.Clear();
    }
}
