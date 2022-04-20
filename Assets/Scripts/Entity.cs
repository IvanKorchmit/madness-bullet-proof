using UnityEngine;
public abstract class Entity : MonoBehaviour, IDamagable
{
    [SerializeField] private AudioClip landSound;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip defaultPunchSound;

    [SerializeField] private LayerMask meleeLayerMask;

    public const string IS_MOVING = "isMoving";
    public const string IS_FALLING = "isFalling";
    public const string IS_KNOCKED_OUT = "isKnockedOut";
    public const string JUMP_TRIGGER = "Jump";
    public const string ATTACK_TRIGGER = "Attack";
    public const string DAMAGE_TRIGGER = "Damage";
    public const string LAND_TRIGGER = "Land";

    public bool IsVulnerable => isKnockedOut;

    [SerializeField] private WeaponBase currentWeapon;
    private int ammo;
    private Vector2 aimDirection;
    public int MeleeMask => meleeLayerMask;
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
    public bool IsMoving => controller.IsMoving;
    public bool IsFalling => rb.velocity.y < 0 && !controller.IsGrounded;
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
    }

    #region Event Subscriptions

    private void Entity_onEntityWakeUp()
    {
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        sr.color = Color.white;
        sr.transform.rotation = Quaternion.identity;
    }

    private void Entity_onEntityKnockout()
    {

        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        sr.color = Color.blue;
        sr.transform.rotation = Quaternion.Euler(0, 0, 90);

    }
    private void Entity_onEntityRecover()
    {
        GetComponentInChildren<SpriteRenderer>().color = Color.white;
    }

    private void Entity_onEntityStun()
    {
        GetComponentInChildren<SpriteRenderer>().color = Color.red;
    }

    protected virtual void Entity_onEntityAttack()
    {
        audioSource.PlayOneShot(currentWeapon.ShootSound);
    }

    private void Entity_onEntityPunch()
    {
        audioSource.PlayOneShot(defaultPunchSound);
    }

    protected virtual void Entity_onEntityLand()
    {
        if (isKnockedOut)
        {
            rb.velocity = new Vector2();
        }
        Debug.Log("Land");
        audioSource.PlayOneShot(landSound);
        animator.SetTrigger(LAND_TRIGGER);
        onEntityLand?.Invoke();
    }

    protected virtual void Entity_onEntityJump()
    {
        audioSource.PlayOneShot(jumpSound);
        onEntityJump?.Invoke();
        animator.SetTrigger(JUMP_TRIGGER);
    }

    #endregion
    protected virtual void Update()
    {
        if (!isKnockedOut)
        {
            controller.enabled = true;
            controller.Move(movement.x * speed, crounch, isJumping);
        }
        else
        {
            controller.Move(0, false, false);
            controller.enabled = false;
        }
    }
    protected virtual void FixedUpdate()
    {
        // animator.SetBool(IS_MOVING, IsMoving);
        animator.SetBool(IS_FALLING, IsFalling);
    }
    #endregion
    #region Movement Methods
    /// <summary>
    /// Forces character to walk the desired direction
    /// </summary>
    /// <param name="direction"></param>
    protected void Move(float direction)
    {
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
    protected void Attack(bool quickly)
    {
        if (isKnockedOut) return;
        void Punch()
        {
            if (isKnockedOut) return;
            RaycastHit2D hit = Physics2D.CircleCast(transform.position, 2f, movement, 1f, meleeLayerMask);
            if (hit.collider != null)
            {
                if (hit.collider.TryGetComponent(out IDamagable damage))
                {
                    damage.Damage(this, 2);
                    onEntityPunch?.Invoke();
                }
            }
        }
        void PerformAttack()
        {
            if (isKnockedOut) return;
            currentWeapon?.Attack(this, aimDirection);
            onEntityAttack?.Invoke();
        }
        if (currentWeapon != null)
        {
            if (!quickly)
            {
                TimerUtils.AddTimer(currentWeapon.Cooldown, PerformAttack);
            }
            else
            {
                PerformAttack();
            }
        }
        else
        {

            if (!quickly)
            {
                TimerUtils.AddTimer(0.5f, Punch);
            }
            else
            {
                Punch();
            }
        }
    }
    protected void Aim(Vector2 direction)
    {
        if (isKnockedOut) return;

        if (direction == Vector2.zero)
        {
            aimDirection.y = 0;
            return;
        }
        aimDirection = direction;
    }
    public void Crounch(bool isTrue)
    {
        if (isKnockedOut) return;

        crounch = isTrue;
    }
    #endregion
    [SerializeField] private int health = 10;
    private bool isKnockedOut;
    private int numberOfHits;
    public void Damage(Entity damager, int damage)
    {
        void Death ()
        {
            Destroy(gameObject,1.75f);
            KnockOut(damager);

        }
        if (isKnockedOut && damager != this)
        {
            Push(damager);
            return;
        }
        numberOfHits += damage;
        health -= damage;
        if (numberOfHits >= 4)
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
    }
    private void Stun()
    {
        isKnockedOut = true;
        onEntityStun?.Invoke();
        TimerUtils.AddTimer(0.33333f, ResetStun);
    }
    private void ResetStun()
    {
        isKnockedOut = false;
        onEntityRecover?.Invoke();
    }
    private void KnockOut(Entity damager)
    {
#if UNITY_EDITOR
        try
        {
#endif
            numberOfHits = 0;
            isKnockedOut = true;
            Push(damager);
            onEntityKnockout?.Invoke();
            TimerUtils.Cancel(ResetStun);
            TimerUtils.Cancel(NoKnockOut);
            TimerUtils.AddTimer(2f, NoKnockOut);
        }
#if UNITY_EDITOR
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
        rb.AddForce(new Vector2((direction.x >= 0f ? 1f : -1f) * 5f, 10f), ForceMode2D.Impulse);
    }

    public void NoKnockOut()
    {
        isKnockedOut = false;
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
    void Damage(Entity damager, int damage);
    bool IsVulnerable { get; }
}
