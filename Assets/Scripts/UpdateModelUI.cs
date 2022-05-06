using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateModelUI : MonoBehaviour
{
    [SerializeField] private WeaponAndModel[] weaponModels;
    private void OnGUI()
    {
        Player p = Player.Singleton;
        if (p == null) return;
        foreach (WeaponAndModel item in weaponModels)
        {
            item.model.gameObject.SetActive(p.PlayerWeapon == item.weapon);
        }
    }
}

[System.Serializable]
public struct WeaponAndModel
{
    public Transform model;
    public WeaponBase weapon;
}
