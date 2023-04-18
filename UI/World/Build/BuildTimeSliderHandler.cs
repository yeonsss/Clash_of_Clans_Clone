using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static Define;
public class BuildTimeSliderHandler : MonoBehaviour
{
    private GameObject m_Obj;
    private Slider m_Slider;
    private Coroutine C_BuildProcess = null;
    private bool m_BuildComplete = false;
    public float remainingTime = 0;

    public void Init(GameObject target, float rtime = 0)
    {
        m_Obj = target;
        m_Slider = GetComponentInChildren<Slider>();
        var bp = m_Obj.GetComponent<Build>();
        remainingTime = DataManager.instance.buildingDict[bp.type].BuildTime - rtime;
    }

    public void SetValue(float value)
    {
        if (m_Slider != null)
        {
            m_Slider.value = value;    
        }
    }

    private void Update()
    {
        if (m_BuildComplete)
        {
            Destroy(gameObject);
        }
        else
        {
            if (C_BuildProcess == null && m_Obj != null)
            {
                var bp = m_Obj.GetComponent<Build>();
                var end = DataManager.instance.buildingDict[bp.type].BuildTime;
                var start = end - remainingTime;
                C_BuildProcess = StartCoroutine(CoolProcess(start, end, bp));
            }
        }
        
    }
    
    IEnumerator CoolProcess(float start, float end, Build buildScript)
    {
        float startTime = Time.realtimeSinceStartup;
        float timer = 0f;

        while(true) {
            timer = Time.realtimeSinceStartup;
            if (timer - startTime >= end) {
                Debug.Log("Build Completed");
                m_BuildComplete = true;
                buildScript.m_CooldownComplete = true;
                // TODO: 서버에 완료된건지 체크해야하나?? 고민
                yield break;
            }

            SetValue(Mathf.Lerp(0, 1, (start + (timer - startTime)) / end));
            yield return null;
        }
    }

}
