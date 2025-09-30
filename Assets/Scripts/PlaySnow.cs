using UnityEngine;

public class PlaySnow : MonoBehaviour
{
    public Transform player;
    public GameObject snowObject;
    public GameObject rainObject;
    public Terrain terrain;
    public float activationHeight = 70f;
    public int snowLayerIndex = 5; // Index of snow texture in terrain layers
    public float blendStrength = 1f;

    private bool snowPainted = false;

    void Update()
    {
        bool isSnowing = player.position.y > activationHeight;

        snowObject.SetActive(isSnowing);
        //rainObject.SetActive(!isSnowing);

        if (isSnowing && !snowPainted)
        {
            //PaintSnowLayer();
            snowPainted = true;
        }
        else if (!isSnowing && snowPainted)
        {
            //ClearSnowLayer();
            snowPainted = false;
        }
    }
    //activate this to spawn in snow layers if needed
    // be carefull, this changes all lower layers to grass
    void PaintSnowLayer()
    {
        TerrainData terrainData = terrain.terrainData;
        int width = terrainData.alphamapWidth;
        int height = terrainData.alphamapHeight;
        float[,,] splatmap = terrainData.GetAlphamaps(0, 0, width, height);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                float worldX = (float)x / width * terrainData.size.x + terrain.transform.position.x;
                float worldZ = (float)z / height * terrainData.size.z + terrain.transform.position.z;
                float worldY = terrain.SampleHeight(new Vector3(worldX, 0, worldZ)) + terrain.transform.position.y;

                if (worldY >= activationHeight)
                {
                    for (int i = 0; i < terrainData.alphamapLayers; i++)
                    {
                        splatmap[z, x, i] = (i == snowLayerIndex) ? blendStrength : 0f;
                    }
                }
            }
        }

        terrainData.SetAlphamaps(0, 0, splatmap);
    }

    void ClearSnowLayer()
    {
        TerrainData terrainData = terrain.terrainData;
        int width = terrainData.alphamapWidth;
        int height = terrainData.alphamapHeight;
        float[,,] splatmap = terrainData.GetAlphamaps(0, 0, width, height);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                for (int i = 0; i < terrainData.alphamapLayers; i++)
                {
                    splatmap[z, x, i] = (i == 0) ? 1f : 0f; // Reset to base layer
                }
            }
        }

        terrainData.SetAlphamaps(0, 0, splatmap);
    }
}
