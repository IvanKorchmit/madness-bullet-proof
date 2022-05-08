public interface IDamagable
{
    /// <summary>
    /// Apply damage
    /// </summary>
    /// <param name="damager">Who did the damage?</param>
    /// <param name="damage">The amount of damage</param>
    bool Damage(IHitter damager, int damage);
    void InstantKill();
    bool IsUndamagable { get; }
}
