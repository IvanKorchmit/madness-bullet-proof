using UnityEngine;
public abstract class Entity : MonoBehaviour, IDamagable
{
    #region SFX
    [SerializeField] private AudioClip landSound;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip defaultPunchSound;
    [SerializeField] private AudioClip[] stepSounds;
    #endregion

    [SerializeField] private GameObject bloodParticle;

    #region Animation const
    public const string IS_MOVING = "isMoving";
    public const string IS_FALLING = "isFalling";
    public const string IS_KNOCKED_OUT = "isKnockedOut";
    public const string HAS_FIREARM = "hasFirearm";
    public const string JUMP_TRIGGER = "Jump";
    public const string ATTACK_TRIGGER = "Attack";
    public const string STUN_TRIGGER = "Stun";
    public const string DAMAGE_TRIGGER = "Damage";
    public const string LAND_TRIGGER = "Land";
    public const string KNOCKOUT_TRIGGER = "Knockout";
    public const string WAKEUP_TRIGGER = "WakeUp";
    public const string RECOVER_TRIGGER = "Recover";
    #endregion

    public bool IsVulnerable => isKnockedOut;
    private bool isStunned;
    private bool hasAttacked;
    public bool IsStunned => isKnockedOut || isStunned;
    #region Combat
    [SerializeField] private WeaponBase currentWeapon;
    public WeaponBase CurrentWeapon => currentWeapon;
    [SerializeField] private LayerMask meleeLayerMask;
    private Vector2 aimDirection;
    private int ammo;
    public int MeleeMask => meleeLayerMask;

    #endregion
    [SerializeField] private int stamina;
    #region Action Events
    public event System.Action onEntityLand;
    public event System.Action onEntityJump;
    public event System.Action onEntityAttack;
    public event System.Action onEntityPunch;
    public event System.Action onEntityKnockout;
    public event System.Action onEntityWakeUp;
    public event System.Action onEntityStun;
    public event System.Action onEntityRecover;
    #endregion
    #region Movement
    #region Properties
    protected CharacterController2D Controller => controller;
    protected Animator EntityAnimator => animator;
    public AudioSource Audio => audioSource;
    [SerializeField] private float speed;
    private bool isJumping;
    #region States
    public bool IsMoving => movement.x != 0 && controller.IsGrounded && !(isStunned || isWakingUp || isKnockedOut || IsFalling);
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
    private SpriteRenderer weaponVisuals;
    [SerializeField] private Collider2D mainCollider;
    #endregion
    #region Mandatory Methods
    protected virtual void Start()
    {
        controller = GetComponent<CharacterController2D>();
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        controller.OnLandEvent.AddListener(Entity_onEntityLand);
        controller.OnJumpEvent.AddListener(Entity_onEntityJump);
        onEntityAttack += Entity_onEntityAttack;
        onEntityPunch += Entity_onEntityPunch;
        onEntityStun += Entity_onEntityStun;
        onEntityRecover += Entity_onEntityRecover;
        onEntityKnockout += Entity_onEntityKnockout;
        onEntityWakeUp += Entity_onEntityWakeUp;
        weaponVisuals = transform.Find("Weapon").GetComponentInChildren<SpriteRenderer>();
    }

    #region Event Subscriptions
    public void StepSound()
    {
        Audio.PlayOneShot(stepSounds[Random.Range(0, stepSounds.Length)]);
    }
    private void Entity_onEntityWakeUp()
    {
        animator.SetTrigger(WAKEUP_TRIGGER);
        isKnockedOut = false;
        isStunned = false;
        isWakingUp = false;
        hasAttacked = false;
    }

    private void Entity_onEntityKnockout()
    {
        isWakingUp = false;
        hasAttacked = false;
        isStunned = false;
        animator.SetTrigger(KNOCKOUT_TRIGGER);

    }

