using TMPro;
using UnityEngine;

public class DamageNumber : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;

    private Camera mainCamera;

    /// <summary>
    /// Positions the number within a unit sphere around centreWorldPos and sets the displayed value.
    /// </summary>
    public void Initialise(float damage, Vector3 centreWorldPos)
    {
        transform.position = centreWorldPos + Random.insideUnitSphere;
        label.text = damage.ToString("0");
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (mainCamera != null)
            transform.rotation = mainCamera.transform.rotation;
    }

    // Call this via an Animation Event at the end of the spawn animation clip.
    public void DestroyNumber()
    {
        Destroy(gameObject);
    }
}