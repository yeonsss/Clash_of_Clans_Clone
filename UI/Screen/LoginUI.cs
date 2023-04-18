using System;
using System.Collections;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class LoginUI : BaseUI
{
    // 로그인이 진행중인지 확인.
    private bool _isLoggingIn = false;

    private enum Buttons
    {
        LoginButton,
        MessageExitButton
    }
    
    private enum InputFields
    {
        InputFieldID,
        InputFieldPassword,
    }
    
    private enum Texts
    {
        Message
    }

    public override void Init()
    {
        Bind<Button>(typeof(Buttons));
        Bind<TMP_Text>(typeof(Texts));
        Bind<TMP_InputField>(typeof(InputFields));
        
        Get<Button>((int)Buttons.LoginButton).gameObject.BindEvent(LoginHandler);
        var btn = Get<Button>((int)Buttons.MessageExitButton).gameObject;
        btn.BindEvent(MessageCloseHandler);
        btn.SetParentActive(false);
    }

    private async void LoginHandler()
    {
        if (_isLoggingIn == false)
        {
            _isLoggingIn = true;
            var requestDto = new RequestLoginDto()
            {
                id = Get<TMP_InputField>((int)InputFields.InputFieldID).text,
                password = Get<TMP_InputField>((int)InputFields.InputFieldPassword).text
            };

            var response = await NetworkManager.instance.Post<RequestLoginDto, ResponseLoginDto>(
                "/login",
                requestDto
            );

            if (response == default)
            {
                var message = Get<TMP_Text>((int)Texts.Message);
                message.gameObject.SetParentActive(true);
                message.text = "Login failed.";
                return;
            }

            if (response.state == true)
            {
                StartCoroutine(SocketManager.instance.Connect());
                C_SceneManager.instance.SwitchScene("HomeGround");
            }
        }
    }

    private void MessageCloseHandler()
    {
        _isLoggingIn = false;
        Get<Button>((int)Buttons.MessageExitButton).gameObject.SetParentActive(false);
    }
}
