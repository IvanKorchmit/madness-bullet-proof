using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Audio Events/Simple")]
public class SimpleAudioEvent : AudioEvent
{
    public AudioClip[] clips;

    [MinMaxRange(0, 2)]
    public RangedFloat volume = new RangedFloat(1, 1);

    [MinMaxRange(0, 2)]
    public RangedFloat pitch = new RangedFloat(1,1);

    public override void Play(AudioSource source)
    {
        if (clips.Length == 0) return;

        int clipIndex = Random.Range(0, clips.Length);

        source.volume = Random.Range(volume.minValue, volume.maxValue) * Volumes.SFX;
        source.pitch = Random.Range(pitch.minValue, pitch.maxValue) * Time.timeScale;
        //source.Play();
        source.PlayOneShot(clips[clipIndex]);
    }


    public void Reset()
    {
        volume = new RangedFloat(1, 1);
        pitch = new RangedFloat(1, 1);
    }
}