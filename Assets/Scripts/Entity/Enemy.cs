using UnityEngine;

public class Enemy : Entity
{
    private float airTime;
    private int shoots;
    private bool hasSpotted;
    [SerializeField] private float spotDistance;
    private float patrolDirection;

    private LineOfSight los;
    [field: SerializeField] public override bool IsUndamagable { get; set; }

    protected override void Start()
    {
        base.Start();
        Stamina = 3;
        OnEntityLand += Enemy_onEntityLand;
        OnEntityAttack += ()=> shoots++;
        SetRandomPatrol();
        los = GetComponent<LineOfSight>();
        los.onPlayerSpot += ()=> hasSpotted = true;
        Speed *= Random.Range(0.8f, 1.8f);
        CurrentWeapon.Ammo = 25;
    }
    private void Enemy_onEntityLand()
    {
        if (airTime > 4f)
        {
            Damage(this, (int)airTime / 4);
        }
        airTime = 0;
    }
    private void SetRandomPatrol()
    {
        patrolDirection = 0;
        TimerUtils.AddTimer(0.5f,()=>patrolDirection = Random.value >= 0.5f ? 1 : Random.value <= 0.25f ? 0 : -1);
        TimerUtils.AddTimer(Random.value * 2f, SetRandomPatrol);
    }
    protected override void Update()
    {
        base.Update();
        var p = Player.Singleton;
        Move(!hasSpotted || p == null ? patrolDirection : (p.transform.position - transform.position).normalized.x);
        if (hasSpotted && p != null && p.transform.position.y > transform.position.y + 10)
        {
            if (p.transform.position.y > transform.position.y + 10)
            {
                Jump();
            }
            TimerUtils.AddTimer(CurrentWeapon.Base.Cooldown, CurrentWeapon.Use);
        }
        if (Stamina <= 0)
        {
            KO();
        }
    }

    public override bool Damage(IHitter damager, int damage)
    {
        
        if (IsUndamagable && EntityAnimator.GetBool(IS_KNOCKED_OUT)) return false;
        Visuals.flipX = damager.transform.position.x > transform.position.x;
        CanMove = false;
        EntityAnimator.SetTrigger(STUN_TRIGGER);
        Stamina -= damage;
        Health -= damage;
        return true;
    }

    public override void InstantKill()
    {
        throw new System.NotImplementedException();
    }
}
