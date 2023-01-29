using System;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component 
{
    // instance��� ������ static���� �����Ͽ� �ٸ� ������Ʈ ����
    // ��ũ��Ʈ������ instance�� �ҷ��� �� �ִ�.
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
