using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponObject : MonoBehaviour
{
    [SerializeField] private WeaponBase weapon;
    [SerializeField] private int ammo;
    private bool hasPicked;
    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = weapon.WeaponSprite;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player p = Player.Singleton;
        if (!hasPicked && p)
        {
            if (collision.gameObject == p.gameObject)
            {
                hasPicked = true;
                p.SetWeapon(weapon, ammo);
                Destroy(gameObject);
                Debug.Log(ammo);
            }
        }
    }
}
