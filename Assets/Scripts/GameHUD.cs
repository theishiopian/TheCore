using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameHUD : MonoBehaviour
{
    //depth gauge
    private Transform arrowTransform;
    private const float MAX_DEPTH_ANGLE = -83;
    private const float ZERO_DEPTH_ANGLE = 83;
    private float depthMax;

    //xp bar
    private GameObject slider;
    private Slider sliderBar;

    //level up
    
    public Sprite[] spriteArray;
    public int currentSprite;

    private GameObject player;
    private MapGenerator gen;
    private Image spriteRenderer;
    private Text depthText;

    private void Start()
    {
        depthText = GlobalVars.GetObject("depth_text").GetComponent<Text>();
        spriteRenderer = GlobalVars.GetObject("nixie").GetComponent<Image>();
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
        try
        {
            spriteRenderer.sprite = spriteArray[GlobalVars.level];
            //arrow angle corresponds to depth change
            arrowTransform.eulerAngles = new Vector3(0, 0, GetDepthRotation());

            //xp bar value
            //Debug.Log((float)GlobalVars.xp + "/" + (float)GlobalVars.LevelUpThresholds[GlobalVars.level]);
            //Debug.Log(GlobalVars.xp / GlobalVars.LevelUpThresholds[GlobalVars.level]);
            sliderBar.value = ((float)GlobalVars.xp) / ((float)GlobalVars.LevelUpThresholds[GlobalVars.level]);
        }
        catch
        {

        }
    }

    private float GetDepthRotation()
    {
        //float totalAngleSize = ZERO_DEPTH_ANGLE - MAX_DEPTH_ANGLE;
        //float depthNormalized = Mathf.Abs(depth) / depthMax;

        float t = 0;

        t = Mathf.Abs(player.transform.position.y) / depthMax;

        return -Mathf.Lerp(MAX_DEPTH_ANGLE, ZERO_DEPTH_ANGLE, t);
    }

    //void ChangeLevelSprite()
    //{
    //    spriteRenderer.sprite = spriteArray[currentSprite];
    //    currentSprite++;

    //    if (currentSprite >= spriteArray.Length)
    //        currentSprite = 0;
    //}
   

}
