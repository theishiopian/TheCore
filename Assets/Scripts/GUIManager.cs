using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
    private Text levelText;
    private Text xpText;
    private Text depthText;
    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        levelText = GlobalVars.GetObject("level_text").GetComponent<Text>();
        xpText = GlobalVars.GetObject("xp_text").GetComponent<Text>();
        depthText = GlobalVars.GetObject("depth_text").GetComponent<Text>();
        player = GlobalVars.GetObject("player");
    }
    string depth;
    // Update is called once per frame
    void Update()
    {
        levelText.text = "Drill Power: " + (GlobalVars.level + 1);

        xpText.text = "Current XP: " + GlobalVars.xp + "/" + GlobalVars.LevelUpThresholds[GlobalVars.level];
        float y = player.transform.position.y;
        depth = y < 1.4f ? (-Mathf.RoundToInt(y)+1) + "m" : "Sea Level";
        depthText.text = "Depth: " + depth;
    }
}
