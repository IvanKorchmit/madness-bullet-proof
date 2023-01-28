using UnityEngine;

public class Player : Entity
{
    public static Player Singleton { get; private set; }
    [field:SerializeField] public int Grenads { get; private set; }
    
    [field: SerializeField] public override bool IsUndamagable { get; set; }

    [SerializeField] private GameObject grenadePrefab;
    protected override void Start()
    {
        base.Start();
        Controller.OnDoubleJumpEvent.AddListener(OnPlayerDoubleJump);
        OnEntityLand += Player_onEntityLand;
    }

    private void Player_onEntityLand()
    {
        if (RB.velocity.magnitude > 35f)
        {
            var stomp = Physics2D.CircleCastAll(transform.position, 3f, new Vector2(), 0, Melee);
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
        if (w == CurrentWeapon.Base)
        {
            CurrentWeapon.Ammo += ammo;
        }
        else
        {
            CurrentWeapon.SetWeapon(w, ammo);
        }
        WeaponVisuals.enabled = CurrentWeapon != null;
    }
    private void Awake()
    {
        Singleton = this;
    }
    private void OnPlayerDoubleJump()
    {
        EntityAnimator.SetTrigger(JUMP_TRIGGER);
    }
    private void ThrowGrenade()
    {
        var gren = Instantiate(grenadePrefab, transform.position, Quaternion.identity);
        gren.GetComponent<Rigidbody2D>().velocity = ((Vector2)transform.up + (Visuals.flipX ? Vector2.left : Vector2.right)) * 10f;
    }
    protected override void Update()
    {
        base.Update();
        Move(Input.GetAxisRaw("Horizontal"));
        if (Input.GetKeyDown(KeyCode.Space) && Input.GetKey(KeyCode.LeftShift)) JumpOff();
        if (!Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        Aim(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")));
        Attack();
        Interact();

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

        void Attack()
        {
            if (Input.GetKey(KeyCode.Return)) TimerUtils.AddTimer(CurrentWeapon.Base.Cooldown, CurrentWeapon.Use);
            if (Input.GetKeyDown(KeyCode.Slash)) ThrowGrenade();
        }
    }

    public override bool Damage(IHitter damager, int damage)
    {
        throw new System.NotImplementedException();
    }

    public override void InstantKill()
    {
        throw new System.NotImplementedException();
    }
}