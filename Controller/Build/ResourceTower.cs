using System.Collections;
using UnityEngine;
using static Define;
public class ResourceTower : Build
{
    private int m_StorageResource = 0;
    private int m_CompleteTime = 10;
    private int m_MaxResource = 100;
    private int m_ResourceStep = 0;
    
    protected ResourceType m_RType = ResourceType.Gold;
    private bool _isResourceFull = false;
        
    public bool GetResourceFull
    {
        get
        {
            return _isResourceFull;
        }
        set
        {
            if (value == true)
            {
                _isResourceFull = true;
                alertMessage.SetActive(true);
            }
            else
            {
                _isResourceFull = false;
                alertMessage.SetActive(false);
            }
        }
    }

    private Coroutine C_ResourceGain = null;
    private GameObject alertMessage; 

    protected override void Awake()
    {
        base.Awake();
        SocketManager.instance.AddEventHandler(SocketEvent.GET_BUILD_STORAGE, CreditStorageFull);
        InitAlertMessage();
    }

    private void InitAlertMessage()
    {
        if (alertMessage != null) return;
        alertMessage = ResourceManager.instance.Instantiate("WorldSpaceUI/ResourceAlert", gameObject.transform);
        
        alertMessage.BindEvent(GetResource);
        alertMessage.SetActive(false);
    }
    
    private IEnumerator CreditStorageFull(IResponse response)
    {
        if (response is not ResponseGetBuildStorageDto dto) yield break;

        if (dto.buildId == buildId)
        {
            InitAlertMessage();
            GetResourceFull = true;
        }
    }

    private async void GetResource()
    {
        var resourceTowers = FindObjectsOfType<ResourceTower>();
        foreach (var rb in resourceTowers)
        {
            rb.GetResourceFull = false;
        }

        var response = await NetworkManager.instance.Get<ResponseGetResourceDTO>("/build/resource");
        if (response.state == true)
        {
            GameManager.instance.GainResource(m_RType, response.amount);
        }
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
