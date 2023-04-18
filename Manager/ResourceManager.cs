using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
    public T Load<T>(string path) where T : Object
    {
        return Resources.Load<T>(path);
    }
    
    public void Init() {}

    public GameObject Instantiate(string path, Transform parent = null)
    {
        GameObject prefab = Load<GameObject>($"Prefabs/{path}");
        if (prefab == null)
        {
            return null;
        }

        return Object.Instantiate(prefab, parent);
    }
    
    public GameObject Instantiate(string path, Vector3 pos, Quaternion rotate, Transform parent = null)
    {
        GameObject prefab = Load<GameObject>($"Prefabs/{path}");
        if (prefab == null)
        {
            return null;
        }

        return Object.Instantiate(prefab, pos, rotate, parent);
    }
    
    public GameObject InstantiateDontDistroy(string path, Transform parent = null)
    {
        GameObject prefab = Load<GameObject>($"Prefabs/{path}");
        if (prefab == null)
        {
            return null;
        }

        var obj = Object.Instantiate(prefab, parent);
        DontDestroyOnLoad(obj);
        
        return obj;
    }
    
    public GameObject InstantiateDontDistroy(string path, Vector3 pos, Quaternion rotate, Transform parent = null)
    {
        GameObject prefab = Load<GameObject>($"Prefabs/{path}");
        if (prefab == null)
        {
            return null;
        }
        
        var obj = Object.Instantiate(prefab, pos, rotate, parent);
        DontDestroyOnLoad(obj);

        return obj;
    }

    public GameObject InstantiateEmtObj(string objName)
    {
        GameObject obj = new GameObject { name = objName };
        DontDestroyOnLoad(obj);
        return obj;
    }
}
