using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages the outline highlight effect on targetable enemies
/// </summary>
public class TargetHighlighter : MonoBehaviour
{
    [Header("Highlight Settings")]
    public Color highlightColor = new Color(1f, 0.8f, 0.2f, 1f);
    public float outlineThickness = 2.0f;
    public float outlineIntensity = 3.0f;

    [Header("Shader")]
    [Tooltip("Leave null to auto-find 'Custom/OccludedOutline'")]
    public Shader outlineShader;

    private HashSet<GameObject> currentTargets = new HashSet<GameObject>();
    private Dictionary<GameObject, List<GameObject>> outlineObjects = new Dictionary<GameObject, List<GameObject>>();

    private void Awake()
    {
        if (outlineShader == null)
        {
            outlineShader = Shader.Find("Custom/OccludedOutline");
        }

        if (outlineShader == null)
        {
            Debug.LogWarning("[TargetHighlighter] Could not find outline shader. Using fallback.");
        }
    }

    /// <summary>
    /// Set the target to highlight
    /// </summary>
    public void SetTarget(GameObject target)
    {
        if (target == null)
        {
            ClearTargets();
            return;
        }

        SetTargets(new List<GameObject> { target });
    }

    /// <summary>
    /// Clear any current highlight
    /// </summary>
    public void ClearTargets()
    {
        if (currentTargets.Count == 0)
        {
            return;
        }

        var targetsToRemove = new List<GameObject>(currentTargets);
        for (int i = 0; i < targetsToRemove.Count; i++)
        {
            RemoveHighlight(targetsToRemove[i]);
        }
    }

    public void SetTargets(List<GameObject> targets)
    {
        if (targets == null || targets.Count == 0)
        {
            ClearTargets();
            return;
        }

        HashSet<GameObject> newTargets = new HashSet<GameObject>();
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i] != null)
            {
                newTargets.Add(targets[i]);
            }
        }

        if (newTargets.Count == 0)
        {
            ClearTargets();
            return;
        }

        var toRemove = new List<GameObject>();
        foreach (GameObject existing in currentTargets)
        {
            if (!newTargets.Contains(existing))
            {
                toRemove.Add(existing);
            }
        }

        for (int i = 0; i < toRemove.Count; i++)
        {
            RemoveHighlight(toRemove[i]);
        }

        foreach (GameObject target in newTargets)
        {
            if (!currentTargets.Contains(target))
            {
                ApplyHighlight(target);
            }
        }
    }

    private void ApplyHighlight(GameObject target)
    {
        Renderer[] renderers = target.GetComponentsInChildren<Renderer>();
        List<GameObject> spawnedObjects = new List<GameObject>();

        foreach (Renderer renderer in renderers)
        {
            if (renderer is ParticleSystemRenderer) continue;
            if (renderer.GetComponentInParent<EnemyVisionCone>() != null) continue;

            MeshFilter meshFilter = renderer.GetComponent<MeshFilter>();
            if (meshFilter == null || meshFilter.sharedMesh == null) continue;

            int subMeshCount = meshFilter.sharedMesh.subMeshCount;

            Material[] outlineMats = new Material[subMeshCount];
            for (int i = 0; i < subMeshCount; i++)
            {
                outlineMats[i] = CreateOutlineMaterial();
            }

            GameObject outlineObj = new GameObject("_OutlinePass");
            outlineObj.transform.SetParent(renderer.transform, false);
            outlineObj.transform.localPosition = Vector3.zero;
            outlineObj.transform.localRotation = Quaternion.identity;
            outlineObj.transform.localScale = Vector3.one;

            MeshFilter outlineMF = outlineObj.AddComponent<MeshFilter>();
            outlineMF.sharedMesh = meshFilter.sharedMesh;

            MeshRenderer outlineMR = outlineObj.AddComponent<MeshRenderer>();
            outlineMR.materials = outlineMats;
            outlineMR.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            outlineMR.receiveShadows = false;

            spawnedObjects.Add(outlineObj);
        }

        outlineObjects[target] = spawnedObjects;
        currentTargets.Add(target);
    }

    private void RemoveHighlight(GameObject target)
    {
        if (outlineObjects.TryGetValue(target, out List<GameObject> spawnedObjects))
        {
            for (int i = 0; i < spawnedObjects.Count; i++)
            {
                if (spawnedObjects[i] == null) continue;

                MeshRenderer mr = spawnedObjects[i].GetComponent<MeshRenderer>();
                if (mr != null)
                {
                    foreach (Material mat in mr.materials)
                    {
                        if (mat != null) Destroy(mat);
                    }
                }

                Destroy(spawnedObjects[i]);
            }

            outlineObjects.Remove(target);
        }

        currentTargets.Remove(target);
    }

    private Material CreateOutlineMaterial()
    {
        Material mat;

        if (outlineShader != null)
        {
            mat = new Material(outlineShader);
            mat.SetColor("_OutlineColor", highlightColor);
            mat.SetFloat("_OutlinePower", outlineThickness);
            mat.SetFloat("_OutlineIntensity", outlineIntensity);
        }
        else
        {
            // Use a simple unlit transparent material
            Shader fallbackShader = Shader.Find("Universal Render Pipeline/Unlit");
            mat = new Material(fallbackShader);

            mat.SetFloat("_Surface", 1);
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

            mat.SetColor("_BaseColor", new Color(highlightColor.r, highlightColor.g, highlightColor.b, 0.3f));
        }

        return mat;
    }

    /// <summary>
    /// Update highlight color
    /// </summary>
    public void SetHighlightColor(Color color)
    {
        highlightColor = color;

        foreach (List<GameObject> spawnedObjects in outlineObjects.Values)
        {
            for (int i = 0; i < spawnedObjects.Count; i++)
            {
                if (spawnedObjects[i] == null) continue;

                MeshRenderer mr = spawnedObjects[i].GetComponent<MeshRenderer>();
                if (mr == null) continue;

                foreach (Material mat in mr.materials)
                {
                    if (mat == null) continue;
                    if (outlineShader != null)
                    {
                        mat.SetColor("_OutlineColor", color);
                    }
                    else
                    {
                        mat.SetColor("_BaseColor", new Color(color.r, color.g, color.b, 0.3f));
                    }
                }
            }
        }
    }

    private void OnDestroy()
    {
        ClearTargets();
    }
}