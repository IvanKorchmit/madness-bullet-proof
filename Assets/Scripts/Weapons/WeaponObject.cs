using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponObject : MonoBehaviour
{
    [SerializeField] private WeaponBase weapon;
    [SerializeField] private int ammo;
    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = weapon.WeaponSprite;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player p = Player.Singleton;
        if (p)
        if (collision.gameObject == p.gameObject)
        {
                p.SetWeapon(weapon, ammo);
                Destroy(gameObject);
        }
    }
}
