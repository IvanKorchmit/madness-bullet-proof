using UnityEngine;
public partial class Entity
{
    protected void Move(float direction)
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsTag("NoMove"))
        {
            movement.x = 0;
            return;
        }
        movement.x = direction;
    }
    protected void Jump(bool isTrue)
    {
        if (FlagUtils.State_Has(state, EntityState.IsKnockedOut)) return;
        if (isTrue && controller.IsGrounded)
        {
            onEntityJump?.Invoke();
        }
        isJumping = isTrue;
    }

    protected void Aim(Vector2 direction)
    {
        void WeaponAim()
        {
            weaponVisuals.flipY = aimDirection.x < 0 || visuals.flipX;
            float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
            weaponVisuals.transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        bool isKnockedOut = FlagUtils.State_Has(state, EntityState.IsKnockedOut);
        bool isStunned = FlagUtils.State_Has(state, EntityState.IsStunned);
        if (isKnockedOut || isStunned) return;
        if (direction == Vector2.zero)
        {
            direction.y = 0;
            WeaponAim();
            return;
        }
        aimDirection = direction;
        WeaponAim();
    }
    public void Crounch(bool isTrue)
    {
        if (FlagUtils.State_HasAnyOf(state, EntityState.IsKnockedOut, EntityState.IsStunned)) return;

        crounch = isTrue;
    }
    protected void JumpOff()
    {
        var hits = Physics2D.OverlapBoxAll(transform.position, new Vector2(1, 2), 0);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].CompareTag("Platform"))
            {
                JumpOffAction(hits, i);
                break;
            }
            else if (hits[i].CompareTag("Glass")) hits[i].GetComponent<IDamagable>().Damage(this, 1);
        }

        void DisableColliders(Collider2D[] hits, int i)
        {
            foreach (var currentCollider in GetComponents<Collider2D>())
            {
                Physics2D.IgnoreCollision(currentCollider, hits[i], true);
            }
        }

        void JumpOffAction(Collider2D[] hits, int i)
        {
            Collider2D[] colliders = GetComponents<Collider2D>();
            void Back()
            {
                foreach (var currentCollider in GetComponents<Collider2D>())
                {
                    Physics2D.IgnoreCollision(currentCollider, hits[i], false);
                }
            }
            DisableColliders(hits, i);
            TimerUtils.AddTimer(1f, Back);
        }
    }
}