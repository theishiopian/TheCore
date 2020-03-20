using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroSequence : MonoBehaviour
{
    public SpriteRenderer[] cards;
    
    // Start is called before the first frame update
    void Start()
    {
        foreach (SpriteRenderer s in cards)
        {
            s.color = new Color(1, 1, 1, 0);
        }
        StartCoroutine("StartSequence");
    }

    IEnumerator StartSequence()
    {
        for (int i = 0; i != 3; i++)
        {
            while (cards[i].color.a < 1)
            {
                yield return new WaitForSeconds(0.033f);
                cards[i].color += new Color(0, 0, 0, 0.1f);
            }
            yield return new WaitForSeconds(3f);

            while (cards[i].color.a > 0)
            {
                yield return new WaitForSeconds(0.033f);
                cards[i].color -= new Color(0, 0, 0, 0.1f);
            }
        }

        while (cards[3].color.a < 1)
        {
            yield return new WaitForSeconds(0.033f);
            cards[3].color += new Color(0, 0, 0, 0.1f);
        }

        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));

        while (cards[3].color.a > 0)
        {
            yield return new WaitForSeconds(0.033f);
            cards[3].color -= new Color(0, 0, 0, 0.1f);
        }

        SceneManager.LoadScene("Main Scene");
        yield return null;
    }
}
