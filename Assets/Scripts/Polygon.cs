using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Istoreads
{
    public class Polygon : MonoBehaviour
    {
        private LineRenderer _lineRenderer;
        private PolygonCollider2D _polygonCollider;
        private Transform _transform;

        private int _initialVertex;
        private int _vertexAmount;
        private float _radius;
        private bool[] _atachedVertex;

        public static System.Action OnDestroyed;


        private void Awake()
        {
            _transform = transform;
            _lineRenderer = GetComponent<LineRenderer>();
            _polygonCollider = GetComponent<PolygonCollider2D>();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        private void FixedUpdate()
        {

        }

        public void Initialize(int vertexAmount, Vector3 position, float radius)
        {
            _initialVertex = vertexAmount;
            _vertexAmount = vertexAmount;
            _atachedVertex = new bool[_initialVertex];
            _transform.position = position;
            _radius = radius;
            float angle = 2 * Mathf.PI / vertexAmount;
            Vector3 vertexPos;
            Vertex vertex;
            for(int i = 0; i < _vertexAmount; i++)
            {
                vertexPos = new Vector3(Mathf.Cos(angle * i) * _radius, Mathf.Sin(angle * i) * _radius);
                vertex = Spawner.Instance.GetVertex();
                vertex.OnKilled += DisableVertex;
                _atachedVertex[i] = true;
                vertex.Initialize(vertexPos, i, _transform);
            }
        }

        private void DisableVertex(int vertexID)
        {
            if (_atachedVertex[vertexID])
            {
                _atachedVertex[vertexID] = false;
                --_vertexAmount;
                CheckIntegrity();
            }
        }

        private void CheckIntegrity()
        {

        }

        public void Split(bool[] activeVertex, Transform[] vertexs, float radius)
        {
            _initialVertex = vertexs.Length;
            _vertexAmount = _initialVertex;
            _atachedVertex = activeVertex;
            _radius = radius;
            float angle = 2 * Mathf.PI /_vertexAmount;
            Vector3 vertexPos;
            Vertex vertex;
            for (int i = 0; i < _vertexAmount; i++)
            {
                vertexPos = new Vector3(Mathf.Cos(angle * i) * _radius, Mathf.Sin(angle * i) * _radius);
                vertex = vertexs[i].GetComponent<Vertex>();
                vertex.OnKilled += DisableVertex;
                vertex.Reatach(vertexPos, _transform);
            }
        }
    }
}