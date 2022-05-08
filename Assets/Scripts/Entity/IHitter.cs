using UnityEngine;

public interface IHitter : IDamagable
{
    Transform transform { get; }
    GameObject gameObject { get; }
}