using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public abstract class Entity : MonoBehaviour, IDamagable, IHitter
{
    public event System.Action OnEntityLand;
    public event System.Action OnEntityJump;
    public event System.Action OnEntityAttack;
    public event System.Action OnEntityKnockout;
    public event System.Action OnEntityWakeUp;

    [field: SerializeField] public LayerMask Melee { get; private set; }


    public bool IsMoving => Controller.IsMoving;
    public int Ammo => CurrentWeapon.Ammo;
    public WeaponBase WeaponBase => CurrentWeapon.Base;



    public const string IS_MOVING = "isMoving";
    public const string IS_FALLING = "isFalling";
    public const string IS_KNOCKED_OUT = "isKnockedOut";
    public const string IS_WAKING_UP = "isWakingUp";
    public const string HAS_FIREARM = "hasFirearm";
    public const string JUMP_TRIGGER = "Jump";
    public const string ATTACK_TRIGGER = "Attack";
    public const string STUN_TRIGGER = "Stun";
    public const string DAMAGE_TRIGGER = "Damage";
    public const string LAND_TRIGGER = "Land";
    public const string KNOCKOUT_TRIGGER = "Knockout";
    public const string WAKE_UP_TRIGGER = "WakeUp";
    public const string RECOVER_TRIGGER = "Recover";

    [System.Serializable]
    public class Weapon
    {
        [SerializeField] private Melee fists;
        [field:SerializeField]
        public int Ammo { get; set; }
        public Entity Owner { get; set; }
        [SerializeField] private WeaponBase @base;
        public WeaponBase Base
        {
            get
            {
                return @base == null ? fists : @base;
            }
            set
            {
                @base = value;
            }
        }
        public void SetWeapon(WeaponBase weapon, int ammo)
        {
            Ammo = ammo;
            Base = weapon;
        }

        public void Use()
        {
            if (Base is Firearm && Ammo > 0)
            {
                Base.Attack(Owner, Owner.aimDirection);
                Ammo--;
            }
            else if (Base != null)
            {
                Owner.EntityAnimator.SetTrigger(ATTACK_TRIGGER);
                Base.Attack(Owner, Owner.aimDirection);

            }
            else
            {
                Owner.EntityAnimator.SetTrigger(ATTACK_TRIGGER);

                fists.Attack(Owner, Owner.aimDirection);
            }
        }
    }
    [field: SerializeField] protected Weapon CurrentWeapon { get; private set; }
    public CharacterController2D Controller { get; private set; }
    [field: SerializeField] public SpriteRenderer Visuals { get; private set; }
    [field: SerializeField] public SpriteRenderer WeaponVisuals { get; private set; }
    [field: SerializeField] public Animator WeaponAnimator { get; private set; }
    public Rigidbody2D RB { get; private set; }
    public Animator EntityAnimator { get; private set; }
    public AudioSource Audio { get; private set; }
    [field: SerializeField] protected float Speed { get; set; }
    public int Stamina { get; protected set; }

    private bool isStunned = false;
    public bool IsFalling => RB.velocity.y < 0 && !Controller.IsGrounded;

    [SerializeField] private AudioEvent steps;


    public abstract bool IsUndamagable { get; set; }

    private Vector2 aimDirection;

    [field: SerializeField] public bool CanMove { get; set; }
    [field: SerializeField] public int Health { get; set; }

    protected virtual void Start()
    {
        Controller = GetComponent<CharacterController2D>();
        RB = GetComponent<Rigidbody2D>();
        EntityAnimator = GetComponent<Animator>();
        Audio = GetComponent<AudioSource>();
        Controller.OnLandEvent.AddListener(() => OnEntityLand?.Invoke());
        WeaponAnimator = WeaponVisuals.GetComponent<Animator>();
        OnEntityLand += Entity_OnEntityLand;
        CurrentWeapon.Owner = this;

    }

    private void Entity_OnEntityLand()
    {
        EntityAnimator.SetTrigger(LAND_TRIGGER);
    }

    protected virtual void Update()
    {
        if (WeaponAnimator != CurrentWeapon.Base.Animation)
        {
            WeaponAnimator.runtimeAnimatorController = CurrentWeapon.Base.Animation;
        }
        EntityAnimator.SetBool(IS_MOVING, IsMoving);
        EntityAnimator.SetBool(IS_FALLING, IsFalling);
        if (Health <= 0)
        {
            Destroy(gameObject, 5.0f);
            KO();
        }
    }
    protected void Move(float direction)
    {
        if (!CanMove)
        {
            return;
        }
        Controller.Move(direction * Speed, false, false);
    }
    protected void Jump()
    {
        EntityAnimator.SetTrigger(JUMP_TRIGGER);
        Controller.Move(0, false, true);
    }
    public abstract bool Damage(IHitter damager, int damage);
    public abstract void InstantKill();
    public void StepSound()
    {
        steps.Play(Audio);
    }
    protected void JumpOff()
    {
        var hits = Physics2D.OverlapBoxAll(transform.position, new Vector2(1, 2), 0);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].CompareTag("Platform"))
            {
                ColliderUtils.JumpOffAction(GetComponents<Collider2D>(),hits, i);
                break;
            }
            else if (hits[i].CompareTag("Glass")) hits[i].GetComponent<IDamagable>().Damage(this, 1);
        }

    }
    
    protected void Aim(Vector2 direction)
    {
        void WeaponAim()
        {
            if (!CurrentWeapon.Base.IsAimable)
            {
                WeaponVisuals.flipX = Visuals.flipX;
                return;
            }
                WeaponVisuals.flipY = aimDirection.x < 0 || Visuals.flipX;
            WeaponVisuals.flipX = false;
            float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
            WeaponVisuals.transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        if (!CanMove) return;
        if (direction == Vector2.zero)
        {
            direction.y = 0;
            WeaponAim();
            return;
        }
        aimDirection = direction;
        WeaponAim();
    }
    protected void KO()
    {
        if (isStunned) return;
        WeaponVisuals.enabled = false;
        OnEntityKnockout?.Invoke();
        CanMove = false;
        EntityAnimator.SetBool(IS_KNOCKED_OUT, true);
        EntityAnimator.SetTrigger(KNOCKOUT_TRIGGER);
        RB.AddForce(Vector2.up * 10 + new Vector2(0,-RB.velocity.normalized.x) * 3.5f, ForceMode2D.Impulse);
        isStunned = true;

    }
    public void OnWakeUp()
    {
        OnEntityWakeUp?.Invoke();
        WeaponVisuals.enabled = true;
        isStunned = false;
        CanMove = true;
        Stamina = 3;
        EntityAnimator.ResetTrigger(STUN_TRIGGER);
        EntityAnimator.SetBool(IS_KNOCKED_OUT, false);
    }
}
