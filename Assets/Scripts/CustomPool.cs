using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public class CustomPool<T> where T : MonoBehaviour
{
    private readonly int _maxPoolSize;

    private readonly T _prefab;
    private readonly List<T> _allObjects = new List<T>();
    private readonly Queue<T> _availableObjects = new Queue<T>();
    private readonly Action<T> _initializeAction;

    public CustomPool(T prefab, Action<T> initializeAction, int prewarmObjects, int maxPoolSize = 100)
    {
        if (prefab is null)
            throw new System.ArgumentNullException(nameof(prefab));

        _prefab = prefab;
        _initializeAction = initializeAction;
        _maxPoolSize = maxPoolSize;

        for (int i = 0; i < prewarmObjects; i++)
        {
            CreateNewObject();
        }
    }

    public T Get()
    {
        if (_availableObjects.Count > 0)
        {
            var @object = _availableObjects.Dequeue();
            InitializeObject(@object);

            return @object;
        }

        if (_allObjects.Count >= _maxPoolSize)
        {
            var oldesInactive = _allObjects
                .Where(obj => obj.gameObject.activeInHierarchy == false)
                .OrderBy(obj => obj.GetInstanceID())
                .FirstOrDefault();

            if(oldesInactive != null)
            {
                InitializeObject(oldesInactive);

                return oldesInactive;
            }

            return null;
        }

        return CreateNewObject();
    }

    private void InitializeObject(T @object)
    {
        @object.gameObject.SetActive(true);        
    }

    public void Release(T @object)
    {
        @object.gameObject.SetActive(false);

        _availableObjects.Enqueue(@object);
    }

    private T CreateNewObject()
    {
        var @object = Object.Instantiate(_prefab);
        _initializeAction?.Invoke(@object);
        @object.gameObject.SetActive(false);
        _allObjects.Add(@object);
        _availableObjects.Enqueue(@object);

        return @object;
    }
}