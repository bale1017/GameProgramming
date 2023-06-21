using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Events;

public class Game : MonoBehaviour
{
    enum GameState
    {
        PENDING,
        PAUSED,
        RUNNING,
        FAILURE,
        VICTORY
    }

    public static Game current;
    public static bool IsRewinding = false;

    public UnityEvent OnGameLoad = new();
    public UnityEvent OnGameStart = new();
    public UnityEvent OnGamePause = new();
    public UnityEvent OnGameUnpause = new();
    public UnityEvent OnGameCompletion = new();
    public UnityEvent OnGameVictory = new();
    public UnityEvent OnGameFailure = new();
    public UnityEvent OnRewindStart = new();
    public UnityEvent OnRewindEnd = new();

    public KeyCode RewindKey = KeyCode.R;

    private GameState gameState = GameState.PENDING;
    // set to the previous game state when pausing and reset to this value after unpausing
    private GameState pausedGameState = GameState.PAUSED;
    // set to the previous game state when pausing and reset to this value after unpausing
    private float pausedTimeScale = 1;

    double timer = 60;

    // Start is called before the first frame update
    void Start()
    {
        current = this;
        LoadGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(RewindKey))
        {
            StartRewind();
        }
        if (Input.GetKeyUp(RewindKey))
        {
            StopRewind();
        }

        // start and stop if Rewind Key is pressed and game is not paused
        if (IsRunning())
        {
            if (Input.GetKeyDown(RewindKey))
            {
                StartRewind();
            }
            else if (Input.GetKeyUp(RewindKey))
            {
                StopRewind();
            }
        }

        // make as listener
        if (IsRewinding)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer -= Time.deltaTime;
        }
    }

    public void LoadGame()
    {
        gameState = GameState.PENDING;
        OnGameLoad.Invoke();
        // TODO 1s delay toleranz
        StartGame();
    }

    public void StartGame()
    {
        gameState = GameState.RUNNING;
        Time.timeScale = 1;
        OnGameStart.Invoke();
    }

    public void WinGame()
    {
        gameState = GameState.VICTORY;
        OnGameVictory.Invoke();
    }

    public void FailGame()
    {
        gameState = GameState.FAILURE;
        OnGameFailure.Invoke();
    }

    public void PauseGame()
    {
        pausedGameState = gameState;
        pausedTimeScale = Time.timeScale;
        Time.timeScale = 0;
        OnGamePause.Invoke();
    }

    public void ResumeGame()
    {
        gameState = pausedGameState;
        pausedGameState = GameState.PAUSED;
        Time.timeScale = pausedTimeScale;
        OnGameUnpause.Invoke();
    }

    public void StartRewind()
    {
        if (!IsRunning()) return;
        if (IsRewinding) return;
        IsRewinding = true;
        Debug.Log("start rewind");
        // Invoke start event so that all listeners know that time is now rewinding.
        OnRewindStart.Invoke();
    }

    public void StopRewind()
    {
        if (!IsRunning()) return;
        if (!IsRewinding) return;
        IsRewinding = false;
        // Invoke stop event so that all listeners know that time is no longer rewinding.
        OnRewindEnd.Invoke();
    }

    public double GetTime()
    {
        return timer;
    }

    public bool IsRunning()
    {
        return !IsPaused() && gameState == GameState.RUNNING;
    }

    public bool IsPaused()
    {
        return pausedGameState != GameState.PAUSED;
    }
}