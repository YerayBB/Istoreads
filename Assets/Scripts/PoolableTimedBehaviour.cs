using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtilsUnknown.Extensions;

namespace UtilsUnknown
{
    public class PoolableTimedBehaviour : PoolableBehaviour
    {
        protected Coroutine _timeoutCoroutine = null;
        protected float _defaultTimeout = 10f;

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