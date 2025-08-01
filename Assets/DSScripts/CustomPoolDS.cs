using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.DSScripts
{
    public class CustomPoolDS<T> where T : MonoBehaviour
    {
        private T _prefab;
        private List<T> _objects;

        public CustomPoolDS(T prefab, int prewarmObjects)
        {
            _prefab = prefab;
            _objects = new List<T>();

            for (int i = 0; i < prewarmObjects; i++)
            {
                var @object = GameObject.Instantiate(_prefab);
                @object.gameObject.SetActive(false);
                _objects.Add(@object);
            }
        }

        public T Get()
        {
            var @object = _objects.FirstOrDefault(x => !x.isActiveAndEnabled);

            if (@object == null)
                @object = Create();

            @object.gameObject.SetActive(true);

            return @object;
        }

        public void Release(T @object)
        {
            @object.gameObject.SetActive(false);
        }

        private T Create()
        {
            var @object = GameObject.Instantiate(_prefab);
            _objects.Add(@object);

            return @object;
        }
    }
}