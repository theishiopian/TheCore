using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameHUD : MonoBehaviour
{
    private Transform arrowTransform;
    private const float MAX_DEPTH_ANGLE = -83;
    private const float ZERO_DEPTH_ANGLE = 83;
    private GameObject slider;
    private Slider sliderBar;
    public SpriteRenderer spriteRenderer;
    public Sprite[] spriteArray;
    public int currentSprite;
    private float depthMax;

    private GameObject player;
    private MapGenerator gen;

    private void Start()
    {
        player = GlobalVars.GetObject("player");
        gen = GlobalVars.GetObject("grid").GetComponent<MapGenerator>();

        //set depth gauge stuff
        arrowTransform = GlobalVars.GetObject("needle").transform;
        depthMax = gen.depth;

        //set xp bar stuff
        slider = GlobalVars.GetObject("xp_bar");
        sliderBar = slider.GetComponent<Slider>();

        //set level up stuff
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = spriteArray[0];
    }

    private void Update()
    {
        //arrow angle corresponds to depth change
        arrowTransform.eulerAngles = new Vector3(0, 0, GetDepthRotation());

        //xp bar value
        sliderBar.value = GlobalVars.xp;
    }

    private float GetDepthRotation()
    {
        //float totalAngleSize = ZERO_DEPTH_ANGLE - MAX_DEPTH_ANGLE;
        //float depthNormalized = Mathf.Abs(depth) / depthMax;

        float t = 0;

        t = Mathf.Abs(player.transform.position.y) / depthMax;

        return -Mathf.Lerp(MAX_DEPTH_ANGLE, ZERO_DEPTH_ANGLE, t);
    }

    void ChangeLevelSprite()
    {
        spriteRenderer.sprite = spriteArray[currentSprite];
        currentSprite++;

        if (currentSprite >= spriteArray.Length)
            currentSprite = 0;
    }
   

}
