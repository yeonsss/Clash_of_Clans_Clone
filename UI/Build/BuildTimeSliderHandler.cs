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

    public void Init(GameObject target)
    {
        m_Obj = target;
        m_Slider = GetComponentInChildren<Slider>();
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
                C_BuildProcess = StartCoroutine(CoolProcess());
            }
        }
        
    }
    
    IEnumerator CoolProcess()
    {
        var bp = m_Obj.GetComponent<Build>();
        float startTime = Time.realtimeSinceStartup;
        float delay = bp.CoolTime;
        float timer = 0f;

        while(true) {
            timer = Time.realtimeSinceStartup;
            if (timer - startTime >= delay) {
                Debug.Log("Build Completed");
                m_BuildComplete = true;
                bp.m_CooldownComplete = true;
                yield break;
            }

            SetValue(Mathf.Lerp(0, 1, (timer - startTime) / delay));
            yield return null;
        }
    }

}
