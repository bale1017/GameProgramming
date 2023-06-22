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

    public Game()
    {
        current = this;
    }

    // Start is called before the first frame update
    void Start()
    {
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
    }

    public void LoadGame()
    {
        gameState = GameState.PENDING;
        OnGameLoad.Invoke();

        StartGame();
    }

    IEnumerator then(int sec, Action f)
    {
        // Give some time so taht player can realize, then game over screen.
        yield return new WaitForSeconds(sec);
        f.Invoke();
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
        OnGameCompletion.Invoke();
    }

    public void FailGame()
    {
        gameState = GameState.FAILURE;
        OnGameFailure.Invoke();
        OnGameCompletion.Invoke();
    }

    public void PauseGame()
    {
        if (gameState == GameState.VICTORY || gameState == GameState.FAILURE)
        {
            return;
        }
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
        // Invoke start event so that all listeners know that time is now rewinding.
        OnRewindStart.Invoke();
    }

    public void StopRewind()
    {
        if (!IsRunning()) return;
        IsRewinding = false;
        // Invoke stop event so that all listeners know that time is no longer rewinding.
        OnRewindEnd.Invoke();
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