using UnityEngine;
public class Entity : MonoBehaviour, IDamagable
{
    [SerializeField] private AudioClip landSound;
    [SerializeField] private AudioClip jumpSound;


    [SerializeField] private WeaponBase currentWeapon;
    private int ammo;

    private Vector2 aimDirection;

    #region Action Events
    public event System.Action onEntityLand;
    public event System.Action onEntityJump;
    public event System.Action onEntityAttack;
    #endregion
    #region Movement
    #region Properties
    [SerializeField] private float speed;
    private bool isJumping;
    #region States
    public bool IsMoving => controller.IsMoving;
    #endregion
    #endregion
    private Vector2 movement;
    private bool crounch;
    private bool cooldownJump;
    #endregion
    #region Components
    private CharacterController2D controller;
    private AudioSource audioSource;
    #endregion
    #region Mandatory Methods
    protected virtual void Start()
    {
        controller = GetComponent<CharacterController2D>();
        audioSource = GetComponent<AudioSource>();
        controller.OnLandEvent.AddListener(Entity_onEntityLand);
        controller.OnJumpEvent.AddListener(Entity_onEntityJump);
        onEntityAttack += Entity_onEntityAttack;
    }

    private void Entity_onEntityAttack()
    {
        audioSource.PlayOneShot(currentWeapon.ShootSound);
    }

    private void Entity_onEntityLand()
    {
        audioSource.PlayOneShot(landSound);
        onEntityLand?.Invoke();
    }

    private void Entity_onEntityJump()
    {
        audioSource.PlayOneShot(jumpSound);
        onEntityJump?.Invoke();
    }

    protected virtual void Update()
    {
    }
    protected virtual void FixedUpdate()
    {
        if (!isKnockedOut)
        {
            controller.Move(movement.x * speed, crounch, isJumping && cooldownJump);
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
        movement.x = direction;
    }
    /// <summary>
    /// Makes the character perform jumping
    /// </summary>
    protected void Jump(bool isTrue)
    {
        if (!cooldownJump && isTrue && controller.IsGrounded)
        {
            onEntityJump?.Invoke();
        }
        isJumping = isTrue;
        cooldownJump = true;
        TimerUtils.AddTimer(0.002f, ResetJump);
    }
    protected void Attack()
    {
        void PerformAttack()
        {
            currentWeapon?.Attack(this, aimDirection);
            onEntityAttack?.Invoke();
        }
        if (currentWeapon != null)
        {
            TimerUtils.AddTimer(currentWeapon.Cooldown, PerformAttack);
        }
    }
    protected void Aim(Vector2 direction)
    {

        if (direction == Vector2.zero)
        {
            aimDirection.y = 0;
            return;
        }
        aimDirection = direction;
    }
    public void Crounch(bool isTrue)
    {
        crounch = isTrue;
    }
    private void ResetJump()
    {
        cooldownJump = false;
    }
    #endregion
    [SerializeField] private int health = 10;
    private bool isKnockedOut;
    private int numberOfHits;
    public void Damage(Entity damager, int damage)
    {
        numberOfHits++;
        health--;
        if (numberOfHits % 4 == 0)
        {
            KnockOut(damager);
        }
    }
    private void KnockOut(Entity damager)
    {
        isKnockedOut = true;
    }
    public void NoKnockOut()
    {
        isKnockedOut = false;
    }
}


public class Enemy : Entity
{
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }
}


interface IDamagable
{
    void Damage(Entity damager, int damage);
    
}
