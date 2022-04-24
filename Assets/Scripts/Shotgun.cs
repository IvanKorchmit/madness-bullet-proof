using UnityEngine;

[CreateAssetMenu(fileName = "New Shotgun")]
public class Shotgun : Firearm
{
    [SerializeField] private float cone;
    public static Vector2 RandomVector(Vector3 min, Vector3 max)
    {
        return new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));
    }
    public override void Attack(Entity owner, Vector2 direction)
    {
        Vector2 curr = Vector2.Perpendicular(direction);
        for (int i = 0; i < 5; i++)
        {
            curr = Vector2.Lerp(curr, direction, 0.25f);
            base.Attack(owner, curr);
        }

    }
}