using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    [Header("Data")]
    [SerializeField] private TutorialDatabase database;

    [Header("UI References")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI messageText;

    private readonly HashSet<string> _shownThisRun = new(System.StringComparer.OrdinalIgnoreCase);
    private readonly Queue<string> _queue = new();
    private bool _isShowing;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (panel != null) panel.SetActive(false);
    }

    private void Start()
    {
        Trigger("game_start");
    }

    /// <summary>
    /// Queues a tutorial popup for the given key. If nothing is currently showing it displays
    /// immediately, otherwise it waits until the active popup is closed.
    /// Does nothing if the key was already shown this run, is missing from the database, or required UI references are unassigned.
    /// </summary>
    public void Trigger(string key)
    {
        if (string.IsNullOrWhiteSpace(key)) return;
        if (_shownThisRun.Contains(key)) return;
        if (database == null)
        {
            Debug.LogWarning("[TutorialManager] No TutorialDatabase assigned.");
            return;
        }
        if (!database.TryGetMessage(key, out string _))
        {
            Debug.LogWarning($"[TutorialManager] Trigger key not found in database: '{key}'");
            return;
        }
        if (panel == null || messageText == null)
        {
            Debug.LogWarning("[TutorialManager] Panel or MessageText reference is missing.");
            return;
        }

        _shownThisRun.Add(key);

        if (_isShowing)
        {
            _queue.Enqueue(key);
            return;
        }

        ShowKey(key);
    }

    public void Close()
    {
        _isShowing = false;

        if (_queue.Count > 0)
        {
            ShowKey(_queue.Dequeue());
            return;
        }

        if (panel != null) panel.SetActive(false);
        Time.timeScale = 1f;
    }

    private void ShowKey(string key)
    {
        database.TryGetMessage(key, out string message);
        messageText.text = message;
        panel.SetActive(true);
        Time.timeScale = 0f;
        _isShowing = true;
    }
}