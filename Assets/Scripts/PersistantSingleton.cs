using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistantSingleton<T> : MonoBehaviour where T : Component
{
    private static T singleton;
    public static T Singleton
    {
        get
        {
            if (singleton != null) return singleton;
            
            singleton = FindObjectOfType<T>();
            
            if (singleton != null) return singleton;
            
            GameObject obj = new GameObject
            {
                name = typeof(T).Name
            };
            singleton = obj.AddComponent<T>();
            
            Debug.LogWarning("Could not find the requested " + singleton.GetType() + " in the scene. Creating...");
            
            return singleton;
        }
    }
 
    protected virtual void Awake ()
    {
        if (singleton == null)
        {
            singleton = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
