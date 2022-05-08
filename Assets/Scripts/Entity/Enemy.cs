using UnityEngine;

public class Enemy : Entity
{
    private float airTime;
    private int shoots;
    private bool canJump;
    private bool hasSpotted;
    [SerializeField] private float spotDistance;
    private float patrolDirection;

    private LineOfSight los;

    protected override void FixedUpdate() => base.FixedUpdate();
    protected override void Start()
    {
        base.Start();
        onEntityLand += Enemy_onEntityLand;
        onEntityPunch += ()=> shoots++;
        onEntityAttack += ()=> shoots++;
        canJump = true;
        SetRandomPatrol();
        los = GetComponent<LineOfSight>();
        los.onPlayerSpot += ()=> hasSpotted = true;
        speed *= Random.Range(0.8f, 1.8f);
        Ammo = 25;
    }
    private void Enemy_onEntityLand()
    {
        canJump = true;
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
        Player target = Player.Singleton;
        base.Update();
        if (!Controller.IsGrounded)
        {
            airTime += Time.deltaTime;
        }
        if (target == null)
        {
            Move(patrolDirection);
            return;
        }
        TryJump(target);
        TryMove(target);
        if (CanAttack() && hasSpotted)
        {
            if (CurrentWeapon is Firearm)
            {
                Attack();
            }
            else if ((CurrentWeapon == null || CurrentWeapon is Melee) && Vector2.Distance(Player.Singleton.transform.position, transform.position) <= spotDistance / 10)
            {
                Attack();
            }
        }

        void TryJump(Player target)
        {
            
            if (hasSpotted && canJump && !IsMoving && target.transform.position.y > transform.position.y + 5f)
            {
                Jump(true);
                canJump = false;
            }
            else if (hasSpotted && canJump && !IsMoving && target.transform.position.y < transform.position.y)
            {
                JumpOff();
            }
            else
            {
                Jump(false);
            }
        }

        void TryMove(Player target)
        {
            if (hasSpotted)
            {
                Move(transform.position.x >= target.transform.position.x ? -1f : 1f);
                Aim(-(transform.position - target.transform.position).normalized);
            }
            else
            {
                Move(patrolDirection);
                Aim(new Vector2(patrolDirection, 0));
            }
        }
    }
    private bool CanAttack()
    {
        if (Vector2.Distance(Player.Singleton.transform.position, transform.position) <= spotDistance)
        {
            if (shoots < 3)
            {
                return true;
            }
            else
            {
                TimerUtils.AddTimer(1f, ()=> shoots = 0);
            }
        }
        return false;
    }
}
