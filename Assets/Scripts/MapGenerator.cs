using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//helper classes for editor-accessible nested lists
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
    public int randomFillPercent = 50;//percentage of empty space that should be filled

    [Range(0, 10)]
    public int smoothingIterations = 5;//how many times the generator smooths the tiles

    [SerializeField]
    public LayerList fillTiles;//the tiles to generate, organized by layer. the first tile in each layyer is the fill tile, the rest are ores

    private Tilemap tileMap;//cache to store the output tilemap
    private Grid grid;//cache to store output tilemap alignment grid
    private string seed;//seed for rng
    private int[,] intMap;//storage for initial map shape

    //debug log variables
    int cycles = 0;
    int tiles = 0;

    void Start()
    {
        //cache stuff
        tileMap = this.GetComponent<Tilemap>();
        grid = tileMap.layoutGrid;
        //do the thing
        GenerateMap();
        Debug.Log("Loop cycles: " + cycles);
        Debug.Log("Tiles placed: " + tiles);
    }

    void GenerateMap()
    {
        //fill map with random squares
        intMap = new int[width, depth];
        RandomFillMap();

        //do smoothing
        for (int i = 0; i < smoothingIterations; i++)
        {
            SmoothMap();
        }

        //generate actual tiles
        PopulateTileMap();
    }

    //fill map with random squares
    void RandomFillMap()
    {
        if (useRandomSeed)
        {
            seed = System.DateTime.Now.ToString();//generate seed from date
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
                    intMap[x, y] = 1;
                }
                else
                {
                    intMap[x, y] = (pseudoRandom.Next(0, 100) < randomFillPercent) ? 1 : 0;
                }

                //iterate debug var
                cycles++;
            }
        }
    }

    void SmoothMap()//run smoothing iteration via cellular automata
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < depth; y++)
            {
                int neighbourWallTiles = GetSurroundingTileCount(x, y);
                if (neighbourWallTiles > 4)
                    intMap[x, y] = 1;
                else if (neighbourWallTiles < 4)
                    intMap[x, y] = 0;

                //iterate debug var
                cycles++;
            }
        }
    }

    //helper method for cellular automata algorithm
    int GetSurroundingTileCount(int gridX, int gridY)
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
                //iterate debug var
                cycles++;
            }
        }
        return wallCount;
    }

    //meat and potatoes. this is where the tiles themselves are placed
    void PopulateTileMap()
    {
        int layers = fillTiles.list.Count;
        int layerDepth = depth / layers;

        int currentLayer = 1;
        if (intMap != null)
        {
            float[] weights = generateWeights(fillTiles.list[currentLayer - 1].list.Count, 5);//generate inital weights
            for (int y = 0; y < depth; y++)
            {
                if ((y / currentLayer) == layerDepth)//if at layer transition
                {
                    currentLayer++;
                    weights = generateWeights(fillTiles.list[currentLayer - 1].list.Count, 5);//regenerate weights for next layer
                }

                for (int x = 0; x < width; x++)
                {
                    if (intMap[x, y] > 0)
                    {
                        //iterate debug vars
                        cycles++;
                        tiles++;
                        
                        //place tile
                        tileMap.SetTile(new Vector3Int(x - (width / 2), -y, 0), fillTiles.list[currentLayer - 1].list[GetRandomWeightedIndex(weights)]);
                    }
                }
            }
        }
    }

    //helper method for making list of weights
    private float[] generateWeights(int size, int increment)
    {
        float[] weights = new float[size];

        float total = 100;

        for(int i = 1; i != size; i++)
        {
            total -= increment;

            weights[i] = increment;
        }

        weights[0] = total;

        return weights;
    }

    //helper method to get random index from weight list
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
        Debug.Log("error detected in weighted random, defaulting to fill tile value");
        return 0;
    }
}