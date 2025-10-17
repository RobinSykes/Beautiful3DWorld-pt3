using System.Collections.Generic;
using UnityEngine;

public class RandomPointsInBoxCollider : MonoBehaviour
{
    public int numberOfPoints = 5;
    public GameObject pointPrefab;
    public List<Transform> patrolPoints = new List<Transform>();

    void Start()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        if (box == null)
        {
            Debug.LogError("No BoxCollider found on this GameObject!");
            return;
        }

        patrolPoints.Clear();

        for (int i = 0; i < numberOfPoints; i++)
        {
            Vector3 randomLocalPoint = new Vector3(
                Random.Range(-box.size.x / 2f, box.size.x / 2f),
                Random.Range(-box.size.y / 2f, box.size.y / 2f),
                Random.Range(-box.size.z / 2f, box.size.z / 2f)
            );

            Vector3 worldPoint = transform.TransformPoint(randomLocalPoint + box.center);

            GameObject newPoint = Instantiate(pointPrefab, worldPoint, Quaternion.identity);
            newPoint.name = "PatrolPoint_" + i;

            patrolPoints.Add(newPoint.transform);

            //Debug.Log("Spawned patrol point " + i + " at " + worldPoint);
        }
    }
}
