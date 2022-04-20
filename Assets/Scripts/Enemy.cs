using UnityEngine;

public class Enemy : Entity
{
    private float airTime;
    private int shoots;
    private bool canJump;
    [SerializeField] private float spotDistance;
    private float patrolDirection;
    private bool didShoot = true;
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }
    protected override void Start()
    {
        base.Start();
        onEntityLand += Enemy_onEntityLand;
        canJump = true;
        SetRandomPatrol();
    }

    private void Enemy_onEntityLand()
    {
        canJump = true;
        if (airTime > 2f)
        {
            Damage(this, (int)airTime);
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
        airTime += Time.deltaTime;
        if (target == null) return;
        if (canJump && Controller.IsGrounded && !IsMoving && target.transform.position.y > transform.position.y + 5f)
        {
            Jump(true);
            canJump = false;
        }
        else
        {
            Jump(false);
        }
        if (Vector2.Distance(target.transform.position, transform.position) <= spotDistance)
        {
            Move(transform.position.x >= target.transform.position.x ? -1f : 1f);
        }
        else
        {
            Move(patrolDirection);
            
        }
        Aim(-(transform.position - target.transform.position).normalized);
        if (CanAttack())
        {
            Attack(didShoot);
            didShoot = false;
        }
    }
    private bool CanAttack()
    {
        void ResetShoots()
        {
            shoots = 0;
        }
        if (Vector2.Distance(Player.Singleton.transform.position, transform.position) <= spotDistance)
        {
            if (shoots < 3)
            {
                return true;
            }
            else
            {
                TimerUtils.AddTimer(1f, ResetShoots);
            }
        }


        return false;
    }
    protected override void Entity_onEntityAttack()
    {
        base.Entity_onEntityAttack();
        shoots++;
    }
}
