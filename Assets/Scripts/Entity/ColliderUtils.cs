using UnityEngine;

public static class ColliderUtils
{
    public static void DisableColliders(Collider2D[] selfColliders,Collider2D[] hits, int i)
    {
        foreach (var currentCollider in selfColliders)
        {
            Physics2D.IgnoreCollision(currentCollider, hits[i], true);
        }
    }

    public static void JumpOffAction(Collider2D[] selfColliders, Collider2D[] hits, int i)
    {
        void Back()
        {
            foreach (var currentCollider in selfColliders)
            {
                Physics2D.IgnoreCollision(currentCollider, hits[i], false);
            }
        }
        DisableColliders(selfColliders, hits, i);
        TimerUtils.AddTimer(1f, Back);
    }
}