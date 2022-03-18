using System;
using System.Collections;
using System.Collections.Generic;
using Nevergreen;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 
/// </summary>
public class GameManager : SingletonBehaviour<GameManager>
{
    public UnityEvent OnWorldGenerated;
    
    private void Awake()
    {
        Settings.LoadSettings();
        
        World.Initialize();
    }

    private void OnApplicationQuit()
    {
        Settings.SaveSettings();
    }

    /// <summary>
    /// Called when the start/generate button in the main menu is clicked.
    /// </summary>
    /// <param name="preGenerate">Whether or not to pre-generate the world.</param>
    public void OnStartClicked(bool preGenerate)
    {
        GenerationManager.Singleton.PrepareGeneration(preGenerate);
        
        ChunkLoadManager.Singleton.InitializeCamera();
        ChunkLoadManager.Singleton.StartLoading();
    }

    /// <summary>
    /// Called when the save settings button is clicked.
    /// </summary>
    public void OnSaveSettingsClicked()
    {
        Settings.SaveSettings();
    }
}