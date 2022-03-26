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

        [Header("Spawner config")]
        [SerializeField]
        private float _spawnRate = 2f;
        [SerializeField]
        private float _spawnDistance = 22;
        [SerializeField]
        private Vector3 _spawnCenter = Vector3.zero;
        [SerializeField]
        private Vector2 _polygonVertexRange = new Vector2(3, 5);
        [SerializeField]
        private float _polygonGrowth;
        [SerializeField]
        private Vector2 _poligonRadius = new Vector2(2, 4);
        [SerializeField]
        private Vector2 _polygonRadiusLimits;
        [SerializeField]
        private Vector2 _polygonSpeedRange = new Vector2(2, 4);
        [SerializeField]
        private float _waveDensity;
        [SerializeField]
        private float _waveGrowth;
        [SerializeField]
        private float _trajectoryVariance = 15;


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

                PoolSystem.Instance.GetPolygon().Initialize(Mathf.RoundToInt(_polygonVertexRange.RandomInRange()), position, rotation * -direction, _polygonSpeedRange.RandomInRange(), _poligonRadius.RandomInRange());
            }

            StartCoroutine(SpawnWave());
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(_spawnCenter, _spawnDistance);
        }
    }
}
