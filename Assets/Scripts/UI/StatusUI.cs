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
        text.text = $"________\nammo>{(Player.Singleton?.Ammo.ToString("D3")) ?? "000"}\nhp__>{(Player.Singleton?.Health.ToString("D3")) ?? "000"}\n________\n";
    }
}
