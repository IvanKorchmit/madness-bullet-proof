using UnityEngine;

public class Player : Entity
{
    [SerializeField] private AudioClip doubleJumpSound;
    private static Player _instance;
    public static Player Singleton => _instance;
    protected override void Start()
    {
        base.Start();
        Controller.OnDoubleJumpEvent.AddListener(OnPlayerDoubleJump);
        onEntityLand += Player_onEntityLand;
        onEntityWakeUp += Player_onEntityWakeUp;
        onEntityKnockout += Player_onEntityKnockout;
    }

    private void Player_onEntityKnockout()
    {
        immune = true;
    }

    private void Player_onEntityWakeUp()
    {
        void ResetImmune()
        {
            immune = false;
        }
        TimerUtils.AddTimer(1.5f, ResetImmune);
    }

    private void Player_onEntityLand()
    {
        if (RB.velocity.magnitude > 35f)
        {
            var stomp = Physics2D.CircleCastAll(transform.position, 3f, new Vector2(), 0, meleeLayerMask);
            foreach (var item in stomp)
            {
                if (item)
                {
                    if (item.collider.TryGetComponent(out IDamagable damage))
                    {
                        damage.Damage(this, Mathf.RoundToInt(RB.velocity.magnitude / 10));
                    }
                }
            }
        }
    }

    public void SetWeapon(WeaponBase w, int ammo)
    {
        if (w == CurrentWeapon)
        {
            Ammo += ammo;
        }
        else
        {
            CurrentWeapon = w;
            Ammo = ammo;
        }
        weaponVisuals.enabled = CurrentWeapon != null;
    }
    private void Awake()
    {
        _instance = this;
    }
    private void OnPlayerDoubleJump()
    {
        Audio.PlayOneShot(doubleJumpSound);
        EntityAnimator.SetTrigger(JUMP_TRIGGER);
    }
    protected override void Update()
    {
        base.Update();
        Move(Input.GetAxisRaw("Horizontal"));
        Jump(Input.GetKeyDown(KeyCode.Space) && !Input.GetKey(KeyCode.LeftShift));
        Crounch(Input.GetKey(KeyCode.LeftShift));
        Aim(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")));
        if (Input.GetKey(KeyCode.Return)) Attack();
        if (Input.GetKeyDown(KeyCode.Space) && Input.GetKey(KeyCode.LeftShift)) JumpOff();
        if (Input.GetKeyDown(KeyCode.E))
        {
            var interact = Physics2D.CircleCastAll(transform.position, 3f, new Vector2(), 0);
            foreach (var item in interact)
            {
                if (item)
                {
                    if (item.collider.TryGetComponent(out IInteractable interInstance))
                    {
                        if (interInstance.IsInteractable)
                        {
                            interInstance.Interact();
                        }
                    }
                }
            }
        }
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }
}

public interface IInteractable
{
    bool IsInteractable { get; }
    void Interact();
}
