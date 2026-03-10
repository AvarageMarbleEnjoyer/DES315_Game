using UnityEngine;
using UnityEditor;

public class SceneMoodTool : EditorWindow
{
    // ── Values the artist controls ──
    private Color _shadowTint = new Color(0.20f, 0.20f, 0.40f, 1f);
    private Color _highlightTint = new Color(1.00f, 0.95f, 0.80f, 1f);
    private float _saturation = 1f;
    private float _pulseSpeed = 1f;
    private float _emissionMultiplier = 1f;

    [MenuItem("Tools/Scene Mood Tool")]
    public static void Open()
    {
        GetWindow<SceneMoodTool>("Scene Mood Tool");
    }

    void OnGUI()
    {
        GUILayout.Label("Colour Palette", EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();
        _shadowTint = EditorGUILayout.ColorField("Shadow Tint", _shadowTint);
        _highlightTint = EditorGUILayout.ColorField("Highlight Tint", _highlightTint);
        _saturation = EditorGUILayout.Slider("Saturation", _saturation, 0f, 2f);
        if (EditorGUI.EndChangeCheck())
        {
            ApplyToScene();
        }

        EditorGUILayout.Space(10);
        GUILayout.Label("Emissives", EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();
        _pulseSpeed = EditorGUILayout.Slider("Pulse Speed", _pulseSpeed, 0f, 5f);
        _emissionMultiplier = EditorGUILayout.Slider("Emission Multiplier", _emissionMultiplier, 0f, 5f);
        if (EditorGUI.EndChangeCheck())
        {
            ApplyToScene();
        }
    }

    void ApplyToScene()
    {
        Shader.SetGlobalColor("_ShadowTint", _shadowTint);
        Shader.SetGlobalColor("_HighlightTint", _highlightTint);
        Shader.SetGlobalFloat("_GlobalSaturation", _saturation);
    }
}