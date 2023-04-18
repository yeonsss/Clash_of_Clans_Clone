using System;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Define;
public class BuildAcceptHandler : MonoBehaviour
{
    private void Update()
    {
        var bd = GetComponentInParent<BuildFinalChoice>();
        var button = GetComponentInChildren<Button>();
        if (bd.obj.GetComponent<Build>().m_CollisionCheck == false)
        {
            button.enabled = true;
        }
        else
        {
            button.enabled = false;
        }
    }

    public async void ClickEvent()
    {
        var bd = GetComponentInParent<BuildFinalChoice>();
        if (bd.obj != null)
        {
            var objBuildScript = bd.obj.GetComponent<Build>();
            var position = bd.obj.transform.position;
            
            UIManager.instance.TransformUI(UI.GAMEUI);
            GameManager.instance.UseResource(ResourceType.Gold, bd.buildCost);
            
            var barPrefab = Resources.Load("Prefabs/WorldSpaceUI/BuildSliderBar");
            var bar = Instantiate(barPrefab, bd.obj.transform);
            bar.GetComponent<BuildTimeSliderHandler>().Init(bd.obj);
            
            objBuildScript.buildActive = true;
            objBuildScript.m_BuildChoiceComplete = true;

            if (objBuildScript.type == BuildName.Wall)
            {
                SpawnManager.instance.SpawnWall(position.x, position.z);
            }

            Destroy(transform.parent.gameObject);

            var requestDto = new RequestCreateBuildDto()
            {
                name = objBuildScript.type.ToString(),
                posX = position.x,
                posY = position.z,
                clientTime = DateTime.Now
            };
            
            await NetworkManager.instance.Post<RequestCreateBuildDto, ResponseCreateBuildDto>(
                "/build",
                requestDto
            );
        }
    }
}
