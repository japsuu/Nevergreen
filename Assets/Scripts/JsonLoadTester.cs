using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Nevergreen
{
    public class JsonLoadTester : MonoBehaviour
    {
        private void Start()
        {
            LoadHelper.LoadOfficialBlockData();
                
            Debug.LogError("Task failed successfully!");
        }
    }
}