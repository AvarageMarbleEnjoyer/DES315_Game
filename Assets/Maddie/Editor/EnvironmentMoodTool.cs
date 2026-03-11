using UnityEngine;
using UnityEditor;

public class EnvironmentMoodTool : EditorWindow
{
    // ── Values the artist controls ──
    private Color _shadowTint = new Color(0.20f, 0.20f, 0.40f, 1f);
    private Color _highlightTint = new Color(1.00f, 0.95f, 0.80f, 1f);
    private float _saturation = 1f;
    private float _pulseSpeed = 1f;
    private float _emissionMultiplier = 1f;
    private int _toonSteps = 4;
    private float _fresnelIntensity = 0.5f;
    private Color _rimColour = Color.white;
    private float _specularHardness = 0.5f;

    [MenuItem("Tools/Environment Mood Tool")]
    public static void Open()
    {
        GetWindow<EnvironmentMoodTool>("Environment Mood Tool");
    }

    void OnEnable()
    {
        ApplyToScene();
    }

    void OnGUI()
    {
        GUILayout.Label("Colour Palette", EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();
        _shadowTint = EditorGUILayout.ColorField("Shadow Tint", _shadowTint);
        _highlightTint = EditorGUILayout.ColorField("Highlight Tint", _highlightTint);
        _saturation = EditorGUILayout.Slider("Saturation", _saturation, 0f, 2f);
        if (EditorGUI.EndChangeCheck()) ApplyToScene();

        EditorGUILayout.Space(10);
        GUILayout.Label("Emissives", EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();
        _pulseSpeed = EditorGUILayout.Slider("Pulse Speed", _pulseSpeed, 0f, 5f);
        _emissionMultiplier = EditorGUILayout.Slider("Emission Multiplier", _emissionMultiplier, 0f, 5f);
        if (EditorGUI.EndChangeCheck()) ApplyToScene();

        EditorGUILayout.Space(10);
        GUILayout.Label("Stylisation", EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();
        _toonSteps = EditorGUILayout.IntSlider("Toon Steps", _toonSteps, 2, 8);
        _fresnelIntensity = EditorGUILayout.Slider("Rim Light Intensity", _fresnelIntensity, 0f, 1f);
        _rimColour = EditorGUILayout.ColorField("Rim Light Colour", _rimColour);
        _specularHardness = EditorGUILayout.Slider("Specular Hardness", _specularHardness, 0f, 1f);
        if (EditorGUI.EndChangeCheck()) ApplyToScene();
    }

    void ApplyToScene()
    {
        Shader.SetGlobalColor("_Env_ShadowTint", _shadowTint);
        Shader.SetGlobalColor("_Env_HighlightTint", _highlightTint);
        Shader.SetGlobalFloat("_Env_Saturation", _saturation);
        Shader.SetGlobalFloat("_Env_PulseSpeed", _pulseSpeed);
        Shader.SetGlobalFloat("_Env_EmissionMultiplier", _emissionMultiplier);
        Shader.SetGlobalInt("_Env_ToonSteps", _toonSteps);
        Shader.SetGlobalFloat("_Env_FresnelIntensity", _fresnelIntensity);
        Shader.SetGlobalColor("_Env_RimColour", _rimColour);
        Shader.SetGlobalFloat("_Env_SpecularHardness", _specularHardness);
    }
}