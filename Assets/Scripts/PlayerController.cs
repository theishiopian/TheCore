using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public float castDistance;
    public float maxCameraSpeed;

    private Rigidbody2D body;
    private new Camera camera;

    // Start is called before the first frame update
    void Start()
    {
        body = this.GetComponent<Rigidbody2D>();
        camera = Camera.main;
    }

    float x;
    float cameraVelocity;
    // Update is called once per frame
    void FixedUpdate()
    {
        x = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;

        body.position += new Vector2(x, 0);

        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(Physics2D.Raycast(this.transform.position, Vector2.down, castDistance))
            {
                body.AddForce(new Vector2(0, 5), ForceMode2D.Impulse);
            }
        }

        if(transform.position.y < 0)
        {
            var y = Mathf.SmoothDamp(camera.transform.position.y, this.transform.position.y, ref cameraVelocity, Time.deltaTime, maxCameraSpeed);
            camera.transform.position = new Vector3(0, y, -10);
        }
    }
}
