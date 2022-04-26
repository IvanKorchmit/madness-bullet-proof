using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class StatusUI : MonoBehaviour
{
    private Text text;
    private void Start()
    {
        text = GetComponent<Text>();
    }
    private void OnGUI()
    {
        text.text = $"_______\nammo>{(Player.Singleton?.Ammo.ToString("D2")) ?? "00"}\nhp__>{(Player.Singleton?.Health.ToString("D2")) ?? "00"}\n_______\n";
    }
}
