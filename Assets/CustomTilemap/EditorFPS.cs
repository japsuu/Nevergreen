using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class EditorFPS : MonoBehaviour
{
#if UNITY_EDITOR

    public Rect TextPosition = new Rect(10, 10, 80, 30);
    public float refresh = 0.2f;
    
    private float timer;
    private float avgFramerate;

    private void OnGUI()
    {
        float timelapse = Time.smoothDeltaTime;
        if (timer <= 0)
            timer = refresh;
        else
            timer = timer -= timelapse;

        if(timer <= 0)
            avgFramerate = (int)(1f / timelapse);

        string text = $"{avgFramerate} FPS";
        
        GUI.Label(TextPosition, text);
    }

#endif
}
