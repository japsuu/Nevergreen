using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Very simple movement script for the camera.
/// May want to decouple the input.
/// </summary>
public class SimpleCameraMovement : MonoBehaviour
{
    public float speed;
    public float zoomIncrement;
    public float cameraZoomMin, cameraZoomMax;


    private void Update ()
    {
        // Simple movement
        transform.position += new Vector3(
            Input.GetAxis("Horizontal") * Time.deltaTime * speed,
            Input.GetAxis("Vertical") * Time.deltaTime * speed);

        // Detect zoom
        float scrollwheelInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollwheelInput != 0f && !Input.GetKey(KeyCode.LeftShift))
            Zoom(scrollwheelInput < 0f ? zoomIncrement : -zoomIncrement);
    }


    /// <summary>
    /// Zooms the camera with the given relative change to apply.
    /// Automatically clamps between the minimal and maximal zoom.
    /// </summary>
    /// <param name="relativeChange"></param>
    public void Zoom(float relativeChange)
    {
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize + relativeChange, 
            cameraZoomMin, cameraZoomMax);
    }
}