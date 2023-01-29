using UnityEngine;
using static Define;
public class ResourceTower : Build
{
    private int m_StorageResource = 0;
    private int m_CompleteTime = 10;
    private int m_MaxResource = 100;
    private int m_ResourceStep = 0;
    
    protected ResourceType m_RType;
    
    public bool m_GetResourcePossible = false;
    private Coroutine C_ResourceGain = null;

    // 주석 - 자원 수집시간 제거 (자원수집 시간동안 기다린 후 클릭하면 자원 흭득)
    
    protected override void Awake()
    {
        // m_ResourceStep = (int)Mathf.Round(m_MaxResource / m_CompleteTime);
        base.Awake();
    }
    
    protected override void Update()
    {
        // base.Update();        
        // if (buildActive && m_CooldownComplete && C_ResourceGain == null && m_GetResourcePossible == false)
        // {
        //     Debug.Log("Start cor");
        //     C_ResourceGain = StartCoroutine(StorageResource());    
        // }
    }
    
    protected override void UseSkill()
    {
        // if (m_GetResourcePossible == true)
        // {
        //     if (m_RType != ResourceType.None)
        //     {
        //         GameManager.instance.GainResource(ResourceType.Gold, 10);
        //         m_GetResourcePossible = false;
        //         DeleteAlert();
        //     }
        // }
    }

    // IEnumerator StorageResource()
    // {
    //     while (true)
    //     {
    //         yield return new WaitForSecondsRealtime(1);
    //         if (m_StorageResource >= m_MaxResource)
    //         {
    //             m_GetResourcePossible = true;
    //             m_StorageResource = 0;
    //             CreateAlert();
    //             C_ResourceGain = null;
    //             yield break;
    //         }
    //         m_StorageResource = Mathf.Clamp(m_StorageResource + m_ResourceStep, 0, m_MaxResource);
    //     }
    // }
    
    // private void CreateAlert()
    // {
    //     var m_AlertPrefeb = Resources.Load("Prefabs/ResourceAlertMessage") as GameObject;
    //     if (m_AlertPrefeb != null)
    //     {
    //         Vector3 alertPos = new(transform.position.x + 1, 1, transform.position.z + 1);
    //         Instantiate(m_AlertPrefeb, alertPos, Quaternion.Euler(45, 45, 0), transform);
    //         // 메시지에 글자 수정
    //         var msgbox = gameObject.GetComponentInChildren<ResAlertMessageHandler>();
    //         if (msgbox != null)
    //         {
    //             msgbox.SetMessage(m_RType);
    //         }
    //     }
    // }
    //
    // private void DeleteAlert()
    // {
    //     
    //     var ResAlert = gameObject.GetComponentInChildren<ResAlertMessageHandler>();
    //     if (ResAlert != null)
    //     {
    //         var msg = ResAlert.transform.parent;
    //         Destroy(msg.gameObject);    
    //     }
    // }
    
}
