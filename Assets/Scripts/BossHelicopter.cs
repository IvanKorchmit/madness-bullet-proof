using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class BossHelicopter : MonoBehaviour, IDamagable, IHitter
{

    public UnityEvent onHelicopterDeath;






    private bool alreadyDying;
    public bool undamagable;
    private bool alreadyInRage;

    [field: SerializeField] public bool IsUndamagable { get; set; }
    [field: SerializeField] public int Health { get; set; } = 50;

    private Animator animator;
    [SerializeField] private GameObject projectile;
    [SerializeField] private GameObject missle;
    [SerializeField] private GameObject[] enemies;
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private Transform[] explosionTransforms;
    private int initHealth;
    private void Start()
    {
        animator = GetComponent<Animator>();
        GetComponentInChildren<ParticleSystem>().Pause();
        initHealth = Health;

    }

    public bool Damage(IHitter damager, int damage)
    {
        if (IsUndamagable)
        {
            return false;
        }
        Health -= damage;
        if (!alreadyInRage && Health <= initHealth / 4)
        {
            alreadyInRage = true;
            animator.SetTrigger("Rage");
        }
        if (Health <= 0 && !alreadyDying)
        {
            GetComponentInChildren<ParticleSystem>().Play();
            alreadyDying = true;
            onHelicopterDeath?.Invoke();
        }
        return true;
    }
    private void Update() => animator.SetInteger("Health", Health);

    public void InstantKill() { return; }

    public void SpawnEnemy() => Instantiate(enemies[Random.Range(0, enemies.Length)], transform.position, Quaternion.identity);
    public void ShootRight() => Shoot(Vector2.right, projectile);
    public void ShootMissle() => Shoot(Vector2.right, missle);
    public void ShootDuwn()
    {
        Shoot(Vector2.down, projectile);
    }
    private void Shoot(Vector2 direction, GameObject prefab)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Transform sp = transform.Find("ShootPoint");
        Instantiate(prefab, sp.position, Quaternion.Euler(0,0,angle)).GetComponent<Projectile>().Init(this);
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