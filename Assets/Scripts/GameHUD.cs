using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameHUD : MonoBehaviour
{
    //level up
    public Sprite[] spriteArray;
    public int currentSprite;

    //depth gauge
    private Transform arrowTransform;
    private const float MAX_DEPTH_ANGLE = -83;
    private const float ZERO_DEPTH_ANGLE = 83;
    private float depthMax;

    //xp bar
    private GameObject slider;
    private Slider sliderBar; 
    private GameObject player;
    private MapGenerator gen;
    private Image nixieTube;
    private Text depthText;

    private GameObject bitsGroup;
    private GameObject[] bits;
    private int oldLevel = 0;
    private void Start()
    {
        bitsGroup = GlobalVars.GetObject("rock_bits_group");
        bits = new GameObject[6];

        bits[0] = GlobalVars.GetObject("dirt_bits");
        bits[1] = GlobalVars.GetObject("cobble_bits");
        bits[2] = GlobalVars.GetObject("bedrock_bits");
        bits[3] = GlobalVars.GetObject("crystal_bits");
        bits[4] = GlobalVars.GetObject("mantle_bits");
        bits[5] = GlobalVars.GetObject("obsidian_bits");
        
        depthText = GlobalVars.GetObject("depth_text").GetComponent<Text>();
        nixieTube = GlobalVars.GetObject("nixie").GetComponent<Image>();
        player = GlobalVars.GetObject("player");
        gen = GlobalVars.GetObject("grid").GetComponent<MapGenerator>();

        //depth gauge stuff
        arrowTransform = GlobalVars.GetObject("needle").transform;
        depthMax = gen.depth;

        //xp bar stuff
        slider = GlobalVars.GetObject("xp_bar");
        sliderBar = slider.GetComponent<Slider>();
    }
    float y;
    private void Update()
    {
        y = player.transform.position.y;
        depthText.text = (y < 1.4f ? (-Mathf.RoundToInt(y) + 1) + "m" : "Sea Level");

        if(GlobalVars.level <= gen.fillTiles.list.Length - 2)
        {
            if(GlobalVars.level >oldLevel)
            {
                bitsGroup.transform.position += new Vector3(0, 0.3f, 0);
                bits[GlobalVars.level].GetComponent<Image>().enabled = true;
                oldLevel++;
            }
            sliderBar.value = ((float)GlobalVars.xp) / ((float)GlobalVars.LevelUpThresholds[GlobalVars.level]);
            nixieTube.sprite = spriteArray[GlobalVars.level];
        }
        else
        {
            if (GlobalVars.level > oldLevel)
            {
                bitsGroup.transform.position += new Vector3(0, 0.3f, 0);
                bits[5].GetComponent<Image>().enabled = true;
                oldLevel++;
            }
            sliderBar.value = 1;
            nixieTube.enabled = false;
        }
                
        //arrow angle corresponds to depth change
        arrowTransform.eulerAngles = new Vector3(0, 0, GetDepthRotation());
    }

    private float GetDepthRotation()
    {
        //float totalAngleSize = ZERO_DEPTH_ANGLE - MAX_DEPTH_ANGLE;
        //float depthNormalized = Mathf.Abs(depth) / depthMax;

        float t = 0;

        t = Mathf.Abs(player.transform.position.y) / depthMax;

        return -Mathf.Lerp(MAX_DEPTH_ANGLE, ZERO_DEPTH_ANGLE, t);
    }
}
