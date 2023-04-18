using System;
using UnityEngine;

public class Util
{
    private static float _firstTime = 0;
    public static bool OneClick()
    {
        long currentTime = DateTime.Now.Ticks;
        if (currentTime - _firstTime < 6000000)
        {
            _firstTime = currentTime;
            return false;
        }
        else
        {
            _firstTime = currentTime;
            return true;
        }
    }
    
    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        if (go == null)
            return null;

        if (recursive == false)
        {
            Transform transform = go.transform.Find(name);
            if (transform != null)
                return transform.GetComponent<T>();
        }
        else
        {
            foreach (T component in go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || component.name == name)
                    return component;
            }
        }

        return null;
    }

    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        Transform transform = FindChild<Transform>(go, name, recursive);
        if (transform != null)
            return transform.gameObject;
        return null;
    }
    
    public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();
        return component;
    }

    public static void SetParentActive(GameObject go, bool active)
    {
        if (go == null) return;
        var parent = go.transform.parent;
        if (parent == null) return;
        parent.gameObject.SetActive(active);
    } 
}
