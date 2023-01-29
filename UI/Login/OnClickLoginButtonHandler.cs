using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnClickLoginButtonHandler : MonoBehaviour
{
    public void ClickEvent()
    {
        Debug.Log("Login!!");
        C_SceneManager.instance.SwitchScene("HomeGround");
    }
}
