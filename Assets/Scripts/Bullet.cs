using UnityEngine;
using UtilsUnknown.Extensions;
using UtilsUnknown;

namespace Istoreads
{
    [RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
    public class Bullet : PoolableTimedBehaviour
    {
        [SerializeField]
        private float _speed;

        private Rigidbody2D _rigidbody;
        private Transform _transform;


        #region MonoBehaviourCalls

        private void Awake()
        {
            _transform = transform;
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        #endregion

        public void Initialize(Vector3 pos, Quaternion rotation, Vector2 dir, float speed, float aliveTime)
        {
            _init = false;
            if (_timeoutCoroutine != null) StopCoroutine(_timeoutCoroutine);
            _transform.position = pos;
            _transform.rotation = rotation;
            _speed = speed;
            gameObject.SetActive(true);
            _rigidbody.velocity = dir * _speed;
            _timeoutCoroutine = this.DelayedCall(Disable, aliveTime);
            _init = true;
        }

        public override void Disable()
        {
            if (_init)
            {
                if (_timeoutCoroutine != null) StopCoroutine(_timeoutCoroutine);
                _timeoutCoroutine = null;
                gameObject.SetActive(false);
                _rigidbody.velocity = Vector2.zero;
                OnDisabledTrigger(this);
                _init = false;
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Disable();
        }
    }
}