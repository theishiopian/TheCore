using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.Tilemaps.Tile;

public class TileTerrain : TileBase
{
    [SerializeField]
    public Sprite sprite;

    [SerializeField]
    public int hardness;

    [SerializeField]
    public GameObject dropItem;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        base.GetTileData(position, tilemap, ref tileData);

        tileData.sprite = sprite;
        tileData.colliderType = ColliderType.Sprite;
    }

#if UNITY_EDITOR
    [MenuItem("Assets/Create/Terrain Tile")]
    public static void CreateAnimatedTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Terrain Tile", "New Terrain Tile", "asset", "Save Terrain Tile", "Assets");
        if (path == "")
            return;

        AssetDatabase.CreateAsset(CreateInstance<TileTerrain>(), path);
    }
#endif

#region TileEditor Class
    [CustomEditor(typeof(TileTerrain))]
    public class TileEditor : Editor
    {
        public override Texture2D RenderStaticPreview(string assetPath, UnityEngine.Object[] subAssets, int width, int height)
        {
            TileTerrain tile = target as TileTerrain;

            if (tile == null || tile.sprite == null)
                return null;

            Texture2D cache = new Texture2D(width, height);
            EditorUtility.CopySerialized(AssetPreview.GetAssetPreview(tile.sprite.texture), cache);
            return cache;
        }
    }
#endregion
}
