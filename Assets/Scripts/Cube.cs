using System;
using UnityEngine;

[SelectionBase]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Renderer))]
public class Cube : MonoBehaviour
{
    private int _shutdownCounter;

    private Rigidbody _cacheRigidbody;
    private Collider _cacheCollider;
    private Renderer _cacheRenderer;
    private bool _isHitPlatform = false;

    public event Action<Cube> CubeHitPlatform;

    public int ShutdownCounter => _shutdownCounter;

    public Material Material
    {
        get => _cacheRenderer.material;
        private set => _cacheRenderer.material = value;
    }

    private void Awake()
    {
        _cacheRigidbody = GetComponent<Rigidbody>();
        _cacheCollider = GetComponent<Collider>();
        _cacheRenderer = GetComponent<Renderer>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.rigidbody.TryGetComponent(out Platform platform))
        {
            if (_isHitPlatform == false)
                CubeHitPlatform?.Invoke(this);

            _isHitPlatform = true;
        }
    }

    public void DecreaseCounterValue()
    {
        _shutdownCounter--;
    }

    public void ChangeMaterial(Material material)
    {
        Material = material;
    }

    public void ChangeShutdownCounter(int number)
    {
        _shutdownCounter = number;
    }

    public void ResetPlatformTuch()
    {
        _isHitPlatform = false;
    }
}