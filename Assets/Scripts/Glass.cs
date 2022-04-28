using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glass : MonoBehaviour, IDamagable
{
    [SerializeField] private GameObject glassShards;
    private BoxCollider2D boxCollider;
    private bool isBroken;
    public bool IsVulnerable => false;
    public void InstantKill()
    {
        Break();
    }
    // Start is called before the first frame update
    void Start()
    {
        BoxCollider2D[] colliders = GetComponents<BoxCollider2D>();
        SpriteRenderer sp = GetComponent<SpriteRenderer>();
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].size = sp.size;
            if (colliders[i].isTrigger)
            {
                boxCollider = colliders[i];
            }
        }
    }
    private void Break()
    {
        if (isBroken) return;
        AudioSource src = GetComponentInChildren<AudioSource>();
        src.transform.SetParent(null);
        src.Play();
        Instantiate(glassShards, transform.position, Quaternion.identity);
        Object.Destroy(src.gameObject, src.clip.length);
        Destroy(gameObject, src.clip.length);
        gameObject.SetActive(false);
        isBroken = true;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Player"))
        {
            if (collision.attachedRigidbody.velocity.magnitude > 12f)
            {
                Debug.Log(collision.attachedRigidbody.velocity.magnitude);
                Break();
            }
            else
            {
                boxCollider.isTrigger = false;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            boxCollider.isTrigger = true;
        }
    }
    public bool Damage(Entity damager, int damage)
    {

        Break();
        return true;
    }
}
