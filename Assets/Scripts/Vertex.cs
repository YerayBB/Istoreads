using UnityEngine;
using UtilsUnknown;

namespace Istoreads
{
    public class Vertex : PoolableBehaviour
    {
        private Transform _transform;

        private int _number;
        private Vector3? _finalPos = null;

        public static event System.Action OnDestroyed;
        public static event System.Action<Vector3> OnDestroyedAt;
        public event System.Action<int> OnKilled = null;


        #region MonoBehaviourCalls

        private void Awake()
        {
            _transform = transform;
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

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (_init)
            {
                Death();
            }
        }

        #endregion

        //Initialize the vertex as new with the given data
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

        //Attach to another parent while being active
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
            gameObject.SetActive(false);
            _transform.parent = null;
            OnKilled?.Invoke(_number);
            OnKilled = null;
            OnDestroyed?.Invoke();
            OnDestroyedAt?.Invoke(_transform.position);
            OnDisabledTrigger(this);
        }

        public static void ResetEvents()
        {
            OnDestroyed = null;
            OnDestroyedAt = null;
        }
    }
}