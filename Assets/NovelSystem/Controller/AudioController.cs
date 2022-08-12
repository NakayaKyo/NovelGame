using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    private AudioSource audioSource;

    private AudioClip audioClip;

    /// <summary>
    /// intiate the controller
    /// </summary>
    public void initiate(AudioSource source)
    {
        audioSource = source;
    }

    public void setAudio(AudioClip clip)
    {
        audioClip = clip;
    }

    public void play()
    {
        audioSource.clip = audioClip;
        audioSource.Play();
    }

}
