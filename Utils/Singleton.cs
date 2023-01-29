using System;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component 
{
    // instance라는 변수를 static으로 선언하여 다른 오브젝트 안의
    // 스크립트에서도 instance를 불러올 수 있다.
    private static T _instance = null;

    public static T instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject();
                obj.name = typeof(T).Name;
                obj.hideFlags = HideFlags.HideAndDontSave;
                DontDestroyOnLoad(obj);
                _instance = obj.AddComponent<T>();
            }
            return _instance;
        }
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
}
