using UnityEngine;
public abstract class Entity : MonoBehaviour, IDamagable
{
    [SerializeField] private AudioClip landSound;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip defaultPunchSound;

    [SerializeField] private LayerMask meleeLayerMask;


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
    public AudioSource Audio => audioSource;
    [SerializeField] private float speed;
    private bool isJumping;
    #region States
    public bool IsMoving => controller.IsMoving;
    #endregion
    #endregion
    private Vector2 movement;
    private bool crounch;
    #endregion
    #region Components
    private CharacterController2D controller;
    private AudioSource audioSource;
    private Rigidbody2D rb;
    #endregion
    #region Mandatory Methods
    protected virtual void Start()
    {
        controller = GetComponent<CharacterController2D>();
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
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
        audioSource.PlayOneShot(landSound);
        onEntityLand?.Invoke();
    }

    protected virtual void Entity_onEntityJump()
    {
        audioSource.PlayOneShot(jumpSound);
        onEntityJump?.Invoke();
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
            controller.enabled = false;
        }
    }
    protected virtual void FixedUpdate()
    {
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
    protected void Attack()
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
            TimerUtils.AddTimer(currentWeapon.Cooldown, PerformAttack);
        }
        else
        {

            TimerUtils.AddTimer(0.5f, Punch);
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
        if (isKnockedOut)
        {
            Push(damager);
        }
        if (IsVulnerable) return;
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
            Destroy(gameObject);
        }
    }
    private void Stun()
    {
        numberOfHits = 0;
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
        Debug.Log("Knocked out");
        isKnockedOut = true;
        Push(damager);
        onEntityKnockout?.Invoke();
        TimerUtils.Cancel(ResetStun);
        TimerUtils.Cancel(NoKnockOut);
        TimerUtils.AddTimer(2f, NoKnockOut);
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
