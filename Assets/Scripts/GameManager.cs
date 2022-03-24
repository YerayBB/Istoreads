using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Istoreads
{
    public class GameManager : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            PoolSystem.Instance.GetPolygon().Initialize(Random.Range(8, 15), new Vector3(Random.Range(-15, 15), Random.Range(-15, 15), 0), Random.Range(2, 4));
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
