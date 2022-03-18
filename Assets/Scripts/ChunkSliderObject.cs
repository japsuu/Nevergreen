using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 
/// </summary>
public class ChunkSliderObject : MonoBehaviour
{
    /*
    public Setting targetSetting;

    public Slider inputSlider;    
    public Text inputSliderValueDisplay;
    
    public enum Setting
    {
        ChunkSize,
        ChunkLoadAreaHorizontal,
        ChunkLoadAreaVertical
    }

    private void Start()
    {
        surfaceData = GenerationManager.Singleton.surfaceData;
        inputSlider.value = surfaceData.GetGenerationDataSetting(targetSetting);
        
        inputSliderValueDisplay.text = inputSlider.wholeNumbers ?
            inputSlider.value.ToString() :
            inputSlider.value.ToString("F3");
    }

    public void ApplyChangedValue()
    {
        float inputValue = inputSlider.value;
        surfaceData.SetGenerationDataSetting(targetSetting, inputValue);
        
        inputSliderValueDisplay.text = inputSlider.wholeNumbers ? 
            inputValue.ToString() :
            inputValue.ToString("F3");

        
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(surfaceData);
#endif
    }*/
}