using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    private Transform currentTarget;
    private Vector2 start;
    [SerializeField] private float duration;
    private float elapsedTime;
    public void MoveTo(Transform target)
    {
        start = currentTarget == null ? (Vector2)transform.position : new Vector2(transform.position.x, currentTarget.position.y);
        currentTarget = target;
        elapsedTime = 0;
    }
    private void Update()
    {
        if (currentTarget == null) return;
        Vector2 targetPos = new Vector2(transform.position.x, currentTarget.position.y);
        elapsedTime += Time.deltaTime;
        float percComplete = elapsedTime / duration;
        transform.position = Vector2.Lerp(start, targetPos, percComplete);
    }
}
