﻿using UnityEngine;
using UnityEngine.Tilemaps;

#pragma warning disable 0618

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;    
    public ParticleSystem digParticles;
    public Material[] particleMats;
    public Material sparkMat;

    private Grid grid;
    private Tilemap stoneMap;
    private Tilemap oreMap;
    private Rigidbody2D body;
    private new Camera camera;
    private CameraShake shaker;
    private ParticleSystem.EmissionModule digEmit;
    private ParticleSystemRenderer digParticleRenderer;
    private ParticleSystem lvlUp;
    private Animator animator;
    private ParticleSystem jetpack;
    private AudioPlayer sound;

    // Start is called before the first frame update
    void Start()
    {
        sound = GlobalVars.GetObject("jukebox").GetComponent<AudioPlayer>();
        grid = GlobalVars.GetObject("grid").GetComponent<Grid>();
        stoneMap = GlobalVars.GetObject("stone_map").GetComponent<Tilemap>();
        oreMap = GlobalVars.GetObject("ore_map").GetComponent<Tilemap>();
        body = this.GetComponent<Rigidbody2D>();
        camera = Camera.main;
        shaker = camera.GetComponent<CameraShake>();
        digParticleRenderer = digParticles.GetComponent<ParticleSystemRenderer>();
        digParticleRenderer.material = particleMats[0];
        lvlUp = GlobalVars.GetObject("lvl_up").GetComponent<ParticleSystem>();
        animator = GetComponent<Animator>();
        jetpack = GlobalVars.GetObject("jetpack").GetComponent<ParticleSystem>();
    }

    public void AddXP(int value)
    {
        GlobalVars.xp += value;
        //Debug.Log("Added " + value);

        try
        {
            if (GlobalVars.xp >= GlobalVars.LevelUpThresholds[GlobalVars.level])
            {
                GlobalVars.level++;
                GlobalVars.xp = 0;
                lvlUp.Play();
                //digParticleRenderer.material = particleMats[GlobalVars.level];
            }
        }
        catch
        {

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
    Vector2 oldDir = new Vector2();
    bool changed = false;
    private void FixedUpdate()
    {
        digDir = new Vector2(x, y).normalized;
        //Debug.Log("new: " + digDir + " old: " +oldDir);
        changed = false;
        if (digDir != oldDir)
        {
            changed = true;
        }
        
        if (!(digProgress > 0) && !rocket)
        {
            sound.Stop(0);
            body.position += new Vector2(x, 0);
            if (x < 0)
            {
                animator.Play("Walk_Left");
            }
            if (x > 0)
            {
                animator.Play("Walk_Right");
            }
            if (x == 0)
            {
                animator.Play("Idle");
            }
        }

        if (rocket && !(digProgress > 0))
        {
            sound.Stop(0);
            body.position += new Vector2(x, 0);
            body.AddForce(new Vector2(0, 20), ForceMode2D.Force);
            animator.Play("Fly_Up");
            jetpack.emissionRate = 100;
        }
        else
        {
            jetpack.emissionRate = 0;
        }
             
        if(digDir.magnitude > 0 && !rocket && !changed)
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
                    sound.Play(0);
                    if(x > 0)
                    {
                        animator.Play("Drill_Right");
                    }
                    if (x < 0)
                    {
                        animator.Play("Drill_Left");
                    }
                    if(y < 0)
                    {
                        animator.Play("Drill_Down");
                    }

                    Vector3Int gridPos = grid.WorldToCell(hit.point + (digDir * 0.1f));

                    Vector2 particlePos = new Vector2(gridPos.x + 0.5f, gridPos.y + 0.5f);

                    digParticles.transform.position = particlePos;
  
                    shaker.ShakeCameraContinuous(0.2f, 0.02f);

                    StoneTile stone = (StoneTile)stoneMap.GetTile(gridPos);

                    OreTile ore = (OreTile)oreMap.GetTile(gridPos);

                    bool inBounds = !(gridPos.x >= 7 || gridPos.x < -7);

                    if (inBounds)
                    {
                        if (stone.hardness <=(GlobalVars.level+1))
                        {
                            digProgress += Time.deltaTime;
                            
                            try
                            {
                                digParticleRenderer.material = particleMats[stone.hardness - 1];
                            }
                            catch
                            {

                            }
                        }
                        else
                        {
                            digParticleRenderer.material = sparkMat;
                        }
                        digParticles.emissionRate = 25;

                        if (digProgress >= 0.75f)
                        {
                            stoneMap.SetTile(gridPos, null);
                            oreMap.SetTile(gridPos, null);
                            digProgress = 0;
                            digParticles.emissionRate = 0;
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
            digParticles.emissionRate = 0;
        }
        oldDir = digDir;
    }
}
