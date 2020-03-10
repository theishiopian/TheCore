using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.Tilemaps.Tile;

public class StoneTile : TileBase
{
    [SerializeField]
    public Sprite sprite;//sprite to render on this tile

    [SerializeField]
    public int hardness;//how hard this tile is. if the drill's level is less than this vale, the tile is unbreakable
    
    //an override method is one that replaces a method in the base class
    //this class extends the TileBase class, which has its own GetTileData method
    //but i wanna do extra stuff in this method, so I have replaced the default method with my own
    //this method is used to get information about the tile, IE when the engine places them
    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        base.GetTileData(position, tilemap, ref tileData);//we still want to do the stuff the default method does for us

        tileData.sprite = sprite;//set the sprite
        tileData.colliderType = ColliderType.Sprite;//set the collider
    }

    //this is a preproccessor directive (refered to as a ppd from here on out for brevity)
    //a ppd is a special instruction for the compiler. it lets you change the way your code runs depending on certain conditions
    //this ppd tells this portion of code to only execute if the code is running from the editor
    #if UNITY_EDITOR
    //this method creates a file in the assets folder that can be used to quickly reference this custom tile
    //the MenuItem label allows this code to be called from the editor itself through the dropdown menus
    [MenuItem("Assets/Create/Stone Tile")]
    public static void CreateAnimatedTile()
    {
        //this part opens a save panel
        string path = EditorUtility.SaveFilePanelInProject("Save Stone Tile", "New Stone Tile", "asset", "Save Stone Tile", "Assets");
        if (path == "")
            return;//dont let you save to a null path

        AssetDatabase.CreateAsset(CreateInstance<StoneTile>(), path);//create the asset file at the specified location
    }
#endif

#if UNITY_EDITOR

    //this ppd just lets me collapse this section of code when im not working on it
    #region TileEditor Class
    //fun fact! you can have a class inside of a class! a class is just a definition for an object.
    //in this case the object being defined is an editor interface for the custom tile type. as such, it makes sense to define it here
    //this class could also be defined in its own file as well.
    [CustomEditor(typeof(StoneTile))]
    public class StoneTileEditor : Editor
    {
        //another override. this one is overriding the method used to display an icon in the assets folder
        //i want the tile's texture to be visible from the assets folder for ease of use.
        //this code replaces the normal default icon with the tile's sprite texture.
        //the normal method in the "Editor" class just returns a boring generic icon.
        public override Texture2D RenderStaticPreview(string assetPath, UnityEngine.Object[] subAssets, int width, int height)
        {
            StoneTile tile = target as StoneTile;

            if (tile == null || tile.sprite == null)
                return null;//use the default icon if no custom one is available

            Texture2D cache = new Texture2D(width, height);//create a new texture variable
            EditorUtility.CopySerialized(AssetPreview.GetAssetPreview(tile.sprite.texture), cache);//write the tile's texture into it
            return cache;//and display!
        }
    }
    #endregion
#endif
}
