using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinalScript : MonoBehaviour
{
    public SpriteRenderer endCard;
    public SpriteRenderer credits1;
    public SpriteRenderer credits2;
    public Text endText;

    // Start is called before the first frame update
    void Start()
    {
        endCard.color -= new Color(0, 0, 0, 1);
        credits1.color -= new Color(0, 0, 0, 1);
        credits2.color -= new Color(0, 0, 0, 1);
        endText.color -= new Color(0, 0, 0, 1);
        StartCoroutine("EndSequence");
    }

    IEnumerator EndSequence()
    {
        while (endCard.color.a < 1)
        {
            yield return new WaitForSeconds(0.033f);
            endCard.color += new Color(0, 0, 0, 0.1f);
        }
        yield return new WaitForSeconds(3);

        while (endCard.color.a > 0)
        {
            yield return new WaitForSeconds(0.033f);
            endCard.color -= new Color(0, 0, 0, 0.1f);
        }

        while (credits1.color.a < 1)
        {
            yield return new WaitForSeconds(0.033f);
            credits1.color += new Color(0, 0, 0, 0.1f);
        }
        yield return new WaitForSeconds(5);

        while (credits1.color.a > 0)
        {
            yield return new WaitForSeconds(0.033f);
            credits1.color -= new Color(0, 0, 0, 0.1f);
        }

        while (credits2.color.a < 1)
        {
            yield return new WaitForSeconds(0.033f);
            credits2.color += new Color(0, 0, 0, 0.1f);
        }
        yield return new WaitForSeconds(5);

        while (credits2.color.a > 0)
        {
            yield return new WaitForSeconds(0.033f);
            credits2.color -= new Color(0, 0, 0, 0.1f);
        }

        yield return new WaitForSeconds(0.5f);

        while (endText.color.a < 1)
        {
            yield return new WaitForSeconds(0.033f);
            endText.color += new Color(0, 0, 0, 0.1f);
        }

        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Escape));

        while (endCard.color.a > 0)
        {
            yield return new WaitForSeconds(0.033f);
            endCard.color -= new Color(0, 0, 0, 0.1f);
            endText.color -= new Color(0, 0, 0, 0.1f);
        }

        if (Application.isEditor)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
        else
        {
            Application.Quit();
        }

        yield return null;
    }
}
