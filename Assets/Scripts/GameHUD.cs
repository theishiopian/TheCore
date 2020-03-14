using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameHUD : MonoBehaviour
{
    private Transform arrowTransform;
    private const float MAX_DEPTH_ANGLE = -83;
    private const float ZERO_DEPTH_ANGLE = 83;

    //TODO: hook up depth and depthMax to corresponding variables in our game,
    //      change arrow rotation to change with player's depth
    private float depthMax;
    private float depth;

    private void Awake()
    {
        arrowTransform = transform.Find("arrow");

        depth = 0f;
        depthMax = 200f;
    }

    private void Update()
    {
        //for testing functionality
        depth += 30f * Time.deltaTime;
        if (depth > depthMax) depth = depthMax;

        //arrow angle corresponds to depth change
        arrowTransform.eulerAngles = new Vector3(0, 0, GetDepthRotation());
    }

    private float GetDepthRotation()
    {
        float totalAngleSize = ZERO_DEPTH_ANGLE - MAX_DEPTH_ANGLE;
        float depthNormalized = depth / depthMax;
        return ZERO_DEPTH_ANGLE - depthNormalized * totalAngleSize;
    }

}
