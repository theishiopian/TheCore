using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroSequence : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("StartSequence");
    }

    IEnumerator StartSequence()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("Main Scene");
        yield return null;
    }
}
