using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private Animator animator;
    private int count;
    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        animator.SetInteger("Count", count);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Entity ent))
        {
            count++;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Entity ent))
        {
            count--;
        }
    }
}
