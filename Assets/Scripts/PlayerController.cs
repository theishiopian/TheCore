using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public float maxCameraSpeed;
    public Tilemap tiles;
    public ParticleSystem digParticles;

    private Rigidbody2D body;
    private new Camera camera;
    private Grid grid;
    private ParticleSystem.EmissionModule digEmit;

    int xp = 0;
    int level = 0;

    // Start is called before the first frame update
    void Start()
    {
        body = this.GetComponent<Rigidbody2D>();
        camera = Camera.main;
        grid = tiles.layoutGrid;
        digEmit = digParticles.emission;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Item"))
        {
            //TODO: xp system
            
            int value = collision.collider.gameObject.GetComponent<Item>().value;
            xp += value;
            Debug.Log("Added " + value);
            Destroy(collision.collider.gameObject);

            if(xp >= GlobalVars.LevelTops[level])
            {
                level++;
                xp = 0;
            }
        }
    }

    float x;
    float y;
    float cameraVelocity;
    bool rocket = false;

    private void Update()
    {
        x = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        y = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        if(Input.GetKey(KeyCode.Space))
        {
            rocket = true;
        }
        else
        {
            rocket = false;
        }
    }

    Vector2 digDir;
    float digProgress = 0;

    private void FixedUpdate()
    {
        if(digProgress == 0)body.position += new Vector2(x, 0);

        if (rocket && digProgress == 0)
        {
            body.AddForce(new Vector2(0, 20), ForceMode2D.Force);
        }

        if (transform.position.y < 0)
        {
            var y = Mathf.SmoothDamp(camera.transform.position.y, this.transform.position.y, ref cameraVelocity, Time.deltaTime, maxCameraSpeed);
            camera.transform.position = new Vector3(1.75f, y, -10);
        }

        digDir = new Vector2(x,y).normalized;
        
        if(digDir.magnitude > 0 && !rocket)
        {
            try
            {
                if (digDir.y > 0 || Mathf.Abs(digDir.x) > 0)
                {
                    digDir.y = 0;
                }

                RaycastHit2D hit = Physics2D.CircleCast(this.transform.position, 0.3f, digDir, 0.35f);

                if (hit.collider.CompareTag("Blocks"))
                {
                    Vector3Int gridPos = grid.WorldToCell(hit.point + (digDir * 0.1f));

                    Vector2 particlePos = new Vector2(gridPos.x + 0.5f, gridPos.y + 0.5f);
                    digParticles.transform.position = particlePos;

                    TileTerrain tile = (TileTerrain)tiles.GetTile(gridPos);

                    if(!(gridPos.x >= 7 || gridPos.x < -7))
                    {
                        if(tile.hardness <=(level+1))digProgress += Time.deltaTime;
                        digEmit.rateOverTime = 5;
                        if (digProgress >= 1)
                        {
                            if(tile.dropItem != null)Instantiate(tile.dropItem, particlePos, Quaternion.identity);
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
