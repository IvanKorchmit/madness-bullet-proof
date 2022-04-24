using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glass : MonoBehaviour, IDamagable
{
    [SerializeField] private GameObject glassShards;
    private bool isBroken;
    public bool IsVulnerable => throw new System.NotImplementedException();
    public void InstantKill()
    {
        Break();
    }
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<BoxCollider2D>().size = GetComponent<SpriteRenderer>().size;
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
            Break();
        }
    }

    public bool Damage(Entity damager, int damage)
    {

        Break();
        return true;
    }
}
