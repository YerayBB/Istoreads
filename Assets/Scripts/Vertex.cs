using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Istoreads
{
    public class Vertex : MonoBehaviour
    {
        private int _number;

        public static event System.Action<Vertex> OnDestroyed;

        public event System.Action<int> OnKilled = null;

        private Transform _transform;
        private Vector3? _finalPos = null;


        private void Awake()
        {
            _transform = transform;
            Debug.Log($"Vertex Awaken {gameObject.GetInstanceID()}");
            //gameObject.SetActive(false);
        }

        private void FixedUpdate()
        {
            if(_finalPos != null)
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

        public void Initialize(Vector3 pos, int number, Transform parent)
        {
            _transform.parent = parent;
            _transform.localPosition = pos;
            _number = number;
            _finalPos = null;
            gameObject.SetActive(true);
        }

        public void Reatach(Vector3 pos, int number, Transform parent)
        {
            _transform.parent = parent;
            _finalPos = pos;
            _number = number;
        }

        private void Death()
        {
            gameObject.SetActive(false);
            _transform.parent = null;
            OnKilled?.Invoke(_number);
            OnKilled = null;
            OnDestroyed?.Invoke();
        }
    }
}