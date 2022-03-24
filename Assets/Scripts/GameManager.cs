using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Istoreads
{
    public class GameManager : MonoBehaviour
    {
        private int _score = 0;

        // Start is called before the first frame update
        void Start()
        {
            PoolSystem.Instance.GetPolygon().Initialize(Random.Range(8, 15), new Vector3(Random.Range(-15, 15), Random.Range(-15, 15), 0), Random.Range(2, 4));
            Polygon.OnDestroyed += (polygon) => _score += 100;
            Vertex.OnDestroyed += (vertex) => _score += 1;
        }

        // Update is called once per frame
        void Update()
        {
            //Debug.Log(Time.time);
            if (Mathf.Round(Time.time % 20) == 0)
            {
                Debug.Log("SPAWN");
                PoolSystem.Instance.GetPolygon().Initialize(Random.Range(8, 15), new Vector3(Random.Range(-15, 15), Random.Range(-15, 15), 0), Random.Range(2, 4));
            }
        }
    }
}
