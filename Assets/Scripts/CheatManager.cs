using UnityEngine;

/// <summary>
/// Manages cheat/debug toggles.
/// Attach this to a persistent GameObject in the scene.
/// The UI buttons in your cheat menu should call the public Toggle methods.
/// </summary>
public class CheatManager : MonoBehaviour
{
    public static CheatManager Instance { get; private set; }

    [Header("Cheat State")]
    [SerializeField] private bool infiniteHealth = false;
    [SerializeField] private bool infiniteCoins  = false;

    // Events so the UI can update its toggle visuals without polling
    public event System.Action<bool> OnInfiniteHealthChanged;
    public event System.Action<bool> OnInfiniteCoinsChanged;

    // Properties
    public bool InfiniteHealth => infiniteHealth;
    public bool InfiniteCoins  => infiniteCoins;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>Toggle infinite health on/off.</summary>
    public void ToggleInfiniteHealth()
    {
        SetInfiniteHealth(!infiniteHealth);
    }

    /// <summary>Set infinite health explicitly.</summary>
    public void SetInfiniteHealth(bool value)
    {
        if (infiniteHealth == value) return;
        infiniteHealth = value;
        Debug.Log($"[CheatManager] Infinite Health: {infiniteHealth}");
        OnInfiniteHealthChanged?.Invoke(infiniteHealth);
    }

    /// <summary>Toggle infinite coins (action points) on/off.</summary>
    public void ToggleInfiniteCoins()
    {
        SetInfiniteCoins(!infiniteCoins);
    }

    /// <summary>Set infinite coins explicitly.</summary>
    public void SetInfiniteCoins(bool value)
    {
        if (infiniteCoins == value) return;
        infiniteCoins = value;
        Debug.Log($"[CheatManager] Infinite Coins: {infiniteCoins}");
        OnInfiniteCoinsChanged?.Invoke(infiniteCoins);
    }
}