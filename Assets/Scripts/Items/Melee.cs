using UnityEngine;

[CreateAssetMenu(fileName = "New Melee")]
public class Melee : WeaponBase
{
    [SerializeField] private AudioClip meleeSwing;
    [SerializeField] private int damage;
    public override void Attack(Entity owner, Vector2 direction)
    {
        owner.WeaponAnimator.SetTrigger("Attack");
        owner.EntityAnimator.SetTrigger("Attack");
        RaycastHit2D hit = Physics2D.CircleCast(owner.transform.position, 3, direction, 1.5f, owner.Melee);
        if (hit.collider != null && hit.collider.TryGetComponent(out IDamagable damage))
        {
            if (damage.Damage(owner, this.damage))
            {
                owner.Audio.PlayOneShot(AttackSound);
                return;
            }
        }
        owner.Audio.PlayOneShot(meleeSwing);
    }
}