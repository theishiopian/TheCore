﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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

    int[,] intMap;

    public TileBase fillTile;

    private Tilemap tileMap;
    private Grid grid;

    int iterations = 0;

    void Start()
    {
        tileMap = this.GetComponent<Tilemap>();
        grid = tileMap.layoutGrid;
        GenerateMap();
    }

    void Update()//TODO delete
    {
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            GenerateMap();
        }
    }

    void GenerateMap()
    {
        intMap = new int[width, depth];
        RandomFillMap();

        for (int i = 0; i < smoothingIterations; i++)
        {
            SmoothMap();
        }

        //todo ores pass

        PopulateTileMap();
    }

    private string seed;

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
                    intMap[x, y] = 1;//TODO values, or maybe extend tile, or maybe both
                }
                else
                {
                    intMap[x, y] = (pseudoRandom.Next(0, 100) < randomFillPercent) ? 1 : 0;
                }
                iterations++;
            }
        }
    }

    void SmoothMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < depth; y++)
            {
                int neighbourWallTiles = GetSurroundingWallCount(x, y);

                if (neighbourWallTiles > 4)
                    intMap[x, y] = 1;
                else if (neighbourWallTiles < 4)
                    intMap[x, y] = 0;
                iterations++;
            }
        }
    }

    int GetSurroundingWallCount(int gridX, int gridY)
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
                iterations++;
            }
        }

        return wallCount;
    }

    

    void PopulateTileMap()
    {
        if (intMap != null)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < depth; y++)
                {

                    if(intMap[x,y] > 0)
                    {
                        iterations++;
                        tileMap.SetTile(new Vector3Int(x-(width/2), -y, 0), fillTile);
                    }
                }
            }
        }

        Debug.Log(iterations);
    }
}
