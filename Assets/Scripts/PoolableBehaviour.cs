using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilsUnknown
{
    public abstract class PoolableBehaviour : MonoBehaviour, IPoolable
    {
        public event System.Action<IPoolable> OnDisabled;

        protected bool _init = false;

        public virtual void Disable()
        {
            if (_init)
            {
                gameObject.SetActive(false);
                _init = false;
                OnDisabled?.Invoke(this);
            }
        }

        public virtual void Initialize()
        {
            _init = false;
            gameObject.SetActive(true);
            _init = true;
        }

        protected virtual void OnDisabledTrigger(IPoolable item)
        {
            OnDisabled?.Invoke(item);
        }

    }
}
