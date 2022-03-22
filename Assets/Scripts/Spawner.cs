using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Istoreads
{
    public class Spawner : MonoBehaviour
    {
        public static Spawner Instance { get; private set; }

        [SerializeField]
        private GameObject _vertexPrefab;
        [SerializeField]
        private GameObject _polygonPrefab;

        private Stack<Vertex> _vertexPool;
        private Stack<Polygon> _polygonPool;

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }


        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public Vertex GetVertex()
        {
            if (_vertexPool.Count <= 0)
            {
                PoolVertex();
            }
            return _vertexPool.Pop();
        }

        private void PoolVertex()
        {
            GameObject spawn = Instantiate(_vertexPrefab);
            spawn.SetActive(false);
            _vertexPool.Push(spawn.GetComponent<Vertex>());
        }

        public Polygon GetPolygon()
        {
            if (_polygonPool.Count <= 0)
            {
                PoolPolygon();
            }
            return _polygonPool.Pop();
        }

        private void PoolPolygon()
        {
            GameObject spawn = Instantiate(_polygonPrefab);
            spawn.SetActive(false);
            _polygonPool.Push(spawn.GetComponent<Polygon>());
        }
    }
}
