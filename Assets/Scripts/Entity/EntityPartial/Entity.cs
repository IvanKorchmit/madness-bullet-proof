using UnityEngine;

public abstract partial class Entity : MonoBehaviour, IDamagable, IHitter
{

    [System.Flags]
    public enum EntityState : byte
    {
        IsKnockedOut = 1,
        IsWakingUp = 2,
        IsStunned = 4,
        hasAttacked = 8,
        IsImmune = 16,
    }

    [SerializeField] protected EntityState state;


    #region SFX
    [SerializeField] private AudioClip landSound;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip defaultPunchSound;
    [SerializeField] private AudioClip[] stepSounds;
    #endregion
    [SerializeField] private GameObject bloodParticle;
    public int Health
    {
        get
        {
            return health;
        }
        set
        {
            health = value;
        }
    }
    // public bool IsUndamagable => isKnockedOut || isWakingUp || immune;

    public bool IsUndamagable => FlagUtils.State_HasAnyOf(state, EntityState.IsKnockedOut, EntityState.IsWakingUp, EntityState.IsImmune);
    #region Movement
    #region Properties
    protected CharacterController2D Controller => controller;
    protected Animator EntityAnimator => animator;
    protected Rigidbody2D RB => rb;
    public AudioSource Audio => audioSource;
    [SerializeField] protected float speed;
    private bool isJumping;
    #region States
    public bool IsMoving => movement.x != 0 && controller.IsGrounded && !(FlagUtils.State_HasAnyOf(state, EntityState.IsKnockedOut, EntityState.IsWakingUp));
    public bool IsFalling => rb.velocity.y < 0 && !Controller.IsGrounded;
    #endregion
    #endregion
    private Vector2 movement;
    private bool crounch;
    #endregion
    #region Components
    private CharacterController2D controller;
    private AudioSource audioSource;
    private Animator animator;
    private Rigidbody2D rb;
    protected SpriteRenderer weaponVisuals;
    private SpriteRenderer visuals;
    [SerializeField] private Collider2D mainCollider;
    #endregion
    protected virtual void Start()
    {
        GetComponents();
        SubscribeToEvents();
        weaponVisuals = transform.Find("Weapon").GetComponentInChildren<SpriteRenderer>();
        weaponVisuals.enabled = currentWeapon != null;
        visuals = transform.Find("Visual").GetComponent<SpriteRenderer>();

        void SubscribeToEvents()
        {
            controller.OnLandEvent.AddListener(Entity_onEntityLand);
            controller.OnJumpEvent.AddListener(Entity_onEntityJump);
            onEntityAttack += Entity_onEntityAttack;
            onEntityPunch += Entity_onEntityPunch;
            onEntityStun += Entity_onEntityStun;
            onEntityRecover += Entity_onEntityRecover;
            onEntityKnockout += Entity_onEntityKnockout;
            onEntityWakeUp += Entity_onEntityWakeUp;
        }
        void GetComponents()
        {
            controller = GetComponent<CharacterController2D>();
            audioSource = GetComponent<AudioSource>();
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
        }
    }
    public void StepSound() => Audio.PlayOneShot(stepSounds[Random.Range(0, stepSounds.Length)]);

    #region Event Subscriptions

    #endregion
    protected virtual void Update()
    {
        if (!FlagUtils.State_Has(state, EntityState.IsKnockedOut, EntityState.IsStunned, EntityState.IsWakingUp))
        {
            controller.Move(movement.x * speed, crounch, isJumping);
        }
        if (!FlagUtils.State_Has(state, EntityState.IsWakingUp))
        {
            if (!IsFalling)
            {
                knockedOutTime -= Time.deltaTime;
                if (FlagUtils.State_Has(state, EntityState.IsKnockedOut) && knockedOutTime <= 0)
                {
                    Debug.Log("No KO");
                    NoKnockOut();
                }
            }
        }
    }
    public void UnlockMovement() => state = FlagUtils.State_Remove(state, EntityState.IsWakingUp);
    protected virtual void FixedUpdate()
    {
        animator.SetBool(EntityAnimationConsts.IS_MOVING, IsMoving);
        animator.SetBool(EntityAnimationConsts.IS_FALLING, IsFalling);
        if (IsFalling && !animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        {
            state = FlagUtils.State_Remove(state, EntityState.hasAttacked);
        }
        bool isKnockedOut = FlagUtils.State_Has(state, EntityState.IsKnockedOut);
        bool isWakingUp = FlagUtils.State_Has(state, EntityState.IsWakingUp);
        animator.SetBool(EntityAnimationConsts.IS_KNOCKED_OUT, isKnockedOut);
        animator.SetBool(EntityAnimationConsts.IS_WAKING_UP, isWakingUp);
        animator.SetBool(EntityAnimationConsts.HAS_FIREARM, currentWeapon != null && currentWeapon is Firearm);
    }

}