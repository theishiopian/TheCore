using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryVFXController : MonoBehaviour
{
    public ParticleSystem spirals;
    public ParticleSystem burst;

    public AnimationCurve yPosition;

    public SpriteRenderer fadeSquare;

    private Vector3 startingPos;
    private new SpriteRenderer renderer;
    // Start is called before the first frame update
    void Start()
    {
        startingPos = this.transform.position;
        renderer = this.GetComponent<SpriteRenderer>();
        StartCoroutine("Sequence");
        
    }

    float t = 0;

    void Update()
    {
        if(t<=1 )
        {
            t += Time.deltaTime/10;
            this.transform.position += new Vector3(transform.position.x, yPosition.Evaluate(t) *0.03f);
        }
    }

    IEnumerator Sequence()
    {
        spirals.Play();
        yield return new WaitForSeconds(7.75f);
        burst.Play();
        yield return new WaitForSeconds(0.5f);
        float time = Time.time;

        while(Time.time < time + 1)
        {
            yield return new WaitForSeconds(0.033f);
            renderer.color -= new Color(0,0,0,0.1f);
        }

        yield return null;
    }
}
