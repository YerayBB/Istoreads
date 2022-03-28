using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilsUnknown
{
    public class PoolMono<T> where T : PoolableBehaviour
    {
        private Stack<T> _pool;
        private List<T> _activeItems;
        private uint _limitCapacity = 0;
        private GameObject _prefab;

        private PoolMono() { }

        public PoolMono(GameObject prefab)
        {
            if (prefab == null) throw new System.ArgumentNullException();
            if(prefab.GetComponent<T>() != null)
            {
                _prefab = prefab;
            }
            else
            {
                throw new MissingComponentException($"The given GameObject does not contain a Component of type {typeof(T).ToString()}");
            }
            _pool = new Stack<T>();
        }

        public PoolMono(GameObject prefab, uint maxCapacity) : this(prefab)
        {
            _limitCapacity = maxCapacity;
            if (_limitCapacity != 0)
            {
                _activeItems = new List<T>();
            }
        }

        public void Init(uint amount)
        {
            uint cap = amount > _limitCapacity ? _limitCapacity : amount;
            for(uint i = 0; i< cap; ++i)
            {
                PoolItem();
            }
        }

        public T GetItem()
        {
            if (_pool.Count <= 0)
            {
                if(_limitCapacity == 0)
                {
                    PoolItem();
                }
                else
                {
                    T ret;
                    if (_pool.Count + _activeItems.Count < _limitCapacity)
                    {
                        PoolItem();
                        ret = _pool.Pop();
                    }
                    else
                    {
                        ret = _activeItems[0];
                        _activeItems.RemoveAt(0);
                    }
                    _activeItems.Add(ret);
                    return ret;
                }
            }
            return _pool.Pop();
        }

        private void PoolItem()
        {
            GameObject spawn = Object.Instantiate(_prefab);
            spawn.SetActive(false);
            T component = spawn.GetComponent<T>();
            component.OnDisabled += (item) =>
            {
                if (_limitCapacity != 0) _activeItems.Remove((T)item);
                _pool.Push((T)item);
            };
            _pool.Push(component);
        }
    }
}