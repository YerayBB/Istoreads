using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UtilsUnknown.Extensions;
using UtilsUnknown;


namespace Istoreads
{
    [RequireComponent(typeof(Animator))]
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Pool config")]
        [SerializeField]
        private GameObject _polygonPrefab;
        [SerializeField]
        private uint _polygonPoolInitCapacity;
        [SerializeField]
        private float _polygonTimeout = 30;
        [SerializeField]
        private GameObject _vertexPrefab;
        [SerializeField]
        private uint _vertexPoolInitCapacity;

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

        [SerializeField]
        private ParticleSystem _particleSystem;
        private Transform _particleTransform;

        private Animator _animatorUI;
        private Controls _inputs;
        private TMP_Text _textScore;
        private TMP_Text _textLevel;

        private int _score = 0;
        private int _level = 0;
        private int _cycles = 0;
        private int _vertexDestroyed = 0;

        private PoolMono<Polygon> _polygonPool;
        private PoolMono<Vertex> _vertexPool;


        #region MonoBehaviourCalls

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
            _animatorUI = GetComponent<Animator>();
            _textScore = transform.GetChild(1).GetComponent<TMP_Text>();
            _textLevel = transform.GetChild(2).GetComponent<TMP_Text>();
            if (_particleSystem != null) _particleTransform = _particleSystem.transform;

            //input handling
            _inputs = new Controls();
            _inputs.Menu.Retry.performed += (a) => Retry();
            _inputs.Menu.Quit.performed += (a) => Quit();

            //Pools init
            _polygonPool = new PoolMono<Polygon>(_polygonPrefab);
            _vertexPool = new PoolMono<Vertex>(_vertexPrefab);

            _polygonPool.Init(_polygonPoolInitCapacity);
            _vertexPool.Init(_vertexPoolInitCapacity);
        }

        void Start()
        {
            UpdateScore();
            UpdateLevel();

            Polygon.ResetEvents();
            Polygon.OnDestroyed += () =>
            {
                _score += 100;
                UpdateScore();
            };

            Vertex.ResetEvents();
            Vertex.OnDestroyed += () =>
            {
                _score += 1;
                ++_vertexDestroyed; 
                UpdateScore();
            };

            if(_particleSystem != null)
            {
                Vertex.OnDestroyedAt += (pos) =>
                {
                    _particleSystem.Stop();
                    _particleTransform.position = pos;
                    _particleSystem.Play();
                };
            }

            StartCoroutine(SpawnWave());
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(_spawnCenter, _spawnDistance);
        }

        #endregion


        public Vertex GetVertex()
        {
            return _vertexPool.GetItem();
        }

        public Polygon GetPolygon()
        {
            return _polygonPool.GetItem();
        }

        public void GameOverUI()
        {
            _inputs.Menu.Enable();
            StopAllCoroutines();
            _animatorUI.SetTrigger("GameOver");
        }

        public void Retry()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void Quit()
        {
            SceneManager.LoadScene("Tittle");
        }


        private void UpdateScore()
        {
            _textScore.text = _score.ToString("000000000");
        }

        private void UpdateLevel()
        {
            _textLevel.text = _level.ToString("00");
        }

        private IEnumerator SpawnWave()
        {
            yield return new WaitForSeconds(_spawnRate);
            int amount = Mathf.RoundToInt(_waveDensity);
            for(int i = 0; i< amount; ++i)
            {
                Vector3 direction = Random.insideUnitCircle.normalized;
                Vector3 position = _spawnCenter + direction * _spawnDistance;

                //add variation to its trajectory
                Quaternion rotation = Quaternion.AngleAxis(Random.Range(-_trajectoryVariance, _trajectoryVariance), Vector3.forward);

                _polygonPool.GetItem().Initialize(Mathf.RoundToInt(_polygonVertexRange.RandomInRange()), position, rotation * -direction, _polygonSpeedRange.RandomInRange(), _polygonRadius.RandomInRange(), _polygonTimeout);
            }
            ++_cycles;
            WaveIncrease();
            StartCoroutine(SpawnWave());
        }

        private void WaveIncrease()
        {
            float vertexToLevel = _polygonVertexRange.Sum() * _waveDensity * 0.75f;
            //level up if requirements completed
            if (_vertexDestroyed > vertexToLevel || _cycles > 2) {
                _vertexDestroyed = 0;
                _cycles = 0;
                ++_level;
                _waveDensity += _waveGrowth * _waveDensity;

                //if density surpased, spawn faster but reduce density
                if (_waveDensity > _maxDensity)
                {
                    _spawnRate *= 0.9f;
                    //if cant spawn faster, leave density as it is
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

                //increase the range of radius for the polygons according to polygongrowth, inside some limits
                Vector2 growthScale = new Vector2(1 - _polygonGrowth, 1 + _polygonGrowth);
                if (_polygonRadius != _polygonRadiusLimits)
                {
                    _polygonRadius.Scale(growthScale);
                    if (_polygonRadius.x < _polygonRadiusLimits.x)
                    {
                        _polygonRadius = new Vector2(_polygonRadiusLimits.x, _polygonRadius.y);
                    }
                    if (_polygonRadius.y > _polygonRadiusLimits.y)
                    {
                        _polygonRadius = new Vector2(_polygonRadius.x, _polygonRadiusLimits.y);
                    }
                }

                //increase the range of vertex the polygons might have according to polygongrowth, inside some limits
                if (_polygonVertexRange != _polygonVertexLimits)
                {
                    _polygonVertexRange.Scale(growthScale);
                    if (_polygonVertexRange.x < _polygonVertexLimits.x)
                    {
                        _polygonVertexRange = new Vector2(_polygonVertexLimits.x, _polygonVertexRange.y);
                    }
                    if (_polygonVertexRange.y > _polygonVertexLimits.y)
                    {
                        _polygonVertexRange = new Vector2(_polygonVertexRange.x, _polygonVertexLimits.y);
                    }
                }

                UpdateLevel();
            }

        }
    }
}
