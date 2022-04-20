using UnityEngine;

[CreateAssetMenu(fileName ="New Gun")]
public class Firearm : WeaponBase
{

    private static readonly float[] angles = { 285, 270, 210, 180, 135, 90, 30, 0 };
    [SerializeField] private GameObject projectilePrefab;
    public override void Attack(Entity owner, Vector2 direction)
    {
        Quaternion rotation = Quaternion.Euler(0, 0, Angle(direction,owner));
        Instantiate(projectilePrefab, owner.transform.position, rotation).GetComponent<Projectile>().Init(owner);
    }
    private static float Angle(Vector3 v, Entity owner)
    {
        // normalize the vector: this makes the x and y components numerically
        // equal to the sine and cosine of the angle:
        v.Normalize();
        // get the basic angle:
        var ang = Mathf.Asin(v.y) * Mathf.Rad2Deg;
        // fix the angle for 2nd and 3rd quadrants:
        if (v.x < 0)
        {
            ang = 180 - ang;
        }
        else // fix the angle for 4th quadrant:
        if (v.y < 0)
        {
            ang = 360 + ang;
        }
        ang = owner.IsMoving ? BestFitAngle(ang) : ang;
        return ang;
    }

    private static float BestFitAngle(float angle)
    {
        // 315
        foreach (var a in angles)
        {
            if (angle >= a)
            {
                return angle == 315 ? a + 45 : angle == 135 ? a + 15 : a;
            }
        }
        return 0;
    }

}
