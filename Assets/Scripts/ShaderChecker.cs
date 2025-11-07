using UnityEngine;
using System.Collections.Generic;

public class ShaderChecker : MonoBehaviour
{
    void Start()
    {
        HashSet<string> loggedShaders = new HashSet<string>();

        // Check all normal Renderers
        Renderer[] renderers = Object.FindObjectsOfType<Renderer>();
        foreach (Renderer rend in renderers)
        {
            foreach (Material mat in rend.sharedMaterials)
            {
                if (mat != null)
                    LogShader(mat.shader, loggedShaders);
            }
        }

        // -------------------------
        // ? Check Terrain Materials
        // -------------------------
        Terrain[] terrains = Object.FindObjectsOfType<Terrain>();
        foreach (Terrain terrain in terrains)
        {
            // Terrain material (URP/HDRP/BiRP)
            if (terrain.materialTemplate != null)
                LogShader(terrain.materialTemplate.shader, loggedShaders);

            // Terrain detail (grass/billboards)
            TerrainData data = terrain.terrainData;
            if (data != null && data.detailPrototypes != null)
            {
                foreach (var detail in data.detailPrototypes)
                {
                    if (detail.prototype != null)
                    {
                        Renderer r = detail.prototype.GetComponent<Renderer>();
                        if (r != null)
                        {
                            foreach (Material mat in r.sharedMaterials)
                            {
                                if (mat != null)
                                    LogShader(mat.shader, loggedShaders);
                            }
                        }
                    }
                }
            }
        }

        Debug.Log($"? Total unique shaders used: {loggedShaders.Count}");
    }

    void LogShader(Shader shader, HashSet<string> loggedShaders)
    {
        if (shader == null) return;

        if (!loggedShaders.Contains(shader.name))
        {
            loggedShaders.Add(shader.name);
            Debug.Log("Shader Used: " + shader.name);
        }
    }
}
