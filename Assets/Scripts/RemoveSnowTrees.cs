using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveSnowTrees : MonoBehaviour
{
    public Terrain terrain;
    public float snowLevel = 70.0f;

    public void RemovesnowTrees()
    {
        TerrainData terrainData = terrain.terrainData;
        TreeInstance[] treeInstances = terrainData.treeInstances;
        List<TreeInstance> updatedTreeInstances = new List<TreeInstance>();

        foreach (TreeInstance treeInstance in treeInstances)
        {
            Vector3 treePosition = Vector3.Scale(treeInstance.position, terrainData.size) + terrain.transform.position;

            if (treePosition.y <= snowLevel)
            {
                updatedTreeInstances.Add(treeInstance);
            }
        }

        terrainData.treeInstances = updatedTreeInstances.ToArray();
    }
}
