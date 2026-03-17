using UnityEngine;
using System.Collections.Generic;

public class TargetHighlighter : MonoBehaviour
{
    [Header("Child Object Names")]
    public string ringObjectName = "TargetRing";
    public string arrowObjectName = "TargetArrow";

    private Dictionary<GameObject, (GameObject ring, GameObject arrow)> indicators = new Dictionary<GameObject, (GameObject, GameObject)>();
    private HashSet<GameObject> currentTargets = new HashSet<GameObject>();
    private Color currentTint = Color.white;

    public void SetColor(Color color)
    {
        currentTint = color;
        foreach (var pair in indicators.Values)
        {
            TintObject(pair.ring, color);
            TintObject(pair.arrow, color);
        }
    }

    private void TintObject(GameObject obj, Color color)
    {
        if (obj == null) return;
        foreach (var image in obj.GetComponentsInChildren<UnityEngine.UI.Image>(true))
        {
            image.color = color;
        }
    }

    public void SetTarget(GameObject target)
    {
        if (target == null) { ClearTargets(); return; }
        SetTargets(new List<GameObject> { target });
    }

    public void SetTargets(List<GameObject> targets)
    {
        if (targets == null || targets.Count == 0) { ClearTargets(); return; }

        var newSet = new HashSet<GameObject>();
        foreach (var t in targets) { if (t != null) newSet.Add(t); }

        var toRemove = new List<GameObject>();
        foreach (var existing in currentTargets)
            if (!newSet.Contains(existing)) toRemove.Add(existing);

        foreach (var t in toRemove) SetIndicatorsActive(t, false);
        foreach (var t in newSet)
            if (!currentTargets.Contains(t)) SetIndicatorsActive(t, true);
    }

    public void ClearTargets()
    {
        foreach (var t in new List<GameObject>(currentTargets))
            SetIndicatorsActive(t, false);
    }

    private void SetIndicatorsActive(GameObject target, bool active)
    {
        if (!indicators.TryGetValue(target, out var pair))
        {
            var ring  = FindInChildren(target, ringObjectName);
            var arrow = FindInChildren(target, arrowObjectName);
            pair = (ring, arrow);
            indicators[target] = pair;
        }

        if (pair.ring  != null) pair.ring.SetActive(active);
        if (pair.arrow != null) pair.arrow.SetActive(active);

        if (active)
        {
            TintObject(pair.ring, currentTint);
            TintObject(pair.arrow, currentTint);
            currentTargets.Add(target);
        }
        else currentTargets.Remove(target);
    }

    private GameObject FindInChildren(GameObject root, string name)
    {
        Transform result = root.transform.Find(name);
        if (result != null) return result.gameObject;

        foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
        {
            if (child.name == name) return child.gameObject;
        }

        Debug.LogWarning($"[TargetHighlighter] Could not find '{name}' on {root.name}");
        return null;
    }

    private void OnDestroy()
    {
        ClearTargets();
    }
}