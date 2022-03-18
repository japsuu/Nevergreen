using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 
/// </summary>
public class UIManager : SingletonBehaviour<UIManager>
{
    [Header("Root objects")]
    public GameObject MainMenuRoot;
    //public GameObject PauseMenuRoot;

    [Header("Inputs")]
    public Button StartButton;
    //public Button SettingsButton;
    //public Button CreditsButton;
    public Button QuitButton;

    public Toggle PreGenerateWorld;
    public TMP_InputField SeedInputField;

    private void Awake()
    {
        StartButton.onClick.AddListener(OnStartClicked);
        //SettingsButton.onClick.AddListener(OnSettingsClicked);
        //CreditsButton.onClick.AddListener(OnCreditsClicked);
        QuitButton.onClick.AddListener(OnQuitClicked);

        GameManager.Singleton.OnWorldGenerated.AddListener(OnWorldGenerated);
    }

    private void OnStartClicked()
    {
        if (int.TryParse(SeedInputField.text, out int result))
        {
            GenerationManager.Singleton.SetSeed(result);
        }
        else
        {
            GenerationManager.Singleton.SetSeed();
        }
        
        GameManager.Singleton.OnStartClicked(PreGenerateWorld.isOn);

        StartButton.gameObject.SetActive(false);
    }

    private void OnWorldGenerated()
    {
        MainMenuRoot.SetActive(false);
        StartButton.gameObject.SetActive(true);
    }

    /*private void OnSettingsClicked()
    {
        Debug.LogWarning("NotImplemented");
    }

    private void OnCreditsClicked()
    {
        Debug.LogWarning("NotImplemented");
    }*/

    private void OnQuitClicked()
    {
        //TODO: Save stuff?
        Application.Quit();
    }
}
