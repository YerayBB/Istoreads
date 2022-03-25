using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Istoreads
{
    public class PoolSystem : MonoBehaviour
    {
        public static PoolSystem Instance { get; private set; }

        [SerializeField]
        private int _vertexPoolInitCapacity;
        [SerializeField]
        private int _polygonPoolInitCapacity;
        [SerializeField]
        private int _bulletPoolCapacity;

        [SerializeField]
        private GameObject _vertexPrefab;
        [SerializeField]
        private GameObject _polygonPrefab;
        [SerializeField]
        private GameObject _bulletPrefab;

        private Stack<Vertex> _vertexPool = new Stack<Vertex>();
        private Stack<Polygon> _polygonPool = new Stack<Polygon>();
        private Stack<Bullet> _bulletPool = new Stack<Bullet>();

        private List<Bullet> _activeBullets = new List<Bullet>();

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

            for (int i = 0; i < _vertexPoolInitCapacity; ++i) PoolVertex();
            for (int i = 0; i < _polygonPoolInitCapacity; ++i) PoolPolygon();

            for (int i = 0; i < _bulletPoolCapacity; ++i) PoolBullet();

        }

        // Start is called before the first frame update
        void Start()
        {
            Vertex.OnDisabled += (vertex) => _vertexPool.Push(vertex);
            Polygon.OnDisabled += (polygon) => _polygonPool.Push(polygon);
            Bullet.OnDisabled += (bullet) =>
            {
                _activeBullets.Remove(bullet);
                _bulletPool.Push(bullet);
            };
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

        public Bullet GetBullet()
        {
            Bullet ret;
            if (_bulletPool.Count > 0)
            {
                ret = _bulletPool.Pop();
            }
            else
            {
                ret = _activeBullets[0];
                _activeBullets.RemoveAt(0);
            }
            _activeBullets.Add(ret);
            return ret;

        }

        private void PoolBullet()
        {
            GameObject spawn = Instantiate(_bulletPrefab);
            spawn.SetActive(false);
            _bulletPool.Push(spawn.GetComponent<Bullet>());
        }
    }
}
