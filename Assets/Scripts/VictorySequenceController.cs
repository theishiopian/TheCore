using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VictorySequenceController : MonoBehaviour
{
    public ParticleSystem spirals;
    public ParticleSystem burst;

    public AnimationCurve yPosition;

    public SpriteRenderer fadeSquare;
    public SpriteRenderer fadeSquare2;

    private Vector3 startingPos;
    private new SpriteRenderer renderer;
    private new Camera camera;
    private CameraShake shaker;
    private CameraFollow follow;
    private Text text;
    // Start is called before the first frame update
    void Start()
    {
        startingPos = this.transform.position;
        renderer = this.GetComponent<SpriteRenderer>();
        camera = Camera.main;
        shaker = camera.GetComponent<CameraShake>();
        follow = camera.GetComponent<CameraFollow>();
        StartCoroutine("Sequence");
        follow.SetTarget(this.gameObject);
        shaker.ShakeCamera(2, 5);
        text = GlobalVars.GetObject("win_text").GetComponent<Text>();
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

        yield return new WaitForSeconds(1f);

        while(fadeSquare.color.a < 1)
        {
            yield return new WaitForSeconds(0.033f);
            fadeSquare.color += new Color(0, 0, 0, 0.1f);
        }

        yield return new WaitForSeconds(1);


        while (text.color.a < 1)
        {
            yield return new WaitForSeconds(0.033f);
            text.color += new Color(0, 0, 0, 0.1f);
        }
        yield return new WaitForSeconds(3);

        while (fadeSquare2.color.a < 1)
        {
            yield return new WaitForSeconds(0.033f);
            text.color -= new Color(0, 0, 0, 0.1f);
            fadeSquare2.color += new Color(0, 0, 0, 0.1f);
        }
 
        SceneManager.LoadScene("Game End");
        yield return null;
    }
}
