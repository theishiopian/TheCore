using UnityEngine;

public class Item : MonoBehaviour
{
    public int value;

    public AnimationCurve bounce;
    public float offset = 0.125f;

    private Vector3 topPos;
    private Vector3 bottomPos;
    private Transform sprite;

    // Start is called before the first frame update
    void Start()
    {
        topPos = new Vector3(0, offset, 0);
        bottomPos = new Vector3(0, offset, 0);
        sprite = this.transform.Find("Sprite");
    }

    private float t = 0;

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        t = Mathf.Repeat(t, 1);
        sprite.localPosition = new Vector3(0, bounce.Evaluate(t), 0) * 0.125f;
    }
}
