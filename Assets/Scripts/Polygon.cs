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

        public static event System.Action<Polygon> OnDestroyed;


        private void Awake()
        {
            _transform = transform;
            _lineRenderer = GetComponent<LineRenderer>();
            _polygonCollider = GetComponent<PolygonCollider2D>();
            Debug.Log($"Polygon Awaken {gameObject.GetInstanceID()}");
            //gameObject.SetActive(false);
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        private void Update()
        {
            Vector2[] path = new Vector2[_vertexAmount];
            _lineRenderer.positionCount = _vertexAmount;
            for(int i = 0; i < _vertexAmount; ++i)
            {
                path[i] = _transform.GetChild(i).localPosition;
                _lineRenderer.SetPosition(i, path[i]);
            }
            _polygonCollider.SetPath(0, path);
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
            for(int i = 0; i < _vertexAmount; ++i)
            {
                vertexPos = new Vector3(Mathf.Cos(angle * i) * _radius, Mathf.Sin(angle * i) * _radius);
                vertex = PoolSystem.Instance.GetVertex();
                vertex.OnKilled += DisableVertex;
                _atachedVertex[i] = true;
                vertex.Initialize(vertexPos, i, _transform);
            }
            gameObject.SetActive(true);
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
            if(_vertexAmount == 0)
            {
                Death();
            }
            else if(_vertexAmount > 5)
            {
                List<Vector2Int> fragments = new List<Vector2Int>(); 
                int start = -1;
                //Detect Fragments
                for(int i = 0; i < _initialVertex; ++i)
                {
                    if (_atachedVertex[i])
                    {
                        if (start != -1)
                        {
                            fragments.Add(new Vector2Int(start, i));
                            start = -1;
                        }
                        else
                        {
                            start = i;
                        }
                    }
                    else
                    {
                        if (start != -1)
                        {
                            fragments.Add(new Vector2Int(start, start));
                            start = -1;
                        }
                    }
                }
                //check last and first are connected
                if(fragments.Count > 1)
                {
                    if(_initialVertex - 1 - fragments[fragments.Count-1].y + fragments[0].x == 0)
                    {
                        fragments[0] = new Vector2Int(fragments[fragments.Count - 1].x, fragments[0].y);
                        fragments.RemoveAt(fragments.Count - 1);
                    }
                }

                //join small fragments
                int numberFragments = fragments.Count;
                int j = 0;
                int vertexCount = 0;
                Vector2Int target;
                while (j < numberFragments) 
                {
                    //Detect how many active vertex contains the fragment
                    target = fragments[j];
                    vertexCount = 0;

                    //case fragment includes end and origin vertex
                    if (target.x > target.y)
                    {
                        for (int i = target.x; i < _initialVertex; ++i)
                        {
                            if (_atachedVertex[i]) ++vertexCount;
                        }
                        for (int i = 0; i <= target.y; ++i)
                        {
                            if (_atachedVertex[i]) ++vertexCount;
                        }
                    }
                    else {

                        for (int i = target.x; i <= target.y; ++i)
                        {
                            if (_atachedVertex[i]) ++vertexCount;
                        }
                    }

                    //if fragment is small, join the fragent to the closest fragment
                    if(vertexCount < 3)
                    {
                        int indexDown = (j - 1 + numberFragments) % numberFragments;
                        int indexUp = (j + 1) % numberFragments;
                        if (indexDown != indexUp)
                        {
                            if (Mathf.Abs(target.x - fragments[indexDown].y) <= Mathf.Abs(fragments[indexUp].x - target.y))
                            {
                                fragments[indexDown] = new Vector2Int(fragments[indexDown].x, target.y);
                            }
                            else
                            {
                                fragments[indexUp] = new Vector2Int(target.x, fragments[indexUp].y);
                            }
                        }
                        else
                        {
                            fragments[indexDown] = new Vector2Int(fragments[indexDown].x, target.y);
                        }
                        fragments.RemoveAt(j);
                        --numberFragments;
                    }
                    else
                    {
                        ++j;
                    }
                }

                if(numberFragments > 1)
                {
                    //build the transform array

                    Vector2Int range = fragments[1];
                    int amount = 0;
                    Transform[] vertexs = new Transform[_initialVertex];
                    int subIndex = 0;

                    //case range includes end and origin
                    if (range.x > range.y)
                    {
                        for (int i = 0; i < _initialVertex; ++i)
                        {
                            if (_atachedVertex[i])
                            {
                                if (i >= range.y && i <= range.x)
                                {
                                    vertexs[i] = _transform.GetChild(subIndex);
                                    ++amount;
                                }
                                else
                                {
                                    vertexs[i] = null;
                                }
                                ++subIndex;
                            }
                            else
                            {
                                vertexs[i] = null;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < _initialVertex; ++i)
                        {
                            if (_atachedVertex[i])
                            {
                                if (i >= range.x && i <= range.y)
                                {
                                    vertexs[i] = _transform.GetChild(subIndex);
                                    vertexs[i].GetComponent<Vertex>().OnKilled -= DisableVertex;
                                    ++amount;
                                }
                                else
                                {
                                    vertexs[i] = null;
                                }
                                ++subIndex;
                            }
                            else
                            {
                                vertexs[i] = null;
                            }
                        }
                    }
                    PoolSystem.Instance.GetPolygon().Split(amount, vertexs, _radius);

                }
            }
        }

        public void Split(int amount, Transform[] vertexs, float radius)
        {
            _initialVertex = amount;
            _vertexAmount = _initialVertex;
            _atachedVertex = new bool[_initialVertex];
            _radius = radius;
            float angle = 2 * Mathf.PI /_vertexAmount;
            Vector3 vertexPos;
            Vertex vertex;
            int subIndex = 0;
            while (vertexs[subIndex] == null && subIndex < vertexs.Length)
            {
                ++subIndex;
            }
            for (int i = 0; i < _vertexAmount; ++i)
            {
                if (subIndex >= vertexs.Length)
                {
                    Debug.Log("Error in split, Transform array incomplete");
                    return;
                }
                vertexPos = new Vector3(Mathf.Cos(angle * i) * _radius, Mathf.Sin(angle * i) * _radius);

                vertex = vertexs[subIndex].GetComponent<Vertex>();
                vertex.OnKilled += DisableVertex;
                vertex.Reatach(vertexPos, i, _transform);
                ++subIndex;
                while (vertexs[subIndex] == null && subIndex < vertexs.Length)
                {
                    ++subIndex;
                }
            }
            gameObject.SetActive(true);
        }

        private void Death()
        {
            gameObject.SetActive(false);
            OnDestroyed?.Invoke(this);
        }
    }
}