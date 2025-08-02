using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[SelectionBase]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Renderer))]
public class Cube : MonoBehaviour
{
    private int _shutdownCounter;
    private int _minValueDeactivation = 2;
    private int _maxValueDeactivation = 5;
    private bool _isHitPlatform = false;

    private Rigidbody _cacheRigidbody;
    private Collider _cacheCollider;
    private Renderer _cacheRenderer;
    private ColorChanger _colorChanger;

    public event Action<Cube> CubeTimerHasEnded;

    private Coroutine _decreaseValueCoroutine;

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
            {
                _isHitPlatform = true;

                _colorChanger.ChangeColor(this);

                _decreaseValueCoroutine = StartCoroutine(DecreaseValueRoutine());
            }
        }
    }

    public void Reset()
    {
        _colorChanger.ResetMaterial(this);

        if (_decreaseValueCoroutine != null)
        {
            StopCoroutine(_decreaseValueCoroutine);
            _decreaseValueCoroutine = null;
        }
    }

    private IEnumerator DecreaseValueRoutine()
    {
        yield return new WaitForSeconds(_shutdownCounter);

        CubeTimerHasEnded?.Invoke(this);
    }

    public void ChangeMaterial(Material material)
    {
        if (material == null)
            throw new NullReferenceException(nameof(material));

        Material = material;
    }

    public void Initialization(ColorChanger colorChanger)
    {
        _colorChanger = colorChanger;

        _shutdownCounter = GenerateCounterValue();

        _isHitPlatform = false;
    }

    private int GenerateCounterValue()
    {
        return Random.Range(_minValueDeactivation, _maxValueDeactivation + 1);
    }
}