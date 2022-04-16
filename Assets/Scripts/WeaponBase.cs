using UnityEngine;

public abstract class WeaponBase : ScriptableObject
{
    [SerializeField] private float cooldown;
    [SerializeField] private Sprite sprite;
    [SerializeField] private AudioClip shootSound;
    public abstract void Attack(Entity owner, Vector2 direction);
    public AudioClip ShootSound => shootSound;
    public float Cooldown => cooldown;
}
