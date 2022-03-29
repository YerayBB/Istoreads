using System.Collections.Generic;
using UnityEngine;
using UtilsUnknown;
using UtilsUnknown.Extensions;

namespace Istoreads
{
    [RequireComponent(typeof(LineRenderer), typeof(PolygonCollider2D))]
    public class Polygon : PoolableTimedBehaviour
    {
        private LineRenderer _lineRenderer;
        private PolygonCollider2D _polygonCollider;
        private Transform _transform;

        private int _initialVertex;
        private int _vertexAmount;
        private float _radius;
        private bool[] _atachedVertex;
        private Vector2 _direction = Vector2.zero;
        private float _speed = 6;
        private int _hits = 0;

        public static event System.Action OnDestroyed;

        
        #region MonoBehaviourCalls

        private void Awake()
        {
            _transform = transform;
            _lineRenderer = GetComponent<LineRenderer>();
            _polygonCollider = GetComponent<PolygonCollider2D>();
        }

        private void Update()
        {
            if (_init)
            {
                int aux = _transform.childCount;
                Vector2[] path = new Vector2[aux];
                _lineRenderer.positionCount = aux;
                for (int i = 0; i < aux; ++i)
                {
                    path[i] = _transform.GetChild(i).localPosition;
                    _lineRenderer.SetPosition(i, path[i]);
                }
                _polygonCollider.SetPath(0, path);
            }
        }

        private void FixedUpdate()
        {
            if (_init)
            {
                _transform.position += (Vector3)_direction*_speed * Time.fixedDeltaTime;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            ++_hits;
            if (_hits >= _vertexAmount)
            {
                int target = Random.Range(0, _transform.childCount);
                _transform.GetChild(target).GetComponent<Vertex>().Death();
                _hits = 0;
            }
        }

        #endregion

        //Activate the polygon as new with all given data
        public void Initialize(int vertexAmount, Vector3 position, Vector2 direction, float speed, float radius, float timeAlive)
        {
            if (_timeoutCoroutine != null) StopCoroutine(_timeoutCoroutine);
            _init = false;
            _initialVertex = vertexAmount;
            _vertexAmount = vertexAmount;
            _atachedVertex = new bool[_initialVertex];
            _transform.position = position;
            _transform.rotation = Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.forward);
            _direction = direction.normalized;
            _speed = speed;
            _radius = radius;
            _defaultTimeout = timeAlive;
            float angle = 2 * Mathf.PI / vertexAmount;
            Vector3 vertexPos;
            Vertex vertex;
            //Attach all needed vertex
            for(int i = 0; i < _vertexAmount; ++i)
            {
                vertexPos = new Vector3(Mathf.Cos(angle * i) * _radius, Mathf.Sin(angle * i) * _radius);
                vertex = GameManager.Instance.GetVertex();
                vertex.OnKilled += LostVertex;
                _atachedVertex[i] = true;
                vertex.Initialize(vertexPos, i, _transform);
            }
            gameObject.SetActive(true);
            _timeoutCoroutine = this.DelayedCall(Disable, _defaultTimeout);
            _init = true;
        }

        //Activate the polygon as a fraction of other, with the given vertexs from the other, and the given data
        public void Split(int amount, Transform[] vertexs, Vector3 position, Vector2 direction, float speed, float radius, float timeAlive)
        {
            if (_timeoutCoroutine != null) StopCoroutine(_timeoutCoroutine);
            _init = false;
            _initialVertex = amount;
            _transform.position = position;
            _direction = direction.normalized;
            _speed = speed;
            _vertexAmount = _initialVertex;
            _atachedVertex = new bool[_initialVertex];
            _radius = radius;
            _defaultTimeout = timeAlive;
            float angle = 2 * Mathf.PI / _vertexAmount;
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

                vertex.OnKilled += LostVertex;
                vertex.Reatach(vertexPos, i, _transform);
                ++subIndex;
                while (subIndex < vertexs.Length && vertexs[subIndex] == null)
                {
                    ++subIndex;
                }
                _atachedVertex[i] = true;
            }
            gameObject.SetActive(true);
            _timeoutCoroutine = this.DelayedCall(Disable, _defaultTimeout);
            _init = true;
        }

