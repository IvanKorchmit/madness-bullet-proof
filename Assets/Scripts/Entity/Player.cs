using UnityEngine;

public class Player : Entity
{
    [SerializeField] private AudioClip doubleJumpSound;
    private static Player _instance;
    public static Player Singleton => _instance;
    public WeaponBase PlayerWeapon => CurrentWeapon;
    private int grenades;
    public int Grenads => grenades;
    [SerializeField] private GameObject grenadePrefab;
    protected override void Start()
    {
        base.Start();
        Controller.OnDoubleJumpEvent.AddListener(OnPlayerDoubleJump);
        onEntityLand += Player_onEntityLand;
        onEntityWakeUp += () => TimerUtils.AddTimer(1.5f, () => state = FlagUtils.State_Remove(state, EntityState.IsImmune));
        onEntityKnockout += ()=> state = FlagUtils.State_Add(state, EntityState.IsImmune);
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
        EntityAnimator.SetTrigger(EntityAnimationConsts.JUMP_TRIGGER);
    }
    protected override void Update()
    {
        base.Update();
        BasicMovement();
        Aim(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")));
        Attack();
        Interact();
        #region Local Funcs

        void Interact()
        {
            if (!Input.GetKeyDown(KeyCode.E)) return;
            var interact = Physics2D.CircleCastAll(transform.position, 3f, new Vector2(), 0);
            foreach (var item in interact)
            {
                if (item && item.collider.TryGetComponent(out IInteractable interInstance) && interInstance.IsInteractable)
                {
                    interInstance.Interact();
                }
            }
        }

        void BasicMovement()
        {
            Move(Input.GetAxisRaw("Horizontal"));
            Jump(Input.GetKeyDown(KeyCode.Space) && !Input.GetKey(KeyCode.LeftShift));
            if (Input.GetKeyDown(KeyCode.Space) && Input.GetKey(KeyCode.LeftShift)) JumpOff();
            Crounch(Input.GetKey(KeyCode.LeftShift));
        }

        void Attack()
        {
            if (Input.GetKey(KeyCode.Return)) base.Attack();
        }
        #endregion
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }
}