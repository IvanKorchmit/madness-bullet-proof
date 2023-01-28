using UnityEngine;

public class Volumes : MonoBehaviour
{
    private static float m_MusicVolume = 1f;
    private static float m_SFXVolume = 1f;
    public static float Music => m_MusicVolume;
    public static float SFX => m_SFXVolume;
    public static event System.Action<float> OnMusicVolumeChanged;

    public void ChangeSFX (System.Single value)
    {
        m_SFXVolume = value;
    }
    public void ChangeMusic(System.Single value)
    {
        m_MusicVolume = value;
        OnMusicVolumeChanged?.Invoke(m_MusicVolume);
    }
}