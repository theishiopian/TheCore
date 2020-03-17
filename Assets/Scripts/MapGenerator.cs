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
    public int width;//width of generated map
    public int depth;//depth of generated map

    public bool useRandomSeed = true;
    public string customSeed;

    [Range(0, 100)]
    public int randomFillPercent = 50;//percentage of empty space that should be filled

    [Range(0, 10)]
    public int smoothingIterations = 5;//how many times the generator smooths the tiles

    public TileBase topTile;

    [SerializeField]
    public LayerList fillTiles;//the tiles to generate, organized by layer. the first tile in each layyer is the fill tile, the rest are ores

    public GameObject endRoomPrefab;//the end of the map, generates at the bottom

    private Tilemap stoneMap;//cache to store the output tilemap
    private Tilemap oreMap;
    private Grid grid;//cache to store output tilemap alignment grid

    private string seed;//seed for rng
    private int[,] intMap;//storage for initial map shape
    private int[] oreCount;
    private int layerCount;

    //debug log variables
    int cycles = 0;
    int tiles = 0;

    void Start()
    {
        grid = GlobalVars.GetObject("grid").GetComponent<Grid>();
        stoneMap = GlobalVars.GetObject("stone_map").GetComponent<Tilemap>();
        oreMap = GlobalVars.GetObject("ore_map").GetComponent<Tilemap>();
        layerCount = fillTiles.list.Count;
        //cache stuff

        oreCount = new int[fillTiles.list.Count];

        //do the thing
        GenerateMap();
        Debug.Log("Loop cycles: " + cycles);
        Debug.Log("Tiles placed: " + tiles);

        GlobalVars.LevelUpThresholds = new int[layerCount];

        for(int i = 0; i != layerCount; i++)
        {
            //Debug.Log(oreCount[i]);

            if (oreCount[i] > 0)
            {
                var l = oreCount[i] * ((i + 1));

                GlobalVars.LevelUpThresholds[i] = l;
                //Debug.Log(oreCount[i] + " " + l);
            }
            //else Debug.Log("ore count zero");
        }
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
        PopulateTileMaps();
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
                if (IsXYOnEdge(x,y))
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
                else if (neighbourWallTiles < 3)
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

    //helper method to determine whether or not a tile is on the edge of the map
    private bool IsXYOnEdge(int x, int y)
    {
        return (x == 0 || x == width - 1 || y == 0 || y == depth - 1);
    }

    //meat and potatoes. this is where the tiles themselves are placed
    //the intMap is used to define the shape of the map
    void PopulateTileMaps()
    {
        int layers = fillTiles.list.Count -1;
        int layerDepth = depth / layers;

        int currentLayer = 1;
        if (intMap != null)
        {
            float[] weights = generateWeights(fillTiles.list[currentLayer - 1].list.Count);//generate inital weights
            for (int y = 0; y < depth; y++)
            {
                if ((y / currentLayer) == layerDepth)//if at layer transition
                {
                    currentLayer++;
                    weights = generateWeights(fillTiles.list[currentLayer - 1].list.Count);//regenerate weights for next layer
                }

                for (int x = 0; x < width; x++)
                {
                    if (intMap[x, y] > 0)
                    {
                        if(y == 0)
                        {
                            stoneMap.SetTile(new Vector3Int(x - (width / 2), 0, 0), topTile);
                        }
                        else if(y < depth -1)
                        {
                            //iterate debug vars
                            cycles++;
                            tiles++;

                            //place tile
                            int tile = IsXYOnEdge(x, y) ? 0 : GetRandomWeightedIndex(weights);

                            TileBase currentTile = fillTiles.list[currentLayer - 1].list[tile];
                            TileBase initialTile = fillTiles.list[currentLayer - 1].list[0];
                            if (currentTile.GetType() == typeof(OreTile))
                            {
                                oreMap.SetTile(new Vector3Int(x - (width / 2), -y, 0), currentTile);
                                if (tile > 0) oreCount[currentLayer - 1]++;
                                stoneMap.SetTile(new Vector3Int(x - (width / 2), -y, 0), initialTile);
                            }
                            else
                                stoneMap.SetTile(new Vector3Int(x - (width / 2), -y, 0), currentTile);
                        }
                        else
                        {
                            stoneMap.SetTile(new Vector3Int(x - (width / 2), -y, 0), fillTiles.list[currentLayer].list[0]);
                        }
                    }
                }
            }

            Instantiate(endRoomPrefab, new Vector3(0, -depth, 0), Quaternion.identity);
        }
    }

    //helper method for making list of weights for ore generation
    private float[] generateWeights(int size)
    {
        int increment = 20;

        float[] weights = new float[size];

        float total = 100;

        for(int i = 1; i != size; i++)
        {
            total -= increment;

            weights[i] = increment;
            increment = increment == 0 ? 1 : increment/2;
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