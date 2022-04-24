using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LineOfSight : MonoBehaviour
{
    [SerializeField] private LayerMask mask;
    public event System.Action onPlayerSpot;
    private bool hasSpotted = false;
    private void Update()
    {
        if (!hasSpotted)
        {
            Lookup();
        }
    }
    private void OnDrawGizmosSelected()
    {
        Player target = Player.Singleton;

        if (target == null) return;

        Vector2 direction = target.transform.position - transform.position;
        Gizmos.color = Find(target) ? Color.white : Color.red;
        Gizmos.DrawRay(transform.position, direction);
    }
    private void Lookup()
    {
        Player player = Player.Singleton;
        if (player == null) return;


        if (Find(player))
        {
            onPlayerSpot?.Invoke();
            hasSpotted = true;
        }
    }
    /// <summary>
    /// Check if the target on the sight (360 deg)
    /// </summary>
    /// <param name="target">Target to check</param>
    /// <returns>Returns true if the target is on sight. Whatever is it behind or not.</returns>
    private bool Find(Player target)
    {
        float dist = Vector2.Distance(transform.position, target.transform.position);
        Vector2 direction = target.transform.position - transform.position;
        int layers = mask;
        RaycastHit2D ray = Physics2D.Raycast(transform.position, direction, dist, layers);
        if (ray && ray.collider.gameObject.CompareTag("Player"))
        {
            return true;
        }
        return false;
    }
}
