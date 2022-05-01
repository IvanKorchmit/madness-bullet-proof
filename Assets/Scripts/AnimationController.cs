using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class AnimationController : MonoBehaviour
{
    public UnityEvent onAnimationEnd;
    [SerializeField] private AudioSource reference;
    [SerializeField] private GameObject explosionPrefab;
    public void Play(AudioClip clip)
    {
        reference.Stop();
        reference.PlayOneShot(clip);
    }
    public void PlayContinious(AudioClip clip)
    {
        reference.PlayOneShot(clip);
    }
    public void SpawnExplosion(string name)
    {
        Transform reference = transform.Find(name);
        var instance = Instantiate(explosionPrefab, transform.root);
        instance.transform.position = reference.position;
    }
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
    public void Trigger(string name)
    {
        GetComponent<Animator>().SetTrigger(name);
    }
    public void EndEvent()
    {
        onAnimationEnd?.Invoke();
    }
}
