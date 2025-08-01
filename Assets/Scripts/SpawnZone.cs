using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SpawnZone : MonoBehaviour
{
    private Collider _cachedCollider;

    public Collider SpawnZoneCollider
    {
        get => _cachedCollider;
        private set => _cachedCollider = value;
    }

    public void Awake()
    {
        _cachedCollider = GetComponent<Collider>();
    }

    public Vector3 CreateRandomPoint()
    {
        Bounds bounds = _cachedCollider.bounds;

        Vector3 randomPoint = new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z));

        return randomPoint;
    }
}