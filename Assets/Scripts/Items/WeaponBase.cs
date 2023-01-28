using UnityEngine;

public abstract class WeaponBase : ScriptableObject
{
    [SerializeField] private float cooldown;
    [SerializeField] private Sprite sprite;
    [SerializeField] private AudioClip attackSound;
    [field: SerializeField] public bool IsAimable { get; private set; }
    public abstract void Attack(Entity owner, Vector2 direction);
    public AudioClip AttackSound => attackSound;
    public float Cooldown => cooldown;
    public Sprite WeaponSprite => sprite;

    [field: SerializeField] public RuntimeAnimatorController Animation { get; private set; }


}
