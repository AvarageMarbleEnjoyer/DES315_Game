using UnityEngine;

public class DamageNumberSpawner : MonoBehaviour
{
    public static DamageNumberSpawner Instance { get; private set; }

    [SerializeField] private DamageNumber damageNumberPrefab;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Spawn(float damage, Vector3 centreWorldPos)
    {
        if (damageNumberPrefab == null || damage <= 0f) return;
        DamageNumber number = Instantiate(damageNumberPrefab);
        number.transform.SetParent(null, true);
        number.Initialise(damage, centreWorldPos);
    }
}