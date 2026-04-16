using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthHUDUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currentHealthText;
    [SerializeField] private Image fillImage;
    [SerializeField] private Image blockFillImage;
    [SerializeField] private Player player;

    [Header("Portrait")]
    [SerializeField] private Image portraitImage;
    [SerializeField] private Sprite portraitInLight;
    [SerializeField] private Sprite portraitInShadow;

    private float cachedMaxHealth = 1f;
    private LightDetectable playerLightDetectable;

    private void Awake()
    {
        if (player == null)
            player = FindFirstObjectByType<Player>();

        if (player != null)
            playerLightDetectable = player.GetComponent<LightDetectable>();
    }

    private void OnEnable()
    {
        if (player != null)
        {
            player.OnHealthChanged += HandleHealthChanged;
            player.OnBlockChanged += HandleBlockChanged;
        }

        if (playerLightDetectable != null)
        {
            playerLightDetectable.OnLightStateChanged += HandleLightStateChanged;
            HandleLightStateChanged(playerLightDetectable.IsInLight);
        }
    }

    private void OnDisable()
    {
        if (player != null)
        {
            player.OnHealthChanged -= HandleHealthChanged;
            player.OnBlockChanged -= HandleBlockChanged;
        }

        if (playerLightDetectable != null)
            playerLightDetectable.OnLightStateChanged -= HandleLightStateChanged;
    }

    private void HandleLightStateChanged(bool isInLight)
    {
        if (portraitImage == null) return;

        portraitImage.sprite = isInLight ? portraitInLight : portraitInShadow;
    }

    private void HandleHealthChanged(float current, float max)
    {
        if (fillImage != null && max > 0f)
            fillImage.fillAmount = current / max;

        cachedMaxHealth = max;
        
        currentHealthText.text = $"{current}";
    }

    private void HandleBlockChanged(float current)
    {
        if (blockFillImage != null && cachedMaxHealth > 0f)
            blockFillImage.fillAmount = current / cachedMaxHealth;
    }
}