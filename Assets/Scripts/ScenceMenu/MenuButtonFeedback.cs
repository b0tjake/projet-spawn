using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButtonFeedback : MonoBehaviour,
    ISelectHandler, IDeselectHandler
{
    [Header("Visual")]
    [SerializeField] private GameObject borderObject;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hoverSound;

    private void Awake()  // Change Start → Awake, runs before selection events
{
    RefreshVisual(false);
}

    private void Start()
    {
        RefreshVisual(false);
    }

    public void OnSelect(BaseEventData eventData)
    {
        PlayHoverSound();
        RefreshVisual(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        RefreshVisual(false);
    }

    private void RefreshVisual(bool active)
    {
        if (borderObject != null)
            borderObject.SetActive(active);
    }

    private void PlayHoverSound()
    {
        if (audioSource == null || hoverSound == null) return;
        if (!audioSource.isActiveAndEnabled) return;

        audioSource.PlayOneShot(hoverSound);
    }
}
