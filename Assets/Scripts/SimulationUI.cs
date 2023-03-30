using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SimulationUI : MonoBehaviour
{
    private SaveSO _saveSO;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _shipsToSpawnValue;
    [SerializeField] private TextMeshProUGUI _totalActiveShips;
    [SerializeField] private TextMeshProUGUI _totalInactiveShips;
    [SerializeField] private TextMeshProUGUI _totalShipsSpawnedValue;
    [SerializeField] private TextMeshProUGUI _actualFibonacciNumberValue;
    [SerializeField] private Slider _spawnRate;
    [SerializeField] private Slider _FPSSlider;
    [SerializeField] private Toggle _poolToggle;

    private float accum = 0.0f, timeleft, fps;
    private int frames = 0;

    GUIStyle textStyle = new GUIStyle();

    void OnGUI()
    {
        GUI.Label(new Rect(5, 5, 100, 25), fps.ToString("F2") + "FPS", textStyle);
    }

    public void StartUI(SaveSO _saveSO)
    {
        this._saveSO = _saveSO;

        _poolToggle.SetIsOnWithoutNotify(_saveSO.IsPooling);
        _FPSSlider.value = _saveSO.FPSUpdateInterval;
        _spawnRate.value = _saveSO.TimeBetweenSpawn;

        timeleft = _saveSO.FPSUpdateInterval;

        textStyle.fontStyle = FontStyle.Bold;
        textStyle.normal.textColor = Color.white;
    }

    private void Update()
    {
        timeleft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        ++frames;

        if (timeleft <= 0.0)
        {
            fps = (accum / frames);
            timeleft = _saveSO.FPSUpdateInterval;
            accum = 0.0f;
            frames = 0;
        }
    }

    public void UpdateUI(int activeShips, int inactiveShips, int totalShips, int shipsToSpawn, int actualFibbonaciNumber)
    {
        if (_totalActiveShips && _saveSO.IsPooling) _totalActiveShips.text = activeShips.ToString();
        if (_totalInactiveShips && _saveSO.IsPooling) _totalInactiveShips.text = inactiveShips.ToString();
        if (_shipsToSpawnValue) _shipsToSpawnValue.text = shipsToSpawn.ToString();
        if (_totalShipsSpawnedValue) _totalShipsSpawnedValue.text = totalShips.ToString();
        if (_actualFibonacciNumberValue) _actualFibonacciNumberValue.text = actualFibbonaciNumber.ToString();
    }
}
