using UnityEngine;

namespace UtilsUnknown
{
    //Base class for Monobehaviours that can be pooled
    public abstract class PoolableBehaviour : MonoBehaviour, IPoolable
    {
        protected bool _init = false;

        public event System.Action<IPoolable> OnDisabled;


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

        //Required to be able to call the event on a subclass
        protected virtual void OnDisabledTrigger(IPoolable item)
        {
            OnDisabled?.Invoke(item);
        }

    }
}
