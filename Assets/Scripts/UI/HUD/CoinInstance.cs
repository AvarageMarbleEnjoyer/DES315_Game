using UnityEngine;
using UnityEngine.UI;

public class CoinInstance : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite[] coinSprites;

    [Header("Position Jitter")]
    public float xJitterRange = 5f;

    [SerializeField] private Image coinImage;

    private void Awake()
    {
        if (coinImage == null)
            coinImage = GetComponentInChildren<Image>();

        if (coinImage != null)
        {
            if (coinSprites != null && coinSprites.Length > 0)
                coinImage.sprite = coinSprites[Random.Range(0, coinSprites.Length)];

            RectTransform rt = coinImage.GetComponent<RectTransform>();
            Vector2 pos = rt.anchoredPosition;
            pos.x += Random.Range(-xJitterRange, xJitterRange);
            rt.anchoredPosition = pos;
        }
    }
}