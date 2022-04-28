using UnityEngine;
using UnityEngine.Events;
public class PushableButton : MonoBehaviour, IInteractable
{
    public UnityEvent onButtonPush;
    public bool IsInteractable => activated;
    [SerializeField] private bool activated = true;
    [SerializeField] private Sprite defaultSprite; 
    [SerializeField] private Sprite pushedSprite;
    [SerializeField] private AudioClip pushButtonSound;
    private AudioSource audioSource;
    private SpriteRenderer rend;
    public void Interact()
    {
        void ResetSprite()
        {
            rend.sprite = defaultSprite;
        }
        rend.sprite = pushedSprite;
        audioSource.PlayOneShot(pushButtonSound);
        TimerUtils.AddTimer(1f, ResetSprite);
        onButtonPush?.Invoke();
    }
    public void Unlock()
    {
        activated = true;
    }
    private void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        defaultSprite = rend.sprite;
    }
}