using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Istoreads
{
    public class Ship : MonoBehaviour
    {
        [SerializeField]
        private float _thrustSpeed = 1f;
        [SerializeField]
        private float _rotationtSpeed = 1f;
        private Controls _inputs;


        private void Awake()
        {
            _inputs = new Controls();
            _inputs.Player.Rotate.performed += Rotate;
            _inputs.Player.Thrust.performed += (context) => Thrust();
            _inputs.Player.Stop.performed += (context) => Stop();
            _inputs.Player.Shot.performed += (context) => Shot();
        }

        private void Shot()
        {
           
        }

        private void Stop()
        {
            
        }

        private void Rotate(InputAction.CallbackContext context)
        {
            Debug.Log(context.ReadValue<float>());
        }

        private void Thrust()
        {

        }

        // Start is called before the first frame update
        void Start()
        {
            _inputs.Player.Enable();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Debug.Log($"Bump {collision.gameObject.name}");
        }
    }
}