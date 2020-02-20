using UnityEngine;
using UnityEngine.Tilemaps;

public class DestructionTest : MonoBehaviour
{
    public Tilemap tiles;

    private Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10;

        Vector3 screenPos = cam.ScreenToWorldPoint(mousePos);

        RaycastHit2D hit = Physics2D.Raycast(screenPos, Vector2.zero);

        try
        {
            if (Input.GetMouseButton(0) && hit.collider.CompareTag("Blocks"))
            {
                Grid grid = tiles.layoutGrid;
                Vector3Int gridPos = grid.WorldToCell(hit.point);

                TileTerrain tile = (TileTerrain)tiles.GetTile(gridPos);

                Debug.Log(tile.hardness);

                tiles.SetTile(gridPos, null);
            }
        }
        catch
        { }
    }
}
