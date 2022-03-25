using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtilsUnknown.Extensions;

namespace Istoreads
{
    [RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
    public class Bullet : MonoBehaviour
    {
        public static System.Action<Bullet> OnDisabled;

        [SerializeField]
        private float _speed;

        private Rigidbody2D _rigidbody;
        private Transform _transform;

        private Coroutine _aliveCoroutine = null;

        private void Awake()
        {
            _transform = transform;
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Initialize(Vector3 pos, Quaternion rotation, Vector2 dir, float speed, float aliveTime)
        {
            if (_aliveCoroutine != null) StopCoroutine(_aliveCoroutine);
            _transform.position = pos;
            _transform.rotation = rotation;
            _speed = speed;
            gameObject.SetActive(true);
            _rigidbody.velocity = dir * _speed;
            _aliveCoroutine = this.DelayedCall(Death, aliveTime);

        }

        private void Death()
        {
            if (_aliveCoroutine != null) StopCoroutine(_aliveCoroutine);
            _aliveCoroutine = null;
            gameObject.SetActive(false);
            _rigidbody.velocity = Vector2.zero;
            OnDisabled?.Invoke(this);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Death();
        }
    }
}