using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// Attached to each combat log entry prefab.
/// After <see cref="displayDuration"/> seconds the text fades out, then the GameObject is destroyed.
/// Call <see cref="Initialize"/> immediately after instantiation.
/// </summary>
public class CombatLogEntry : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI label;

    [Header("Timing")]
    [SerializeField] private float displayDuration = 3f;
    [SerializeField] private float fadeDuration = 1f;

    /// <summary>
    /// Sets the message text and starts the display/fade timer.
    /// Call this right after Instantiate().
    /// </summary>
    public void Initialize(string message, float overrideDisplayDuration = -1f, float overrideFadeDuration = -1f)
    {
        if (label != null)
            label.text = message;

        if (overrideDisplayDuration >= 0f)
            displayDuration = overrideDisplayDuration;

        if (overrideFadeDuration >= 0f)
            fadeDuration = overrideFadeDuration;

        StartCoroutine(FadeAndDestroy());
    }

    private IEnumerator FadeAndDestroy()
    {
        // Wait at full opacity
        yield return new WaitForSeconds(Mathf.Max(0f, displayDuration));

        // Lerp alpha to zero
        if (label != null)
        {
            Color startColor = label.color;
            float elapsed = 0f;
            float duration = Mathf.Max(0.01f, fadeDuration);

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                label.color = new Color(startColor.r, startColor.g, startColor.b, Mathf.Lerp(1f, 0f, t));
                yield return null;
            }
        }

        Destroy(gameObject);
    }

#if UNITY_EDITOR
    private void Reset()
    {
        label = GetComponentInChildren<TextMeshProUGUI>();
    }
#endif
}