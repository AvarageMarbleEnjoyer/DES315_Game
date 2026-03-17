using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DoorTransitionManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image fadePanel;

    private static DoorTransitionManager instance;

    private void Awake()
    {
        instance = this;
    }

    public IEnumerator FadeToBlack(float duration)
    {
        if (fadePanel == null)
        {
            Debug.LogError("[DoorTransitionManager] No fade panel found!");
            yield break;
        }

        fadePanel.gameObject.SetActive(true);

        float elapsed = 0f;
        Color color = fadePanel.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsed / duration);
            fadePanel.color = color;
            yield return null;
        }

        color.a = 1f;
        fadePanel.color = color;
    }

    public IEnumerator FadeFromBlack(float duration)
    {
        if (fadePanel == null)
        {
            Debug.LogError("[DoorTransitionManager] No fade panel found!");
            yield break;
        }

        float elapsed = 0f;
        Color color = fadePanel.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            color.a = 1f - Mathf.Clamp01(elapsed / duration);
            fadePanel.color = color;
            yield return null;
        }

        color.a = 0f;
        fadePanel.color = color;
        fadePanel.gameObject.SetActive(false);
    }

    public void SetBlack()
    {
        if (fadePanel != null)
        {
            fadePanel.gameObject.SetActive(true);
            Color c = fadePanel.color;
            c.a = 1f;
            fadePanel.color = c;
        }
    }

    public void SetClear()
    {
        if (fadePanel != null)
        {
            Color c = fadePanel.color;
            c.a = 0f;
            fadePanel.color = c;
            fadePanel.gameObject.SetActive(false);
        }
    }

    public static DoorTransitionManager Instance => instance;
}