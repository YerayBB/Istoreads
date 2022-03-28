using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UtilsUnknown;

namespace Istoreads
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Ship : MonoBehaviour
    {
        [SerializeField]
        private float _thrustSpeed = 1f;
        [SerializeField]
        private float _rotationtSpeed = 1f;
        [SerializeField]
        private GameObject _bulletPrefab;
        [SerializeField]
        private float _bulletTime = 10;
        [SerializeField]
        private uint _maxBullets = 20;
        [SerializeField]
        private uint _initBullets = 10;
        [SerializeField]
        private float _bulletSpeed = 100f;

        private Controls _inputs;
        private Rigidbody2D _rigidBody;
        private Transform _transform;
        private PoolMono<Bullet> _bulletPool;

        private bool _thrusting;
        private float _torque = 0;


        private void Awake()
        {
            _rigidBody = GetComponent<Rigidbody2D>();
            _transform = transform;

            _bulletPool = new PoolMono<Bullet>(_bulletPrefab, _maxBullets);
            _bulletPool.Init(_initBullets);

            _inputs = new Controls();
            _inputs.Player.RotateMouse.performed += RotateMouse;
            _inputs.Player.Rotate.performed += (context) => _torque = context.ReadValue<float>();
            _inputs.Player.Rotate.canceled += (context) => _torque = 0;
            _inputs.Player.Thrust.performed += (context) => _thrusting = true;
            _inputs.Player.Thrust.canceled += (context) => _thrusting = false;
            _inputs.Player.Stop.performed += (context) => Stop();
            _inputs.Player.Shot.performed += (context) => Shot();
        }

        private void Shot()
        {
            _bulletPool.GetItem().Initialize(_transform.position, _transform.rotation, _transform.up, _bulletSpeed, _bulletTime);
        }

        private void Stop()
        {
            _rigidBody.velocity = Vector2.zero;
            _rigidBody.angularVelocity = 0;
        }

        private void RotateMouse(InputAction.CallbackContext context)
        {
            Rotate(context.ReadValue<float>());
        }

        private void Rotate(float dir)
        {
            //Debug.Log("Rotate");
            _rigidBody.AddTorque(dir * _rotationtSpeed);
        }

        private void Thrust()
        {
            //Debug.Log("Thrust");
            _rigidBody.AddForce(_transform.up * _thrustSpeed);
        }

        // Start is called before the first frame update
        void Start()
        {
            _inputs.Player.Enable();
        }

        private void FixedUpdate()
        {
            if (_thrusting)
            {
                Thrust();
            }
            if(_torque != 0)
            {
                Rotate(_torque);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Debug.Log($"Bump {collision.gameObject.name}");
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Debug.Log($"Ouch {collision.gameObject.name}");
        }
    }
}