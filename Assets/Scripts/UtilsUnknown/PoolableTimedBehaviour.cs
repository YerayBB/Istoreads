using UnityEngine;
using UtilsUnknown.Extensions;

namespace UtilsUnknown
{
    //Base class for MonoBehaviours that can be pooled and have expiration time
    public class PoolableTimedBehaviour : PoolableBehaviour
    {
        protected float _defaultTimeout = 10f;
        protected Coroutine _timeoutCoroutine = null;


        public override void Initialize()
        {
            if (_timeoutCoroutine != null)
            {
                StopCoroutine(_timeoutCoroutine);
            }
            base.Initialize();
            _timeoutCoroutine = this.DelayedCall(Disable, _defaultTimeout);
        }

        public override void Disable()
        {
            if (_timeoutCoroutine != null)
            {
                StopCoroutine(_timeoutCoroutine);
                _timeoutCoroutine = null;
            }
            base.Disable();
        }
    }
}