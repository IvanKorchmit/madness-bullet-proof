using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MenuManager : MonoBehaviour
{
    [SerializeField] private AudioClip navigationSound;
    [SerializeField] private AudioClip submitSound;
    [SerializeField] private AudioSource audioSource;
    private int currentIndex;
    [System.Serializable]
    public struct OptionSelector
    {
        [SerializeField] private UnityEvent onButtonSelect;
        [SerializeField] private Text option;
        public bool Select()
        {
            if (option.gameObject.activeInHierarchy)
            {
                if (option.text[0] == '_')
                {
                    option.text = option.text.Remove(0, 1);
                    option.text = ">" + option.text;
                    return true;
                }
            }
            return false;
        }
        public void Deselect()
        {
            if (option.gameObject.activeInHierarchy)
            {
                if (option.text[0] == '>')
                {
                    option.text = option.text.Remove(0, 1);
                    option.text = "_" + option.text;
                }
            }
        }
        public void Submit()
        {
            onButtonSelect?.Invoke();
        }
    }
    [SerializeField] private OptionSelector[] option;
    private bool hasPressedAny;
    public UnityEvent onAnyKeyPress;
    private void OnGUI()
    {
        if (!hasPressedAny && Event.current.keyCode != KeyCode.None)
        {
            hasPressedAny = true;
            onAnyKeyPress?.Invoke();
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S))
        {
            int axis = (int)Input.GetAxisRaw("Vertical") * -1;
            option[currentIndex].Deselect();
            if (currentIndex + axis < 0)
            {
                currentIndex = option.Length - 1;
            }
            else if (currentIndex + axis >= option.Length)
            {
                currentIndex = 0;
            }
            else
            {
                currentIndex += axis;
            }
            if (option[currentIndex].Select())
            {
                audioSource.PlayOneShot(navigationSound);
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            audioSource.PlayOneShot(submitSound);
            option[currentIndex].Submit();
        }
    }
    public void LoadLevel(string levelName)
    {
        SceneManager.LoadSceneAsync(levelName);
    }
}
