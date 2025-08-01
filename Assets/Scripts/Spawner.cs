using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Rain Setting")]
    [SerializeField] private bool _isRaining = true;

    [Header("Spawn Settings")]
    [SerializeField, Min(2)] private int _numberOfCubesPerSecond = 5;
    [SerializeField] private int _maxPoolSize = 100;
    [SerializeField, Range(0.5f, 2.0f)] private float _spawnDelay = 1f;

    [Header("Deactivation Setting")]
    [SerializeField] private int _minValueDeactivation = 5;
    [SerializeField] private int _maxValueDeactivation = 10;

    [Header("Resources")]
    [SerializeField] private Cube _cubePrefab;
    [SerializeField] private SpawnZone _spawnZone;
    [SerializeField] private Material _standartMaterial;

    [Header("")]
    [SerializeField] private List<Material> _materials = new List<Material>();

    private float _disconnectDelay = 1f;
    private int _prewarmCubeCount = 30;
    private CustomPool<Cube> _pool;
    private Coroutine _spawnCoroutine;
    private Coroutine _disconnectionCoroutine;
    private List<Cube> _touchOfPlatformCubes = new List<Cube>();

    private void OnValidate()
    {
        if (_minValueDeactivation >= _maxValueDeactivation)
            _minValueDeactivation = _maxValueDeactivation - 1;

        if (_numberOfCubesPerSecond >= _maxPoolSize)
            _maxPoolSize = _numberOfCubesPerSecond + _numberOfCubesPerSecond;
    }

    private void Start()
    {
        _pool = new CustomPool<Cube>(_cubePrefab, _prewarmCubeCount, _maxPoolSize);
        _spawnCoroutine = StartCoroutine(SpawnRoutine());
        _disconnectionCoroutine = StartCoroutine(DisconnectionRoutine());
    }

    private void OnDisable()
    {
        _isRaining = false;
    }

    private IEnumerator SpawnRoutine()
    {
        while (_isRaining)
        {
            yield return new WaitForSeconds(_spawnDelay);

            Spawned();
        }
    }

    private IEnumerator DisconnectionRoutine()
    {
        while (_isRaining)
        {
            yield return new WaitForSeconds(_disconnectDelay);

            VerifyingCubesForDisconnection();
        }
    }

    private void Spawned()
    {
        for (int i = 0; i < _numberOfCubesPerSecond; i++)
        {
            Cube cube = _pool.Get();

            cube.CubeHitPlatform += ProcessTouchOfPlatformCube;

            ConfigureCube(cube);
        }
    }

    private Vector3 CreateRandomPoint()
    {
        Bounds bounds = _spawnZone.SpawnZoneCollider.bounds;

        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z));
    }

    private void ConfigureCube(Cube cube)
    {
        cube.ChangeShutdownCounter(GenerateCounterValue());

        Vector3 pointSpawn = CreateRandomPoint();
        cube.transform.position = pointSpawn;
    }

    private void ProcessTouchOfPlatformCube(Cube cube)
    {
        cube.ChangeMaterial(GetUniqueMaterial(cube.Material));

        _touchOfPlatformCubes.Add(cube);
    }

    private Material GetUniqueMaterial(Material original)
    {
        if (_materials.Count == 0)
            return CreateRandomMaterial();

        var available = _materials
            .FindAll(material => MaterialsEqual(material, original) == false);

        return available.Count > 0
            ? available[Random.Range(0, available.Count)]
            : _materials[Random.Range(0, _materials.Count)];
    }

    private bool MaterialsEqual(Material newMaterial, Material originalMaterial)
    {
        return newMaterial && originalMaterial && newMaterial.color == originalMaterial.color && newMaterial.mainTexture == originalMaterial.mainTexture;
    }

    private Material CreateRandomMaterial()
    {
        return new Material(Shader.Find("Standard")) { color = Random.ColorHSV() };
    }

    private void VerifyingCubesForDisconnection()
    {
        List<Cube> cubesForDisconnection = new List<Cube>();

        foreach (Cube cubes in _touchOfPlatformCubes)
        {
            if (cubes.ShutdownCounter > 0)
            {
                cubes.DecreaseCounterValue();
            }
            else
            {
                cubesForDisconnection.Add(cubes);
            }
        }

        if (cubesForDisconnection.Count > 0)
        {
            _touchOfPlatformCubes = _touchOfPlatformCubes.Except(cubesForDisconnection).ToList();
            CubesDisconnection(cubesForDisconnection);
        }
    }

    private void CubesDisconnection(List<Cube> cubes)
    {
        foreach (Cube cube in cubes)
        {
            cube.CubeHitPlatform -= ProcessTouchOfPlatformCube;

            ResetConfigureCube(cube);

            _pool.Release(cube);
        }
    }

    private void ResetConfigureCube(Cube cube)
    {
        cube.ResetPlatformTuch();
        cube.transform.rotation = Quaternion.identity;

        if (cube.GetComponent<Rigidbody>() != null)
        {
            cube.GetComponent<Rigidbody>().velocity = Vector3.zero;
            cube.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }

        if (cube.GetComponent<Renderer>() != null)
        {
            cube.GetComponent<Renderer>().material = _standartMaterial;
        }
    }

    private int GenerateCounterValue()
    {
        return Random.Range(_minValueDeactivation, _maxValueDeactivation);
    }
}