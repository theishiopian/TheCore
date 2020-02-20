using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public float castDistance;
    public float maxCameraSpeed;
    public Tilemap tiles;
    public ParticleSystem digParticles;

    private Rigidbody2D body;
    private new Camera camera;
    private Grid grid;
    private ParticleSystem.EmissionModule digEmit;

    // Start is called before the first frame update
    void Start()
    {
        body = this.GetComponent<Rigidbody2D>();
        camera = Camera.main;
        grid = tiles.layoutGrid;
        digEmit = digParticles.emission;
    }

    float x;
    float y;
    float cameraVelocity;
    bool jump = false;

    private void Update()
    {
        x = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        y = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        if(Input.GetKey(KeyCode.Space))
        {
            jump = true;
        }
        else
        {
            jump = false;
        }
    }

    Vector2 digDir;
    float digProgress = 0;

    void FixedUpdate()
    {
        body.position += new Vector2(x, 0);

        if (jump)
        {
            body.AddForce(new Vector2(0, 20), ForceMode2D.Force);
        }

        if (transform.position.y < 0)
        {
            var y = Mathf.SmoothDamp(camera.transform.position.y, this.transform.position.y, ref cameraVelocity, Time.deltaTime, maxCameraSpeed);
            camera.transform.position = new Vector3(0, y, -10);
        }

        digDir = new Vector2(x,y).normalized;
        
        if(digDir.magnitude > 0 && !jump)
        {
            try
            {
                if (digDir.y > 0 || Mathf.Abs(digDir.x) > 0)
                {
                    digDir.y = 0;
                }

                RaycastHit2D hit = Physics2D.CircleCast(this.transform.position, 0.3f, digDir, 1.0f);

                if (hit.collider.CompareTag("Blocks"))
                {
                    Vector3Int gridPos = grid.WorldToCell(hit.point + (digDir * 0.1f));

                    Vector2 particlePos = new Vector2(gridPos.x + 0.5f, gridPos.y + 0.5f);
                    digParticles.transform.position = particlePos;

                    TileTerrain tile = (TileTerrain)tiles.GetTile(gridPos);

                    //Debug.Log(gridPos);

                    if(!(gridPos.x >= 7 || gridPos.x < -7))
                    {
                        if(tile.hardness <=1)digProgress += Time.deltaTime;
                        digEmit.rateOverTime = 5;
                        if (digProgress >= 1)
                        {
                            tiles.SetTile(gridPos, null);
                            digProgress = 0;
                            digEmit.rateOverTime = 0;
                        }
                    }
                }
            }
            catch//(System.NullReferenceException n)
            {
                //Debug.Log("drill hit air");
            }
        }
        else
        {
            digProgress = 0;
            digEmit.rateOverTime = 0;
        }
    }
}
