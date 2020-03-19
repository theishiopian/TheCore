using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour
{
    public int value;

    //public AnimationCurve bounce;
    //public float offset = 0.125f;

    //private Vector3 topPos;
    //private Vector3 bottomPos;
    //private Transform sprite;

    //// Start is called before the first frame update
    //void Start()
    //{
    //    topPos = new Vector3(0, offset, 0);
    //    bottomPos = new Vector3(0, offset, 0);
    //    sprite = this.transform.Find("Sprite");
    //}

    //private float t = 0;

    //// Update is called once per frame
    //void Update()
    //{
    //    t += Time.deltaTime;
    //    t = Mathf.Repeat(t, 1);
    //    sprite.localPosition = new Vector3(0, bounce.Evaluate(t), 0) * offset;
    //}

    public GameObject burst;
    public GameObject smoke;
    public AnimationCurve curve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
    Vector3 start;
    Transform end;
    float duration = 1.0f;
    float t;
    GameObject player;
    GameObject forgeDoor;
    // Use this for initialization
    void Start()
    {
        t = 0.0f;
        start = this.transform.position;
        end = GlobalVars.GetObject("item_target").transform;
        forgeDoor = GlobalVars.GetObject("forge_door");
        forgeDoor.SetActive(false);
    }
    bool run = true;
    // Update is called once per frame
    void Update()
    {
        if(run)
        {
            t += Time.deltaTime;
            float s = t / duration;
            transform.position = Vector3.Lerp(start, end.position, curve.Evaluate(s));

            if (Vector3.Distance(this.transform.position, end.position) == 0)
            {
                //Debug.Log("done");
                GlobalVars.GetObject("player").GetComponent<PlayerController>().AddXP(value);
                Debug.Log(smoke);
                StartCoroutine("DestructionSequence");
                
                run = false;
            }
        }
    }

    IEnumerator DestructionSequence()
    {
        var burst_instance = Instantiate(burst, this.transform.position, Quaternion.identity);
        var smoke_instance = Instantiate(smoke, this.transform.position, Quaternion.identity);
        this.GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitForSeconds(1f);
        Destroy(burst_instance);
        Destroy(smoke_instance);
        forgeDoor.SetActive(true);
        Destroy(this.gameObject);
        yield return null;
    }
}
