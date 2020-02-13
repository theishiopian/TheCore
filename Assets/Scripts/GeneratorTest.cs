using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GeneratorTest : MonoBehaviour
{
    public int depth;
    public TileBase fillTile;

    private Tilemap map;
    private Grid grid;

    // Start is called before the first frame update
    void Start()
    {
        map = this.GetComponent<Tilemap>();
        grid = map.layoutGrid;

        //map.SetTile(new Vector3Int(0,0,0), fillTile);

        for (int y = 0; y != depth; y++)
        {
            //map.SetTile(new Vector3Int(0, -y, 0), fillTile);
            for(int i = -7; i != 7; i++)
            {
                map.SetTile(new Vector3Int(i, -y, 0), fillTile);
            }
        }
    }
}
