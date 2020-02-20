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
    float y;
    float cameraVelocity;
    bool jump = false;

    private void Update()
    {
        x = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        y = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        if(Input.GetKeyDown(KeyCode.Space))
        {
            RaycastHit2D hit = Physics2D.CircleCast(this.transform.position, 0.5f, Vector2.down, 0.6f);
            if(hit.collider.CompareTag("Blocks"))
            {
                jump = true;
            }
        }
    }

    void FixedUpdate()
    {
        body.position += new Vector2(x, 0);

        if (jump)
        {
            jump = false;
            body.AddForce(new Vector2(0, 5), ForceMode2D.Impulse);
        }

        if (transform.position.y < 0)
        {
            var y = Mathf.SmoothDamp(camera.transform.position.y, this.transform.position.y, ref cameraVelocity, Time.deltaTime, maxCameraSpeed);
            camera.transform.position = new Vector3(0, y, -10);
        }
    }
}
