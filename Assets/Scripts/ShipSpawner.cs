using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class ShipSpawner : MonoBehaviour
{
    private ObjectPool<ShipMovement> _pool;

    [Header("Simulation Dependencies")]
    [SerializeField] private ShipMovement _shipPrefab;
    [SerializeField] private SaveSO _saveFile;
    [SerializeField] private SimulationUI _simulationUI;
    
    [Header("Simulation Variables")]
    [SerializeField] private float _timeBetweenSpawn = 0.3f;
    [SerializeField] private bool _isPooling = false;
    private int _previousNumber = 0, _nextNumber = 0, _shipsToSpawn = 0, _totalShipsSpawned = 0;
    

    void Start()
    {
        _timeBetweenSpawn = _saveFile.TimeBetweenSpawn;
        _isPooling = _saveFile.IsPooling;

        _simulationUI.StartUI(_saveFile);

        _pool = new(() =>
        {
            _totalShipsSpawned++;
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
        StartCoroutine(SpawnShips());
    }

    public void TogglePool()
    {
        _saveFile.IsPooling = !_isPooling;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SetTimeBetweenSpawn(float value)
    {
        _timeBetweenSpawn = value;
        _saveFile.TimeBetweenSpawn = value;
    }

    public void SetFPSUpdateTime(float value)
    {
        _saveFile.FPSUpdateInterval = value;
    }

    private void Update()
    {
        _simulationUI.UpdateUI(
            _pool.CountActive,
            _pool.CountInactive,
            _totalShipsSpawned,
            _shipsToSpawn,
            _previousNumber + _nextNumber
        );
    }
    private IEnumerator SpawnShips()
    {
        yield return new WaitForSeconds(_timeBetweenSpawn);
        if(_shipsToSpawn > 0)
        {
            print("Spawning");
            InstantiateShipPrefab();
            StartCoroutine(SpawnShips());
        }
        else
        {
            if (_previousNumber == 0 && _nextNumber == 0)
            {
                print("Primeira nave a ser spawnada");
                _shipsToSpawn = 1;
                InstantiateShipPrefab();
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
        if(!_isPooling) _totalShipsSpawned++;
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