    protected void JumpOff()
    {


        var hits = Physics2D.OverlapBoxAll(transform.position, new Vector2(1, 2), 0);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].CompareTag("Platform"))
            {
                var colliders = GetComponents<Collider2D>();
                void Back()
                {
                    foreach (var currentCollider in GetComponents<Collider2D>())
                    {
                        Physics2D.IgnoreCollision(currentCollider, hits[i], false);
                    }
                }
                foreach (var currentCollider in GetComponents<Collider2D>())
                {
                    Physics2D.IgnoreCollision(currentCollider, hits[i], true);
                }
                TimerUtils.AddTimer(1f, Back);
                break;
            }
        }
    }

    private void Entity_onEntityRecover()
    {
        hasAttacked = false;
        isStunned = false;
        isWakingUp = false;
        animator.SetTrigger(RECOVER_TRIGGER);
    }

    private void Entity_onEntityStun()
    {
        animator.SetTrigger(STUN_TRIGGER);
        isWakingUp = false;
        hasAttacked = false;
    }

    protected virtual void Entity_onEntityAttack()
    {
        audioSource.PlayOneShot(currentWeapon.ShootSound);
        isStunned = false;
    }

    private void Entity_onEntityPunch()
    {
        isStunned = false;
        isWakingUp = false;
        hasAttacked = false;    
        audioSource.PlayOneShot(defaultPunchSound);
    }

    private void Entity_onEntityLand()
    {
        if (IsStunned)
        {
            rb.velocity = new Vector2();
        }
        audioSource.PlayOneShot(landSound);
        animator.SetTrigger(LAND_TRIGGER);
        onEntityLand?.Invoke();
    }

    protected virtual void Entity_onEntityJump()
    {
        audioSource.PlayOneShot(jumpSound);
        onEntityJump?.Invoke();
        hasAttacked = false;
        animator.SetTrigger(JUMP_TRIGGER);
    }

    #endregion
    protected virtual void Update()
    {
        if (!IsStunned && !isWakingUp)
        {
            controller.Move(movement.x * speed, crounch, isJumping);
        }
        else if (!isWakingUp)
        {
            controller.Move(0, false, false);
            if (!IsFalling)
            {
                knockedOutTime -= Time.deltaTime;
                if (isKnockedOut && knockedOutTime <= 0)
                {
                    NoKnockOut();
                }
            }
        }
    }
    public void UnlockMovement()
    {
        isWakingUp = false;
    }
    protected virtual void FixedUpdate()
    {
        animator.SetBool(IS_MOVING, IsMoving);
        animator.SetBool(IS_FALLING, IsFalling);
        if (IsFalling && !animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        {
            hasAttacked = false;
        }
        animator.SetBool(IS_KNOCKED_OUT, isKnockedOut);
        animator.SetBool(HAS_FIREARM, currentWeapon != null && currentWeapon is Firearm);
        if (currentWeapon == null)
        {
            weaponVisuals.sprite = null;
        }
    }
    #endregion
    #region Movement Methods
    /// <summary>
    /// Forces character to walk the desired direction
    /// </summary>
    /// <param name="direction"></param>
    protected void Move(float direction)
    {
        if (IsStunned || isWakingUp || animator.GetCurrentAnimatorStateInfo(0).IsTag("NoMove"))
        {
            movement.x = 0;
            return;
        }
        movement.x = direction;
    }
    /// <summary>
    /// Makes the character perform jumping
    /// </summary>
    protected void Jump(bool isTrue)
    {
        if (isKnockedOut) return;
        if (isTrue && controller.IsGrounded)
        {
            onEntityJump?.Invoke();
        }
        isJumping = isTrue;
    }
    public void Punch()
    {
        if (isKnockedOut) return;
        hasAttacked = false;
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, 2f, movement, 1f, meleeLayerMask);
        if (hit.collider != null)
        {
            if (hit.collider.TryGetComponent(out IDamagable damage))
            {
                if (damage.Damage(this, 1))
                {
                    onEntityPunch?.Invoke();
                }
            }
        }
    }
    protected void Attack()
    {
        if (IsStunned) return;
        void PerformAttack()
        {
            if (IsStunned) return;
            currentWeapon?.Attack(this, aimDirection);
            onEntityAttack?.Invoke();
        }
        if (currentWeapon != null)
        {

                TimerUtils.AddTimer(currentWeapon.Cooldown, PerformAttack);
        }
        else
        {
            if (!hasAttacked && !IsFalling && !IsStunned && !isWakingUp)
            {
                animator.SetTrigger(ATTACK_TRIGGER);
                hasAttacked = true;
            }
        }
    }
    protected void Aim(Vector2 direction)
    {
        void WeaponAim()
        {
            weaponVisuals.flipY = aimDirection.x < 0;
            float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
            weaponVisuals.transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        if (IsStunned) return;

        if (direction == Vector2.zero)
        {
            aimDirection.y = 0;
            WeaponAim();
            return;
        }
        aimDirection = direction;
        WeaponAim();
    }
    public void Crounch(bool isTrue)
    {
        if (IsStunned) return;

        crounch = isTrue;
    }
    #endregion
    [SerializeField] private int health = 10;
    private bool isKnockedOut;
    private bool isWakingUp;
    private float knockedOutTime;
    private int numberOfHits;
    private int numberOfHitsStunned;
    public bool Damage(Entity damager, int damage)
    {
        void Death ()
        {
            Destroy(gameObject,1.75f);
            KnockOut(damager);

        }
        if (IsStunned && damager != this)
        {
            if (numberOfHitsStunned < 4)
            {
                Push(damager);
                numberOfHitsStunned++;
                Instantiate(bloodParticle, transform.position, Quaternion.identity);

                return true;
            }
            return false;
        }
        numberOfHits += damage;
        health -= damage;
        if (numberOfHits >= stamina)
        {
            KnockOut(damager);
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
    private void Stun()
    {
        isStunned = true;
        onEntityStun?.Invoke();
    }
    public void ResetStun()
    {
        isStunned = false;
        onEntityRecover?.Invoke();
    }
    private void KnockOut(Entity damager)
    {
#if UNITY_EDITOR
        try
        {
#endif
            isWakingUp = false;
            knockedOutTime =  !isKnockedOut ? 3f : knockedOutTime;
            numberOfHits = 0;
            isKnockedOut = true;
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

    private void Push(Entity damager)
    {
        Vector2 direction = transform.position - damager.transform.position;
        rb.velocity = new Vector2();
        rb.position += Vector2.up * 1.5f;
        rb.AddForce(new Vector2((direction.x >= 0f ? 1f : -1f) * 5f, 10f), ForceMode2D.Impulse);
    }

    public void NoKnockOut()
    {
        isKnockedOut = false;
        isWakingUp = true;
        numberOfHitsStunned = 0;
        onEntityWakeUp?.Invoke();
    }
}


interface IDamagable
{
    /// <summary>
    /// Apply damage
    /// </summary>
    /// <param name="damager">Who did the damage?</param>
    /// <param name="damage">The amount of damage</param>
    bool Damage(Entity damager, int damage);
    bool IsVulnerable { get; }
}