        public override void Disable()
        {
            if (_init)
            {
                if (_timeoutCoroutine != null) StopCoroutine(_timeoutCoroutine);
                _timeoutCoroutine = null;
                gameObject.SetActive(false);
                _init = false;
                Vertex vertex;
                //disable all attached vertex
                while (_transform.childCount > 0)
                {
                    Transform child = _transform.GetChild(0);
                    child.parent = null;
                    if (child.TryGetComponent<Vertex>(out vertex))
                    {
                        vertex.Disable();
                    }
                }
                OnDisabledTrigger(this);
            }
        }

        //mark vertex vertexID as not active in the polygon 
        private void LostVertex(int vertexID)
        {
            if (_atachedVertex[vertexID])
            {
                _atachedVertex[vertexID] = false;
                --_vertexAmount;
                CheckIntegrity();
            }
        }

        //check if a new polygon can be formed with the vertexs grouped between the gaps in the polygon
        private void CheckIntegrity()
        {
            //make sure the tracking of vertex is correct
            if (_vertexAmount != _transform.childCount)
            {
                //had a bug that required this cond to not break,
                //now its fixed but doesnt hurt to keep the cond just in case
                _vertexAmount = _transform.childCount;
            }

            if(_vertexAmount == 0)
            {
                Death();
            }

            //can only divide into 2 polygons if the remaining vertex add up to at least 6
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
                            fragments.Add(new Vector2Int(start, i-1));
                            start = -1;
                        }
                        else
                        {
                            
                        }
                    }
                }

                //case last vertex = limit
                if (start != -1)
                {
                    fragments.Add(new Vector2Int(start, _initialVertex -1));
                    start = -1;
                }

                //check last and first are connected
                if (fragments.Count > 1)
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
                    //default case
                    else {

                        for (int i = target.x; i <= target.y; ++i)
                        {
                            if (_atachedVertex[i]) ++vertexCount;
                        }
                    }

                    //if fragment is small, join the fragment to the closest fragment
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

                //if there's more than 1 fragment, split the polygon in 2
                if(numberFragments > 1)
                {
                    //build the transform array of the vertex to pass to the new polygon
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
                    //default case
                    else
                    {
                        for (int i = 0; i < _initialVertex; ++i)
                        {
                            if (_atachedVertex[i])
                            {
                                if (i >= range.x && i <= range.y)
                                {
                                    vertexs[i] = _transform.GetChild(subIndex);
                                    vertexs[i].GetComponent<Vertex>().OnKilled -= LostVertex;
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
                    _vertexAmount -= amount;
                    //update the _attachedVertex
                    for(int i = 0; i < _initialVertex; ++i)
                    {
                        _atachedVertex[i] = _atachedVertex[i] && vertexs[i] == null;
                    }
                    //assign a variation on the trajectory of the 2 new polygons
                    Quaternion rot = Quaternion.AngleAxis(45, Vector3.forward);
                    Quaternion rotInv = Quaternion.AngleAxis(-45, Vector3.forward);
                    GameManager.Instance.GetPolygon().Split(amount, vertexs, _transform.position, rot*_direction, _speed, _radius, _defaultTimeout);
                    _direction = rotInv * _direction;
                }
            }
        }

        private void Death()
        {
            _init = false;
            if (_timeoutCoroutine != null) StopCoroutine(_timeoutCoroutine);
            _timeoutCoroutine = null;
            gameObject.SetActive(false);
            OnDestroyed?.Invoke();
            OnDisabledTrigger(this);
        }

        public static void ResetEvents()
        {
            OnDestroyed = null;
        }
    }
}