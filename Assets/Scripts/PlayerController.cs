using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;    
    public ParticleSystem digParticles;

    private Grid grid;
    private Tilemap stoneMap;
    private Tilemap oreMap;
    private Rigidbody2D body;
    private new Camera camera;
    private CameraShake shaker;
    private ParticleSystem.EmissionModule digEmit;

    // Start is called before the first frame update
    void Start()
    {
        grid = GlobalVars.GetObject("grid").GetComponent<Grid>();
        stoneMap = GlobalVars.GetObject("stone_map").GetComponent<Tilemap>();
        oreMap = GlobalVars.GetObject("ore_map").GetComponent<Tilemap>();
        body = this.GetComponent<Rigidbody2D>();
        camera = Camera.main;
        shaker = camera.GetComponent<CameraShake>();
        digEmit = digParticles.emission;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Item"))
        {
            int value = collision.collider.gameObject.GetComponent<Item>().value;
            GlobalVars.xp += value;
            Debug.Log("Added " + value);
            Destroy(collision.collider.gameObject);

            try
            {
                if (GlobalVars.xp >= GlobalVars.LevelUpThresholds[GlobalVars.level])
                {
                    GlobalVars.level++;
                    GlobalVars.xp = 0;
                }
            }
            catch
            {

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

        rocket = Input.GetKey(KeyCode.Space) ? true : false;
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

        //if (transform.position.y < 0)
        //{
        //    var y = Mathf.SmoothDamp(camera.transform.position.y, this.transform.position.y, ref cameraVelocity, Time.deltaTime, maxCameraSpeed);
        //    camera.transform.position = new Vector3(1.75f, y, -10);
        //}

        digDir = new Vector2(x,y).normalized;
        
        if(digDir.magnitude > 0 && !rocket)
        {
            //Debug.Log(0);
            try
            {
                if (digDir.y > 0 || Mathf.Abs(digDir.x) > 0)
                {
                    digDir.y = 0;
                }

                RaycastHit2D hit = Physics2D.CircleCast(this.transform.position, 0.3f, digDir, 0.35f);

                if (hit.collider.CompareTag("Blocks"))
                {
                    //Debug.Log(1);
                    Vector3Int gridPos = grid.WorldToCell(hit.point + (digDir * 0.1f));
                    //Debug.Log(1.1);
                    Vector2 particlePos = new Vector2(gridPos.x + 0.5f, gridPos.y + 0.5f);
                    //Debug.Log(1.2);
                    digParticles.transform.position = particlePos;
                    //Debug.Log(1.3);
                    shaker.ShakeCameraContinuous(0.2f, 0.02f);
                    //Debug.Log(1.4);
                    StoneTile stone = (StoneTile)stoneMap.GetTile(gridPos);
                    //Debug.Log(1.5);
                    OreTile ore = (OreTile)oreMap.GetTile(gridPos);
                    //Debug.Log(1.6);
                    bool inBounds = !(gridPos.x >= 7 || gridPos.x < -7);
                    //Debug.Log(1.7);
                    //Debug.Log(inBounds);
                    if (inBounds)
                    {
                        //Debug.Log(2);
                        if (stone.hardness <=(GlobalVars.level+1))digProgress += Time.deltaTime;
                        digEmit.rateOverTime = 25;
                        if (digProgress >= 0.6f)
                        {
                            //Debug.Log(3);
                            stoneMap.SetTile(gridPos, null);
                            oreMap.SetTile(gridPos, null);
                            digProgress = 0;
                            digEmit.rateOverTime = 0;
                            //Debug.Log(ore.dropItem);
                            if (ore.dropItem != null) Instantiate(ore.dropItem, particlePos, Quaternion.identity);
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
