using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtilsUnknown;

namespace Istoreads
{
    public class Vertex : PoolableBehaviour
    {
        private int _number;

        public static event System.Action OnDestroyed;

        public event System.Action<int> OnKilled = null;

        private Transform _transform;
        private Vector3? _finalPos = null;

        private void Awake()
        {
            _transform = transform;
            //gameObject.SetActive(false);
        }

        private void FixedUpdate()
        {
            if (_init)
            {
                if (_finalPos != null)
                {
                    Vector3 dir = (Vector3)_finalPos - _transform.localPosition;
                    if (dir.magnitude > 1)
                    {
                        dir.Normalize();
                        _transform.localPosition += dir;
                    }
                    else
                    {
                        _transform.localPosition = (Vector3)_finalPos;
                        _finalPos = null;
                    }
                }
            }
        }

        public void Initialize(Vector3 pos, int number, Transform parent)
        {
            _init = false;
            _transform.parent = parent;
            _transform.localPosition = pos;
            _number = number;
            _finalPos = null;
            _init = true;
            gameObject.SetActive(true);
        }

        public void Reatach(Vector3 pos, int number, Transform parent)
        {
            _transform.parent = parent;
            _finalPos = pos;
            _number = number;
        }

        public override void Disable()
        {
            if (_init)
            {
                _init = false;
                gameObject.SetActive(false);
                OnKilled = null;
                OnDisabledTrigger(this);
            }
        }

        public void Death()
        {
            _init = false;
            //Debug.Log($"{this.GetInstanceID()} Death");
            gameObject.SetActive(false);
            _transform.parent = null;
            OnKilled?.Invoke(_number);
            OnKilled = null;
            OnDestroyed?.Invoke();
            OnDisabledTrigger(this);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (_init)
            {
                //Debug.Log($"Death by collision with: " + collision.gameObject.name);
                Death();
            }
        }
    }
}