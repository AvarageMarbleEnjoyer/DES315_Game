using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class LightFlicker : MonoBehaviour
{
    [SerializeField] private float baseIntensity = 1.5f;
    [SerializeField] private float flickerStrength = 0.5f;
    [SerializeField] private float flickerSpeed = 8f;

    private Light lightSource;
    private float seed = 0f;
    
    private void Awake()
    {
        lightSource = GetComponent<Light>();
        seed = Random.Range(0f, 1000f);
    }

    private void Update()
    {
        float noise = Mathf.PerlinNoise(Time.time * flickerSpeed + seed, 0f);
        lightSource.intensity = baseIntensity + (noise - 0.5f) * flickerStrength * 2f;
    }
}
