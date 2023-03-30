using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class ShipSpawner : MonoBehaviour
{

    private ObjectPool<ShipMovement> _pool;

    [Header("Simulation Dependencies")]
    [SerializeField] private ShipMovement _shipPrefab;
    
    [Header("Simulation Variables")]
    [SerializeField] private float _timeBetweenSpawn = 0.3f;
    [SerializeField] private bool _isPooling = false;
    private int _previousNumber = 0, _nextNumber = 0, _shipsToSpawn = 0, _totalShipsSpawned = 0;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _shipsToSpawnValue;
    [SerializeField] private TextMeshProUGUI _totalShipsSpawnedValue;
    [SerializeField] private TextMeshProUGUI _actualFibonacciNumberValue;

    void Start()
    {
        _pool = new(() =>
        {
            return Instantiate(_shipPrefab);
        }, shape =>
        {
            shape.gameObject.SetActive(true);
        }, shape =>
        {
            shape.gameObject.SetActive(false);
        }, shape =>
        {
            Destroy(shape.gameObject);
        }, false, 50, 100);

        _shipsToSpawn = _previousNumber + _nextNumber;
        UpdateUI();
        StartCoroutine(SpawnShips());
    }

    private void UpdateUI()
    {
        if (_shipsToSpawnValue) _shipsToSpawnValue.text = _shipsToSpawn.ToString();
        if (_totalShipsSpawnedValue) _totalShipsSpawnedValue.text = _totalShipsSpawned.ToString();
        if (_actualFibonacciNumberValue) _actualFibonacciNumberValue.text = (_previousNumber + _nextNumber).ToString();
    }

    private IEnumerator SpawnShips()
    {
        yield return new WaitForSeconds(_timeBetweenSpawn);
        if(_shipsToSpawn > 0)
        {
            print("Spawning");
            InstantiateShipPrefab();
            UpdateUI();
            StartCoroutine(SpawnShips());
        }
        else
        {
            if (_previousNumber == 0 && _nextNumber == 0)
            {
                print("Primeira nave a ser spawnada");
                _shipsToSpawn = 1;
                UpdateUI();
                InstantiateShipPrefab();
                UpdateUI();
                _shipsToSpawn = 1;
                _nextNumber = 1;
                StartCoroutine(SpawnShips());
            }
            else
            {
                print("Recalculando");
                var temp = _previousNumber + _nextNumber;
                _previousNumber = _nextNumber;
                _nextNumber = temp;

                _shipsToSpawn = _previousNumber + _nextNumber;

                UpdateUI();
                StartCoroutine(SpawnShips());
            }

        }
    }

    private void InstantiateShipPrefab()
    {
        Quaternion rotation = Quaternion.Euler(0, 0, Random.Range(-180, 180));
        var ship = _isPooling ? _pool.Get() : Instantiate(_shipPrefab);

        ship.transform.position = transform.GetChild(0).position;
        ship.transform.rotation = rotation;

        ship.Init(DestroyAction);
        _shipsToSpawn--;
        _totalShipsSpawned++;
    }

    private void DestroyAction(ShipMovement ship)
    {
        if (_isPooling)
        {
            _pool.Release(ship);
        }
        else
        {
            Destroy(ship.gameObject);
        }
    }
}
