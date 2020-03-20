using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    AudioPlayer sound;
    private void Start()
    {
        SceneManager.LoadScene("Jukebox", LoadSceneMode.Additive);

        StartCoroutine("SoundStart");
    }

    IEnumerator SoundStart()
    {
        yield return new WaitForSeconds(0.1f);
        sound = GlobalVars.GetObject("jukebox").GetComponent<AudioPlayer>();
        sound.Play(1);
        yield return null;
    }

    public void Play()
    {
        sound.Stop(1);
        sound.Play(2);
        SceneManager.LoadScene("Game Intro");
    }

    public void Options()
    {
        //TODO
    }

    public void Quit()
    {
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
    }
}
