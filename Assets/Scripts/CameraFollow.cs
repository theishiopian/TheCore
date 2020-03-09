using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float maxCameraSpeed;

    private GameObject target;
    // Start is called before the first frame update
    void Start()
    {
        target = GlobalVars.GetObject("player");
    }

    public void SetTarget(GameObject t)
    {
        target = t;
    }

    float cameraVelocity;

    // Update is called once per frame
    void Update()
    {
        if (target.transform.position.y < 0)
        {
            var y = Mathf.SmoothDamp(target.transform.position.y, this.transform.position.y, ref cameraVelocity, Time.deltaTime, maxCameraSpeed);
            transform.position = new Vector3(1.75f, y, -10);
        }
    }
}
