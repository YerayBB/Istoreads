using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


namespace Istoreads
{
    public class GameManager : MonoBehaviour
    {
        private TMP_Text _textScore;
        private int _score = 0;


        private void Awake()
        {
            _textScore = transform.GetChild(0).GetComponent<TMP_Text>();
        }

        // Start is called before the first frame update
        void Start()
        {
            UpdateScore();
            var aux = new Vector3(Random.Range(-15, 15),Random.Range(-15, 15), 0);
            PoolSystem.Instance.GetPolygon().Initialize(Random.Range(8, 15), aux, -aux, Random.Range(2, 4));
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
        }

        // Update is called once per frame
        void Update()
        {
            //Debug.Log(Time.time);
            if (Mathf.Round(Time.time % 20) == 0)
            {
                Debug.Log("SPAWN");
                var aux = new Vector3(Random.Range(-15, 15), Random.Range(-15, 15), 0);
                PoolSystem.Instance.GetPolygon().Initialize(Random.Range(8, 15), aux, -aux, Random.Range(2, 4));
            }
        }

        private void UpdateScore()
        {
            _textScore.text = _score.ToString("000000000");
        }
    }
}
