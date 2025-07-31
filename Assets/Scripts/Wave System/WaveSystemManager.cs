using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WaveSystem.Utilities;

public class WaveSystemManager : MonoBehaviour
{
    [Header("Wave Configuration")]
    [SerializeField] private int _waveCount = 0;
    [SerializeField] private float _timeBetweenWaves = 5f;
    [SerializeField] private float _timeBetweenSpawns = 0.1f;
    [SerializeField] private int _incrementedEnemiesPerWave = 10;
    [SerializeField] private int _baseEnemiesPerWave = 30;
    
    [Header("Spawn Configuration")]
    [SerializeField] private GameObject[] _wavePrefabs;
    [SerializeField] private Transform[] _spawnPoints;

    // Enemy tracking
    private HashSet<GameObject> _activeEnemies = new HashSet<GameObject>();
    private int _enemiesSpawnedThisWave = 0;
    
    // System state
    private Coroutine _currentCoroutine;
    private bool _isSpawning = false;
    private bool _systemStarted = false;

    // Public Properties
    public int WaveCount => _waveCount;
    public bool IsSpawning => _isSpawning;
    public int CurrentlyAliveEnemies => _activeEnemies.Count;
    public bool SystemStarted => _systemStarted;
    
    // Calculated Properties
    private int MaxEnemiesThisWave => _baseEnemiesPerWave + (_waveCount * _incrementedEnemiesPerWave);

    // Events
    public static event Action<GameObject> OnEnemySpawned;
    public static event Action<int> OnWaveStarted;
    public static event Action<int> OnWaveCompleted;
    public static event Action OnSystemStarted;

    #region Public Interface

    public void StartWaveSystem()
    {
        if (_systemStarted) return;
        
        _systemStarted = true;
        OnSystemStarted?.Invoke();
        StartNextWave();
    }

    public void StopWaveSystem()
    {
        StopCurrentCoroutine();
        _systemStarted = _isSpawning = false;
        ClearAllEnemies();
    }

    public void ForceNextWave()
    {
        if (!_systemStarted) return;
        
        StopCurrentCoroutine();
        _currentCoroutine = StartCoroutine(ForceNextWaveCoroutine());
    }

    public void DestroyCurrentEnemies()
    {
        if (!_systemStarted) return;
        
        ClearAllEnemies();
        
        // If we were spawning, stop spawning and let the system wait for wave completion
        if (_isSpawning)
        {
            _isSpawning = false;
            StopCurrentCoroutine();
            
            // Start the between-waves countdown since no enemies are alive
            _currentCoroutine = StartCoroutine(WaitBetweenWavesCoroutine());
        }
    }

    public void ResetWaveCount()
    {
        StopCurrentCoroutine();
        _waveCount = 0;
        _systemStarted = _isSpawning = false;
        _activeEnemies.Clear();
    }

    public bool ToggleSpawning()
    {
        _isSpawning = !_isSpawning;
        
        if (_isSpawning && _systemStarted && _currentCoroutine == null)
        {
            _currentCoroutine = StartCoroutine(SpawnWaveCoroutine());
        }
        else if (!_isSpawning)
        {
            StopCurrentCoroutine();
        }
        
        return _isSpawning;
    }

    public void OnEnemyDestroyed(GameObject enemy) => _activeEnemies.Remove(enemy);

    #endregion

    #region Wave Management

    private void StartNextWave()
    {
        _waveCount++;
        _enemiesSpawnedThisWave = 0;
        _isSpawning = true;
        
        OnWaveStarted?.Invoke(_waveCount);
        _currentCoroutine = StartCoroutine(SpawnWaveCoroutine());
    }

    private IEnumerator SpawnWaveCoroutine()
    {
        // Spawn all enemies for this wave
        while (_enemiesSpawnedThisWave < MaxEnemiesThisWave)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(_timeBetweenSpawns);
        }
        
        // Wait for all enemies to be defeated
        _isSpawning = false;
        yield return new WaitUntil(() => _activeEnemies.Count == 0);
        
        // Wave completed - trigger event and wait before next wave
        OnWaveCompleted?.Invoke(_waveCount);
        yield return new WaitForSeconds(_timeBetweenWaves);
        
        // Start next wave
        StartNextWave();
    }

    #endregion

    #region Enemy Spawning

    private void SpawnEnemy()
    {
        if (_wavePrefabs.Length == 0 || _spawnPoints.Length == 0) return;

        // Select random prefab and spawn point
        var enemyPrefab = _wavePrefabs[UnityEngine.Random.Range(0, _wavePrefabs.Length)];
        var spawnPoint = _spawnPoints[UnityEngine.Random.Range(0, _spawnPoints.Length)];
        
        // Spawn using object pool
        var enemy = ObjectPool.Instance.Instantiate(
            enemyPrefab, 
            spawnPoint.position, 
            spawnPoint.rotation
        );

        // Track the spawned enemy
        _activeEnemies.Add(enemy);
        _enemiesSpawnedThisWave++;
        
        OnEnemySpawned?.Invoke(enemy);
    }

    #endregion

    #region Utility Methods

    private void StopCurrentCoroutine()
    {
        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
            _currentCoroutine = null;
        }
    }

    private void SpawnRemainingEnemies()
    {
        while (_enemiesSpawnedThisWave < MaxEnemiesThisWave)
        {
            SpawnEnemy();
        }
    }

    /// <summary>
    /// Coroutine that spawns remaining enemies and immediately starts next wave
    /// </summary>
    private IEnumerator ForceNextWaveCoroutine()
    {
        // Spawn all remaining enemies for current wave instantly
        SpawnRemainingEnemies();
        
        // Wait one frame to ensure all enemies are spawned
        yield return null;
        
        // Start next wave immediately
        StartNextWave();
    }

    private IEnumerator WaitBetweenWavesCoroutine()
    {
        OnWaveCompleted?.Invoke(_waveCount);
        yield return new WaitForSeconds(_timeBetweenWaves);
        StartNextWave();
    }

    private void ClearAllEnemies()
    {
        foreach (var enemy in _activeEnemies)
        {
            if (enemy != null) 
                ObjectPool.Instance.ReturnToPool(enemy);
        }
        _activeEnemies.Clear();
    }

    #endregion
}
