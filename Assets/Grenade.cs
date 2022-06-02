using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour, IHitter
{
    [SerializeField] private GameObject explosion;
    [SerializeField] private GameObject effector;
    private bool hasAlreadyExplode;
    public bool IsUndamagable => false;

    // Start is called before the first frame update
    void Start()
    {
        TimerUtils.AddTimer(5, Explode);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Player p = Player.Singleton;
        if (p == null) return;
        if (collision.collider && collision.collider.gameObject != p.gameObject && collision.collider.TryGetComponent<IDamagable>(out IDamagable non))
        {
            Explode();
        }
    }
    private void Explode()
    {
        if (hasAlreadyExplode) return;
        var hits = Physics2D.CircleCastAll(transform.position, 5, new Vector2(),0f, LayerMask.GetMask("Enemy"));
        foreach (var h in hits)
        {
            Player p = Player.Singleton;
            if (p == null) continue;
            if (h.collider && h.collider.gameObject != p.gameObject && h.collider.TryGetComponent(out IDamagable damage))
            {
                damage.Damage(this, 5);
            }
        }
            var eff = Instantiate(effector, transform.position, Quaternion.identity);
            Object.Destroy(eff, 0.5f);
            Instantiate(explosion, transform.position, Quaternion.identity);
        hasAlreadyExplode = true;
        Destroy(gameObject, 0.5f);
    }

    public bool Damage(IHitter damager, int damage)
    {
        return false;
    }

    public void InstantKill()
    {
        return;
    }
}
