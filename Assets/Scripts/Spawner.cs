using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    [Header("Rain Setting")]
    [SerializeField] private bool _isRaining = true;

    [Header("Spawn Settings")]
    [SerializeField, Min(2)] private int _numberOfCubesPerSecond = 5;
    [SerializeField] private int _maxPoolSize = 100;
    [SerializeField, Range(0.5f, 2.0f)] private float _spawnDelay = 1f;

    [Header("Resources")]
    [SerializeField] private Cube _cubePrefab;
    [SerializeField] private SpawnZone _spawnZone;
    [SerializeField] private ColorChanger _colorChanger;

    private int _prewarmCubeCount = 30;
    private Coroutine _spawnCoroutine;
    private CustomPool<Cube> _pool;
    private List<Cube> _touchOfPlatformCubes = new List<Cube>();

    private void OnValidate()
    {
        if (_numberOfCubesPerSecond >= _maxPoolSize)
            _maxPoolSize = _numberOfCubesPerSecond + _numberOfCubesPerSecond;
    }

    private void Start()
    {
        _pool = new CustomPool<Cube>(_cubePrefab, _prewarmCubeCount, _maxPoolSize);
        _spawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    private void OnDisable()
    {
        _isRaining = false;

        if (_spawnCoroutine != null)
        {
            StopCoroutine(_spawnCoroutine);
            _spawnCoroutine = null;
        }
    }

    private IEnumerator SpawnRoutine()
    {
        while (_isRaining)
        {
            yield return new WaitForSeconds(_spawnDelay);

            Spawned();
        }
    }

    private void Spawned()
    {
        for (int i = 0; i < _numberOfCubesPerSecond; i++)
        {
            Cube cube = _pool.Get();

            ConfigureCube(cube);

            cube.CubeTimerHasEnded += CubesDisconnection;
        }
    }

    private void ConfigureCube(Cube cube)
    {
        cube.Initialization(_colorChanger);

        Vector3 pointSpawn = CreateRandomPoint();
        cube.transform.position = pointSpawn;
    }

    private Vector3 CreateRandomPoint()
    {
        Bounds bounds = _spawnZone.SpawnZoneCollider.bounds;

        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z));
    }

    private void CubesDisconnection(Cube cube)
    {
        cube.CubeTimerHasEnded -= CubesDisconnection;

        ResetConfigureCube(cube);

        _touchOfPlatformCubes.Remove(cube);

        _pool.Release(cube);
    }

    private void ResetConfigureCube(Cube cube)
    {
        cube.Reset();

        cube.transform.rotation = Quaternion.identity;

        if (cube.TryGetComponent<Rigidbody>(out Rigidbody rigidbody))
        {
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
        }
    }
}