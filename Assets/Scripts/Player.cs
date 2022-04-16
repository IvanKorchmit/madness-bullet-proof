using UnityEngine;

public class Player : Entity
{
    private static Player _instance;
    public static Player Singleton => _instance;
    protected override void Start()
    {
        base.Start();
        _instance = this;
    }
    protected override void Update()
    {
        base.Update();
        Move(Input.GetAxisRaw("Horizontal"));
        Jump(Input.GetKey(KeyCode.Space));
        Crounch(Input.GetKey(KeyCode.LeftShift));
        Aim(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")));
        if (Input.GetKey(KeyCode.Return)) Attack();
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }
}