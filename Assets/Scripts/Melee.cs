﻿using UnityEngine;

[CreateAssetMenu(fileName = "New Melee")]
public class Melee : WeaponBase
{
    [SerializeField] private AudioClip meleeHit;
    [SerializeField] private AudioClip meleeSwing;
    [SerializeField] private int damage;
    public override void Attack(Entity owner, Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.CircleCast((Vector2)owner.transform.position, 3, direction, 1.5f, owner.MeleeMask);
        Debug.Log(hit.collider);
        if (hit.collider != null)
        {
            if (hit.collider.TryGetComponent(out IDamagable damage))
            {
                damage.Damage(owner, this.damage);
                owner.Audio.PlayOneShot(meleeHit);
            }
            else
            {
                owner.Audio.PlayOneShot(meleeSwing);
            }
        }
        else
        {
            owner.Audio.PlayOneShot(meleeSwing);
        }
    }
}