using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHelicopter : MonoBehaviour, IDamagable, IHitter
{
    public bool undamagable;
    [SerializeField] private int health = 50;
    public bool IsUndamagable => undamagable;
    private Animator animator;
    [SerializeField] private GameObject projectile;
    [SerializeField] private GameObject[] enemies;
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private Transform[] explosionTransforms;
    private void Start()
    {
        animator = GetComponent<Animator>();
        GetComponentInChildren<ParticleSystem>().Pause();

    }

    public bool Damage(IHitter damager, int damage)
    {
        if (IsUndamagable)
        {
            return false;
        }
        health -= damage;
        if (health <= 0)
        {
            GetComponentInChildren<ParticleSystem>().Play();
        }
        return true;
    }
    private void Update()
    {
        animator.SetInteger("Health", health);
    }

    public void InstantKill()
    {
        return;
    }

   public void SpawnEnemy()
    {
        Instantiate(enemies[Random.Range(0, enemies.Length)], transform.position, Quaternion.identity);
    }
    public void ShootRight()
    {
        Shoot(Vector2.right);
    }
    public void ShootDuwn()
    {
        Shoot(Vector2.down);
    }
    private void Shoot(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Transform sp = transform.Find("ShootPoint");
        Instantiate(projectile, sp.position, Quaternion.Euler(0,0,angle)).GetComponent<Projectile>().Init(this);
    }
    public void Explosion()
    {
        var e = Instantiate(explosionPrefab, explosionTransforms[Random.Range(0, explosionTransforms.Length)]);
        e.transform.localPosition = new Vector2();
    }
    public void OnDestroy()
    {
        AudioSource[] sources = GetComponentsInChildren<AudioSource>();
        foreach (var item in sources)
        {
            item.transform.SetParent(null);
        }
        transform.Find("Fire").SetParent(null);
    }
}