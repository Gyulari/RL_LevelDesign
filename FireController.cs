using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireController : MonoBehaviour
{
    // Type of enemy agents
    private enum TargetType { Blue, Red };
    [SerializeField] private TargetType target;

    // List of enemy agents in fire range
    [HideInInspector] public List<GameObject> collidedObjects = new();

    private void OnEnable()
    {
        // Subscribe to the destroy event function
        BattleAgentController.OnDestroyed += RemoveDestroyedObjects;
    }

    private void OnDisable()
    {
        // Unsubscribe to the destroy event function
        BattleAgentController.OnDestroyed -= RemoveDestroyedObjects;
    }

    public void Fire()
    {
        GameObject enemy = GetNearestEnemy(collidedObjects);

        // Fire to nearest enemy agnets
        if (enemy != null) {
            DrawFireLine(enemy);
            enemy.GetComponent<BattleAgentController>().CurrentHP -= 10;
        }
    }

    // Get nearest enemy agents in fire range
    private GameObject GetNearestEnemy(List<GameObject> collidedObjects)
    {
        if (collidedObjects.Count == 0)
            return null;

        float nearestDistance = float.MaxValue;
        float dist;
        GameObject nearestEnemy = null;

        foreach (GameObject obj in collidedObjects) {
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
        if (!collidedObjects.Contains(other.gameObject) && other.gameObject.transform.CompareTag(target.ToString())) {
            collidedObjects.Add(other.gameObject.transform.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Remove enemy agents outside the fire range from the list
        if (collidedObjects.Contains(other.gameObject) && other.gameObject.transform.CompareTag(target.ToString())) {
            collidedObjects.Remove(other.gameObject.transform.gameObject);
        }
    }

    // Remove destroyed enemy agents from the list
    private void RemoveDestroyedObjects(GameObject destroyedObjects)
    {
        if (collidedObjects.Contains(destroyedObjects)) {
            collidedObjects.Remove(destroyedObjects);
        }
    }
}
