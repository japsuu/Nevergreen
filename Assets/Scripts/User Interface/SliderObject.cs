using System;
using System.Collections;
using System.Collections.Generic;
using Nevergreen;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Describes a slider object in the UI.
/// </summary>
public class SliderObject : MonoBehaviour, 
    IPointerEnterHandler, IPointerExitHandler
{
    public Slider inputSlider;    
    public Text inputSliderValueDisplay;
    public BlockGenerationData.Setting targetSetting;
    public BlockData targetBlockData;

    private const string DecimalFormatting = "F3";


    private IEnumerator Start()
    {
        yield return null;
        InitializeField();
    }


    /// <summary>
    /// Initializes by grabbing data from the assigned data object.
    /// </summary>
    public void InitializeField()
    {
        inputSlider.value = targetBlockData.GetGenerationDataSetting(targetSetting);
        
        inputSliderValueDisplay.text = inputSlider.wholeNumbers ?
            inputSlider.value.ToString() :
            inputSlider.value.ToString(DecimalFormatting);
    }


    /// <summary>
    /// Updates this slider and its related data object with a new slider value.
    /// Called automatically by the related slider's OnValueChanged event.
    /// If this slider activated the Perlin preview map, it updates the map as well.
    /// </summary>
    public void EditField()
    {
        float inputValue = inputSlider.value;
        targetBlockData.SetGenerationDataSetting(targetSetting, inputValue);
        
        inputSliderValueDisplay.text = !inputSlider.wholeNumbers ? 
            inputValue.ToString(DecimalFormatting) : inputValue.ToString();

        if (UserInterface.Singleton.perlinPreviewImage.enabled)
            UpdatePerlinPreviewMap();

        
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(DataLoader.Singleton.Database);
#endif
    }


    /// <summary>
    /// Updates the perlin map previewer with the right data.
    /// </summary>
    /// <returns></returns>
    private bool UpdatePerlinPreviewMap()
    {      
        // Multi-layer view
        if (UserInterface.Singleton.perlinPreviewAllToggle.isOn)
        {
            switch (targetSetting)
            {
                case BlockGenerationData.Setting.PerlinSpeed:
                case BlockGenerationData.Setting.PerlinLevel:
                case BlockGenerationData.Setting.ZonePerlinSpeed:
                case BlockGenerationData.Setting.ZonePerlinLevel:
                case BlockGenerationData.Setting.MapPerlinSpeed:
                case BlockGenerationData.Setting.MapPerlinLevel:
                    UserInterface.Singleton.CalculatePerlinPreviewMap(
                        targetBlockData.GenerationData.PerlinSpeed,
                        targetBlockData.GenerationData.PerlinLevel,
                        targetBlockData.GenerationData.ZonePerlinSpeed,
                        targetBlockData.GenerationData.ZonePerlinLevel,
                        targetBlockData.GenerationData.MapPerlinSpeed,
                        targetBlockData.GenerationData.MapPerlinLevel);
                    return true;
                
                default:
                    return false;
            }
        }
        // Per-layer view
        else
        {
            switch (targetSetting)
            {
                case BlockGenerationData.Setting.PerlinSpeed:
                case BlockGenerationData.Setting.PerlinLevel:
                    UserInterface.Singleton.CalculatePerlinPreviewMap(
                        targetBlockData.GenerationData.PerlinSpeed,
                        targetBlockData.GenerationData.PerlinLevel);
                    return true;
                
                case BlockGenerationData.Setting.ZonePerlinSpeed:
                case BlockGenerationData.Setting.ZonePerlinLevel:
                    UserInterface.Singleton.CalculatePerlinPreviewMap(
                        targetBlockData.GenerationData.ZonePerlinSpeed,
                        targetBlockData.GenerationData.ZonePerlinLevel);
                    return true;
                
                case BlockGenerationData.Setting.MapPerlinSpeed:
                case BlockGenerationData.Setting.MapPerlinLevel:
                    UserInterface.Singleton.CalculatePerlinPreviewMap(
                        targetBlockData.GenerationData.MapPerlinSpeed,
                        targetBlockData.GenerationData.MapPerlinLevel);
                    return true;
                
                default:
                    return false;
            }
        }
    }


    /// <summary>
    /// Shows the Perlin preview map on mouse over if updating it was a success.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (UpdatePerlinPreviewMap())
            UserInterface.Singleton.perlinPreviewImage.enabled = true;
    }


    /// <summary>
    /// Hides the Perlin preview map when the mouse leaves this object.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        UserInterface.Singleton.perlinPreviewImage.enabled = false;
    }
}
 