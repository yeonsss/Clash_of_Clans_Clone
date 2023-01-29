using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Define;
public class C_SceneManager : Singleton<C_SceneManager>
{
    private string m_currentScene;
    private Slider LoadingGuage;
    
    private List<string> SceneList = new List<string>();

    public void Init()
    {
        GetSceneList();
    }

    private void GetSceneList()
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            SceneList.Add(System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i)));
        }
    }
    
    public void SwitchScene(string sceneName)
    {
        bool searchName = SceneList.Contains(sceneName);
        if (searchName == false) return;
        m_currentScene = sceneName;
        
        GameObject originUI = GameObject.FindGameObjectWithTag("UI");
        if (originUI != null)
        {
            originUI.SetActive(false);
        }

        GameObject find = GameObject.Find("LoadingUI");
        if (find == null)
        {
            GameObject loading = Resources.Load("Prefabs/UI/LoadingUI") as GameObject;
            find = Instantiate(loading);
            find.name = "LoadingUI";
        }
        
        LoadingGuage = find.transform.Find("LoadingGuage").GetComponent<Slider>();
        StartCoroutine(LoadSceneProcrss());
    }

    IEnumerator LoadSceneProcrss()
    {
        yield return null;
        AsyncOperation op = SceneManager.LoadSceneAsync(m_currentScene);
        // ���� �� �ҷ������� �� �ڵ����� �̵��� �� ����
        // false�� 90������ �ϰ�, true�� �ٲ��� �� ������ 10�ۼ�Ʈ �ϸ鼭 �� ��ȯ
        // ������ Ŀ���� ���� ����� ���ҽ��� ������ �ε��ؾ��Ѵ�.
        op.allowSceneActivation = false;

        float timer = 0f;

        while (!op.isDone)
        {
            if (op.progress < 0.9f)
            {
                // ù 90�ۼ�Ʈ�� progress�� ���缭 �ٲ��ְ�
                LoadingGuage.value = op.progress;
            }
            else
            {
                // ������ 10�ۼ�Ʈ�� ����ũ �ε��� ����� �ٲ��ش�.
                // timer += Time.unscaledDeltaTime;
                timer += Time.deltaTime;
                LoadingGuage.value = Mathf.Lerp(0.9f, 1f, timer);
                if (LoadingGuage.value >= 1f)
                {
                    op.allowSceneActivation = true;
                    yield break;
                    // Debug.Log("loading done");
                }
            }
            yield return null;
        }
    }
    

}
