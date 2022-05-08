using UnityEngine;



public partial class Entity
{
    public event System.Action onEntityLand;
    public event System.Action onEntityJump;
    public event System.Action onEntityAttack;
    public event System.Action onEntityPunch;
    public event System.Action onEntityKnockout;
    public event System.Action onEntityWakeUp;
    public event System.Action onEntityStun;
    public event System.Action onEntityRecover;










    private void Entity_onEntityWakeUp()
    {
        animator.SetTrigger(EntityAnimationConsts.WAKE_UP_TRIGGER);
        Debug.Log("Waking up");
        state = FlagUtils.State_Remove(state, EntityState.IsWakingUp, EntityState.IsStunned, EntityState.hasAttacked, EntityState.IsKnockedOut);
        TimerUtils.AddTimer(0.5f, () =>
        {
            weaponVisuals.enabled = CurrentWeapon != null;
        });
    }

    private void Entity_onEntityKnockout()
    {
        state = FlagUtils.State_Remove(state, EntityState.IsWakingUp, EntityState.IsStunned, EntityState.hasAttacked);
        animator.SetTrigger(EntityAnimationConsts.KNOCKOUT_TRIGGER);

    }


    private void Entity_onEntityRecover()
    {
        state = FlagUtils.State_Remove(state, EntityState.IsWakingUp, EntityState.IsStunned, EntityState.hasAttacked);
        animator.SetTrigger(EntityAnimationConsts.RECOVER_TRIGGER);
    }

    private void Entity_onEntityStun()
    {
        if (!FlagUtils.State_Has(state, EntityState.IsKnockedOut))
        {
            animator.SetTrigger(EntityAnimationConsts.STUN_TRIGGER);
            state = FlagUtils.State_Remove(state, EntityState.hasAttacked, EntityState.IsWakingUp);
        }
    }

    private void Entity_onEntityAttack()
    {
        audioSource.PlayOneShot(currentWeapon.ShootSound);
        state = FlagUtils.State_Remove(state, EntityState.IsStunned);
    }

    private void Entity_onEntityPunch()
    {
        state = FlagUtils.State_Remove(state, EntityState.hasAttacked, EntityState.IsWakingUp, EntityState.IsStunned);
        audioSource.PlayOneShot(defaultPunchSound);
    }

    private void Entity_onEntityLand()
    {
        if (FlagUtils.State_Has(state, EntityState.IsKnockedOut))
        {
            rb.velocity = new Vector2();
        }
        audioSource.PlayOneShot(landSound);
        animator.SetTrigger(EntityAnimationConsts.LAND_TRIGGER);
        onEntityLand?.Invoke();
    }

    protected virtual void Entity_onEntityJump()
    {
        audioSource.PlayOneShot(jumpSound);
        onEntityJump?.Invoke();
        state = FlagUtils.State_Remove(state, EntityState.hasAttacked);
        animator.SetTrigger(EntityAnimationConsts.JUMP_TRIGGER);
    }

}
