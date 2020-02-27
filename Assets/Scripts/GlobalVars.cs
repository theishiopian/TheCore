using System.Collections.Generic;
using UnityEngine;

public static class GlobalVars
{
    public static int[] LevelUpThresholds;//init in world generation

    public static int level = 0;

    public static int xp = 0;

    private static Dictionary<string, GameObject> gameObjects = new Dictionary<string, GameObject>();

    public static void RegisterObject(string key, GameObject o)
    {
        gameObjects[key] = o;
    }

    public static GameObject GetObject(string key)
    {
        GameObject g;
        if (gameObjects.TryGetValue(key, out g))
            return g;
        else throw new KeyNotFoundException();
    }
}
