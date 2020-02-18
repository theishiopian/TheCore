using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class Layer
{
    public List<TileBase> list;
}

[System.Serializable]
public class LayerList
{
    public List<Layer> list;
}

public class MapGenerator : MonoBehaviour
{
    public int width;
    public int depth;

    public bool useRandomSeed = true;
    public string customSeed;

    [Range(0, 100)]
    public int randomFillPercent = 50;

    [Range(0, 10)]
    public int smoothingIterations = 5;

    [SerializeField]
    public LayerList fillTiles;

    private Tilemap tileMap;
    private Grid grid;
    private string seed;
    private int[,] intMap;


    int cycles = 0;

    int tiles = 0;

    void Start()
    {
        tileMap = this.GetComponent<Tilemap>();
        grid = tileMap.layoutGrid;
        GenerateMap();
        Debug.Log("Loop cycles: " + cycles);
        Debug.Log("Tiles placed: " + tiles);
    }

    void GenerateMap()
    {
        intMap = new int[width, depth];
        RandomFillMap();

        for (int i = 0; i < smoothingIterations; i++)
        {
            SmoothMap();
        }

        //todo ores pass?

        PopulateTileMap();
    }

    void RandomFillMap()
    {
        if (useRandomSeed)
        {
            seed = System.DateTime.Now.ToString();
        }
        else
        {
            seed = customSeed;
        }

        System.Random pseudoRandom = new System.Random(seed.GetHashCode());

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < depth; y++)
            {
                if (x == 0 || x == width - 1 || y == 0 || y == depth - 1)
                {
                    intMap[x, y] = 1;//TODO use higher value for ores and unbreakables
                }
                else
                {
                    intMap[x, y] = (pseudoRandom.Next(0, 100) < randomFillPercent) ? 1 : 0;
                }
                cycles++;
            }
        }
    }

    void SmoothMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < depth; y++)
            {
                int neighbourWallTiles = TileSurvey(x, y).surroundingTiles;
                //todo support for ore
                if (neighbourWallTiles > 4)
                    intMap[x, y] = 1;
                else if (neighbourWallTiles < 4)
                    intMap[x, y] = 0;
                cycles++;
            }
        }
    }

    struct TileSurveyData
    {
        public int tileNum;
        public int surroundingTiles;
    }

    TileSurveyData TileSurvey(int gridX, int gridY)
    {
        int wallCount = 0;
        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
        {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < depth)
                {
                    if (neighbourX != gridX || neighbourY != gridY)
                    {
                        wallCount += intMap[neighbourX, neighbourY];
                    }
                }
                else
                {
                    wallCount++;
                }
                cycles++;
            }
        }
        TileSurveyData data = new TileSurveyData();
        data.surroundingTiles = wallCount;
        data.tileNum = intMap[gridX, gridY];
        return data;
    }

    void PopulateTileMap()
    {
        int layers = fillTiles.list.Count;
        int layerDepth = depth / layers;

        int currentLayer = 1;
        if (intMap != null)
        {
            float[] weights = generateWeights(fillTiles.list[currentLayer - 1].list.Count);
            for (int y = 0; y < depth; y++)
            {
                if ((y / currentLayer) == layerDepth)
                {
                    currentLayer++;
                    weights = generateWeights(fillTiles.list[currentLayer - 1].list.Count);
                    //Debug.Log("moving down");
                }

                for (int x = 0; x < width; x++)
                {
                    if (intMap[x, y] > 0)
                    {
                        cycles++;
                        tiles++;


                        tileMap.SetTile(new Vector3Int(x - (width / 2), -y, 0), fillTiles.list[currentLayer - 1].list[GetRandomWeightedIndex(weights)]);
                    }
                }
            }
        }
    }

    private float[] generateWeights(int size)
    {
        float[] weights = new float[size];

        float total = 100;

        for(int i = 1; i != size; i++)
        {
            total -= 10;

            weights[i] = 10;
        }

        weights[0] = total;

        return weights;
    }


    private int GetRandomWeightedIndex(float[] weights)
    {
        if (weights == null || weights.Length == 0) return -1;

        float w;
        float t = 0;
        int i;
        for (i = 0; i < weights.Length; i++)
        {
            w = weights[i];

            if (float.IsPositiveInfinity(w))
            {
                return i;
            }
            else if (w >= 0f && !float.IsNaN(w))
            {
                t += weights[i];
            }
        }

        float r = Random.value;
        float s = 0f;

        for (i = 0; i < weights.Length; i++)
        {
            w = weights[i];
            if (float.IsNaN(w) || w <= 0f) continue;

            s += w / t;
            if (s >= r) return i;
        }
        Debug.Log("error detected in weighted random, defaulting to null tile");
        return 0;
    }
}


