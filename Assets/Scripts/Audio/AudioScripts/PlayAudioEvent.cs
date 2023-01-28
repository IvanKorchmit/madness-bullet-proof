using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudioEvent : MonoBehaviour
{
    public AudioEvent audioEvent;
    public AudioSource source;

    public void Play()
    {
        audioEvent.Play(source);
    }
}
