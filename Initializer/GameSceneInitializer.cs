using Newtonsoft.Json;
using UnityEngine;
using static Define;
public class GameSceneInitializer : Initializer
{
    public override async void Init()
    {
        if (GameManager.instance.isInit == false)
        {
            SpawnManager.instance.SpawnBoard();

            var response = await NetworkManager.instance.Get<ResponseGetMyDataDTO>("/user");

            if (response == default)
            {
                C_SceneManager.instance.SwitchScene("LoginScene");
            }
            else
            {
                if (response.state == true)
                {
                    SpawnManager.instance.SpawnInit(response);
                    GameManager.instance.InitResource(ResourceType.Gold, response.userInfo.credit);
                }
                else
                {
                    C_SceneManager.instance.SwitchScene("LoginScene");
                }
            }
            GameManager.instance.isInit = true;
        }
        UIManager.instance.TransformUI(UI.GAMEUI);
    }
}