using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstAid : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Player.Singleton == null) return;
        if (collision.gameObject == Player.Singleton.gameObject)
        {
            Player.Singleton.Health += 7;
            Destroy(gameObject);
        }
    }
}
