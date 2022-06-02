using UnityEngine;

public abstract partial class Entity : MonoBehaviour, IDamagable, IHitter
{
    [SerializeField] private WeaponBase currentWeapon;
    protected WeaponBase CurrentWeapon
    {
        get
        {
            return currentWeapon;
        }
        set
        {
            currentWeapon = value;
        }
    }
    [SerializeField] protected LayerMask meleeLayerMask;
    private Vector2 aimDirection;
    private int ammo;
    public int Ammo
    {
        get
        {
            return ammo;
        }
        protected set
        {
            ammo = value;
        }
    }
    public int MeleeMask => meleeLayerMask;
    [SerializeField] private int stamina;



    public void Punch()
    {
        if (FlagUtils.State_Has(state, EntityState.IsKnockedOut)) return;
        state = FlagUtils.State_Remove(state, EntityState.hasAttacked);
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, 2f, movement, 1f, meleeLayerMask);
        DamageTargets(hits);

        void DamageTargets(RaycastHit2D[] hits)
        {
            foreach (var hit in hits)
            {
                if (hit.collider != null && hit.collider.TryGetComponent(out IDamagable damage) && damage.Damage(this, 1))
                {
                    onEntityPunch?.Invoke();
                }
            }
        }
    }





    [SerializeField] private int health = 10;
    private float knockedOutTime;
    private int numberOfHits;
    private int numberOfHitsStunned;
    public bool Damage(IHitter damager, int damage)
    {
        void Stun()
        {
            state = FlagUtils.State_Add(state, EntityState.IsStunned);
            onEntityStun?.Invoke();
        }
        void Death()
        {
            gameObject.SetActive(false);
            KnockOut(damager, this is Player ? 0.5f : 1f);
        }
        bool isKnockedOut = FlagUtils.State_Has(state, EntityState.IsKnockedOut);
        if (!isKnockedOut || !(damager is Entity ent) || ent == this)
        {
            if (IsUndamagable) return false;
            numberOfHits += damage;
            health -= damage;
            if (numberOfHits >= stamina)
            {
                KnockOut(damager, this is Player ? 0.5f : 1f);
            }
            else
            {
                Stun();
            }
            if (health <= 0)
            {
                Death();
            }
            Instantiate(bloodParticle, transform.position, Quaternion.identity);
            return true;
        }
        if (numberOfHitsStunned < 4)
        {
            Push(ent);
            numberOfHitsStunned++;
            Instantiate(bloodParticle, transform.position, Quaternion.identity);

            return true;
        }
        return false;
    }

    public void InstantKill() => Damage(this, 9999);
    public void ResetStun()
    {
        state = FlagUtils.State_Remove(state, EntityState.IsStunned);
        onEntityRecover?.Invoke();
    }
    private void KnockOut(IHitter damager, float time)
    {
#if UNITY_EDITOR
        try
        {
#endif
            knockedOutTime = !FlagUtils.State_Has(state, EntityState.IsKnockedOut) ? time : knockedOutTime;
            numberOfHits = 0;
            state = FlagUtils.State_Remove(state, EntityState.IsWakingUp);
            state = FlagUtils.State_Add(state, EntityState.IsKnockedOut);

            Push(damager);
            onEntityKnockout?.Invoke();
#if UNITY_EDITOR
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Oops! " + ex.Message + " " + ex.StackTrace);
        }
#endif
    }

    private void Push(IHitter damager)
    {
        Vector2 direction = transform.position - damager.transform.position;
        rb.velocity = new Vector2();
        rb.position += Vector2.up * 0.8f;
        rb.AddForce(new Vector2((direction.x >= 0f ? 1f : -1f) * 5f, 10f), ForceMode2D.Impulse);
    }

    public void NoKnockOut()
    {
        state = FlagUtils.State_Remove(state, EntityState.IsKnockedOut);
        state = FlagUtils.State_Add(state, EntityState.IsWakingUp);
        numberOfHitsStunned = 0;
#if UNITY_EDITOR
        try
        {
#endif
            onEntityWakeUp?.Invoke();
#if UNITY_EDITOR
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Oops! " + ex.Message + " " + ex.StackTrace);
        }
#endif
    }
    protected void Attack()
    {
        if (FlagUtils.State_HasAnyOf(state, EntityState.IsStunned, EntityState.IsKnockedOut)) return;
        void PerformAttack()
        {
            if (currentWeapon != null && currentWeapon is Firearm && ammo <= 0)
            {
                currentWeapon = null;
                weaponVisuals.enabled = false;
                return;
            }
            currentWeapon?.Attack(this, aimDirection);
            onEntityAttack?.Invoke();
            if (currentWeapon is Firearm) ammo--;
        }
        if (currentWeapon != null)
        {
            TimerUtils.AddTimer(currentWeapon.Cooldown, PerformAttack);
        }
        else
        {
            bool hasAttackedaAndWakingUp = FlagUtils.State_Has(state, EntityState.hasAttacked, EntityState.IsWakingUp);
            bool IsStunnedOrKOed = FlagUtils.State_HasAnyOf(state, EntityState.IsStunned, EntityState.IsKnockedOut);
            if (!hasAttackedaAndWakingUp && !IsFalling && !IsStunnedOrKOed)
            {
                animator.SetTrigger(EntityAnimationConsts.ATTACK_TRIGGER);
                state = FlagUtils.State_Add(state, EntityState.hasAttacked);
            }
        }
    }
}
