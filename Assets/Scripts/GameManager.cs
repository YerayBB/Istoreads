using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UtilsUnknown.Extensions;


namespace Istoreads
{
    public class GameManager : MonoBehaviour
    {
        private TMP_Text _textScore;
        private int _score = 0;
        private TMP_Text _textLevel;
        private int _level = 0;

        [Header("Spawner config")]
        [SerializeField]
        private float _spawnRate = 5f;
        [SerializeField]
        private float _minRate = 1;
        [SerializeField]
        private float _spawnDistance = 22;
        [SerializeField]
        private Vector3 _spawnCenter = Vector3.zero;
        [SerializeField]
        private Vector2 _polygonVertexRange = new Vector2(3, 5);
        [SerializeField]
        private Vector2 _polygonVertexLimits = new Vector2(8, 20);
        [SerializeField]
        private float _polygonGrowth;
        [SerializeField]
        private Vector2 _polygonRadius = new Vector2(2, 4);
        [SerializeField]
        private Vector2 _polygonRadiusLimits = new Vector2(1,10);
        [SerializeField]
        private Vector2 _polygonSpeedRange = new Vector2(2, 4);
        [SerializeField]
        private float _waveDensity;
        [SerializeField]
        private float _maxDensity;
        [SerializeField]
        private float _waveGrowth;
        [SerializeField]
        private float _trajectoryVariance = 15;

        private int _cycles = 0;
        private int _vertexDestroyed = 0;
        


        private void Awake()
        {
            _textScore = transform.GetChild(0).GetComponent<TMP_Text>();
        }

        // Start is called before the first frame update
        void Start()
        {
            UpdateScore();
           
            Polygon.OnDestroyed += () =>
            {
                _score += 100;
                UpdateScore();
            };
            Vertex.OnDestroyed += () =>
            {
                _score += 1;
                ++_vertexDestroyed; 
                UpdateScore();
            };

            StartCoroutine(SpawnWave());
        }

        // Update is called once per frame
        void Update()
        {
            //Debug.Log(Time.time);
            /*if (Mathf.Round(Time.time % 20) == 0)
            {
                Debug.Log("SPAWN");
                var aux = new Vector3(Random.Range(-15, 15), Random.Range(-15, 15), 0);
                PoolSystem.Instance.GetPolygon().Initialize(Random.Range(8, 15), aux, -aux, Random.Range(2, 4));
            }*/
        }

        private void UpdateScore()
        {
            _textScore.text = _score.ToString("000000000");
        }

        private IEnumerator SpawnWave()
        {
            yield return new WaitForSeconds(_spawnRate);
            int amount = Mathf.RoundToInt(_waveDensity);
            for(int i = 0; i< amount; ++i)
            {
                Vector3 direction = Random.insideUnitCircle.normalized;
                Vector3 position = _spawnCenter + direction * _spawnDistance;

                Quaternion rotation = Quaternion.AngleAxis(Random.Range(-_trajectoryVariance, _trajectoryVariance), Vector3.forward);

                PoolSystem.Instance.GetPolygon().Initialize(Mathf.RoundToInt(_polygonVertexRange.RandomInRange()), position, rotation * -direction, _polygonSpeedRange.RandomInRange(), _polygonRadius.RandomInRange());
            }
            ++_cycles;
            WaveIncrease();
            StartCoroutine(SpawnWave());
        }

        private void WaveIncrease()
        {
            float vertexToLevel = _polygonVertexRange.Sum() * _waveDensity * 0.75f;
            if (_vertexDestroyed > vertexToLevel || _cycles > 2) {
                _vertexDestroyed = 0;
                _cycles = 0;
                ++_level;
                _waveDensity += _waveGrowth * _waveDensity;
                if (_waveDensity > _maxDensity)
                {
                    _spawnRate *= 0.9f;
                    if (_spawnRate < _minRate)
                    {
                        _spawnRate = _minRate;
                        _waveDensity = _maxDensity;
                    }
                    else
                    {
                        _waveDensity = _waveDensity / 10f;
                        if (_waveDensity < 1) _waveDensity = 1;
                    }
                }

                if (_polygonRadius != _polygonRadiusLimits)
                {
                    _polygonRadius.Scale(new Vector2(1 - _polygonGrowth, _polygonGrowth));
                    if (_polygonRadius.x < _polygonRadiusLimits.x)
                    {
                        _polygonRadius = new Vector2(_polygonRadiusLimits.x, _polygonRadius.y);
                    }
                    if (_polygonRadius.y > _polygonRadiusLimits.y)
                    {
                        _polygonRadius = new Vector2(_polygonRadius.x, _polygonRadiusLimits.y);
                    }

                }

                if (_polygonVertexRange != _polygonVertexLimits)
                {
                    _polygonVertexRange *= _polygonGrowth;
                    if (_polygonVertexRange.x < _polygonVertexLimits.x)
                    {
                        _polygonVertexRange = new Vector2(_polygonVertexLimits.x, _polygonVertexRange.y);
                    }
                    if (_polygonVertexRange.y > _polygonVertexLimits.y)
                    {
                        _polygonVertexRange = new Vector2(_polygonVertexRange.x, _polygonVertexLimits.y);
                    }
                }
                    
                    

            }

        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(_spawnCenter, _spawnDistance);
        }
    }
}
