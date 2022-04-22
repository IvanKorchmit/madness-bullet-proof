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
        Jump(Input.GetKeyDown(KeyCode.Space));
        Crounch(Input.GetKey(KeyCode.LeftShift));
        Aim(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")));
        if (Input.GetKey(KeyCode.Return)) Attack(Input.GetKeyDown(KeyCode.Return));
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }
}