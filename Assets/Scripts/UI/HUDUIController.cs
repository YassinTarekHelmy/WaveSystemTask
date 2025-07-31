using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDUIController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button _stopResumeButton;
    [SerializeField] private Button _nextWaveButton;
    [SerializeField] private Button _destroyCurrentButton;

    [SerializeField] private TMP_Text _waveCountText;
    [SerializeField] private TMP_Text _enemyCountText;
    [SerializeField] private TMP_Text _stopResumeButtonText;
    

    private WaveSystemManager _waveSystemManager;

    void Start()
    {
        _waveSystemManager = FindFirstObjectByType<WaveSystemManager>();
        
        if (_waveSystemManager == null)
        {
            Debug.LogError("WaveSystemManager not found in scene!");
        }
        
        UpdateUI();
    }

    void OnEnable()
    {
        _stopResumeButton.onClick.AddListener(OnStopResumeButtonClicked);
        _nextWaveButton.onClick.AddListener(OnNextWaveButtonClicked);
        _destroyCurrentButton.onClick.AddListener(OnDestroyCurrentButtonClicked);
        
        // Subscribe to wave system events
        WaveSystemManager.OnSystemStarted += UpdateUI;
        WaveSystemManager.OnWaveStarted += OnWaveStarted;
        WaveSystemManager.OnWaveCompleted += OnWaveCompleted;
    }

    void Update()
    {

        //TODO: Remove this and make it event-driven
        // Update UI every frame for real-time enemy count
        UpdateUI();
    }

    private void OnWaveStarted(int waveNumber)
    {
        UpdateUI();
    }

    private void OnWaveCompleted(int waveNumber)
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (_waveSystemManager == null) return;

        // Update wave count text
        if (_waveCountText != null)
        {
            _waveCountText.text = $"Wave {_waveSystemManager.WaveCount}";
        }

        // Update enemy count text
        if (_enemyCountText != null)
        {
            _enemyCountText.text = $"Enemies: {_waveSystemManager.CurrentlyAliveEnemies}";
        }

        // Update stop/resume button text based on system state
        if (_stopResumeButtonText != null)
        {
            if (!_waveSystemManager.SystemStarted)
            {
                _stopResumeButtonText.text = "Start";
            }
            else if (_waveSystemManager.IsSpawning)
            {
                _stopResumeButtonText.text = "Pause";
            }
            else
            {
                _stopResumeButtonText.text = "Resume";
            }
        }
    }

    private void OnDestroyCurrentButtonClicked()
    {
        if (_waveSystemManager != null)
        {
            _waveSystemManager.DestroyCurrentEnemies();
            UpdateUI();
        }
    }

    private void OnNextWaveButtonClicked()
    {
        if (_waveSystemManager != null)
        {
            _waveSystemManager.ForceNextWave();
            UpdateUI();
        }
    }

    private void OnStopResumeButtonClicked()
    {
        if (_waveSystemManager != null)
        {
            if (_waveSystemManager.SystemStarted)
            {
                _waveSystemManager.ToggleSpawning();
            }
            else
            {
                _waveSystemManager.StartWaveSystem();
            }
            UpdateUI(); // Update UI immediately after state change
        }
    }

    private void OnDisable()
    {
        _stopResumeButton.onClick.RemoveListener(OnStopResumeButtonClicked);
        _nextWaveButton.onClick.RemoveListener(OnNextWaveButtonClicked);
        _destroyCurrentButton.onClick.RemoveListener(OnDestroyCurrentButtonClicked);
        
        // Unsubscribe from wave system events
        WaveSystemManager.OnSystemStarted -= UpdateUI;
        WaveSystemManager.OnWaveStarted -= OnWaveStarted;
        WaveSystemManager.OnWaveCompleted -= OnWaveCompleted;
    }
}
