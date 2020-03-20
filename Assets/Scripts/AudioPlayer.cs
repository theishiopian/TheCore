using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public AudioSource[] clips;

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void Play(int clip)
    {
        if(!clips[clip].isPlaying)
        {
            clips[clip].Play();
        }
    }

    public void Stop(int clip)
    {
        if(clips[clip].isPlaying)
        {
            clips[clip].Stop();
        }
    }
}
