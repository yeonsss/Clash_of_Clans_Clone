using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class ArmyPanel : BaseUI
{
    private enum Buttons
    {
        EditButton,
        MoClearAllBtn,
        MaClearAllBtn,
    } 
    
    private enum Transforms
    {
        MyMonsterList,
        MyMagicList,
    }
    
    private bool _isEdit = false;
    
    public override void Init()
    {
        Bind<Button>(typeof(Buttons));
        Bind<Transform>(typeof(Transforms));
        
        Get<Button>((int)Buttons.MoClearAllBtn).gameObject.BindEvent(MoClearAllHandler);
        Get<Button>((int)Buttons.MaClearAllBtn).gameObject.BindEvent(MaClearAllHandler);
        Get<Button>((int)Buttons.EditButton).gameObject.BindEvent(EditHandler);
    }

    public void Start()
    {
        SocketManager.instance.AddEventHandler(SocketEvent.GET_TASK_COMPLETE, UpdateArmyCor);
    }

    private void Update()
    {
        AutoChangeEditMode();
    }

    private void SetEditActive(bool active)
    {
        var magicList = Get<Transform>((int)Transforms.MyMagicList)
            .GetComponentsInChildren<Button>();
        var monsterList = Get<Transform>((int)Transforms.MyMonsterList)
            .GetComponentsInChildren<Button>();

        foreach (var magic in magicList)
        {
            magic.transform.GetComponent<DoneItem>().EditActive(active);
        }
        
        foreach (var monster in monsterList)
        {
            monster.transform.GetComponent<DoneItem>().EditActive(active);
        }
    }
    
    private void EditHandler()
    {
        _isEdit = _isEdit switch
        {
            true => false,
            false => true
        };

        SetEditActive(_isEdit);
    }

    private async void MaClearAllHandler()
    {
        var magicList = Get<Transform>((int)Transforms.MyMagicList)
            .GetComponentsInChildren<Button>();

        for (int i = 0; i < magicList.Length; i++)
        {
            Destroy(magicList[i].gameObject);
        }

        await NetworkManager.instance.Post<RequestDeleteSelectArmyAllDTO, ResponseDeleteSelectArmyAllDTO>(
            "/army/all",
            new RequestDeleteSelectArmyAllDTO() { type = "Magic" }
        );
    }
    
    private async void MoClearAllHandler()
    {
        var monsterList =  Get<Transform>((int)Transforms.MyMonsterList)
            .GetComponentsInChildren<Button>();
        
        for (int i = 0; i < monsterList.Length; i++)
        {
            Destroy(monsterList[i].gameObject);
        }

        var dto = new RequestDeleteSelectArmyAllDTO()
        {
            type = "Monster"
        };
        await NetworkManager.instance.Post<RequestDeleteSelectArmyAllDTO, ResponseDeleteSelectArmyAllDTO>(
            "/army/all",
            dto
        );
    }

    private void AutoChangeEditMode()
    {
        var magicList = Get<Transform>((int)Transforms.MyMagicList)
            .GetComponentsInChildren<Button>();

        var monsterList = Get<Transform>((int)Transforms.MyMonsterList)
            .GetComponentsInChildren<Button>();

        if (magicList.Length < 1 && monsterList.Length < 1)
        {
            _isEdit = false;
        }
    }
    
    private void AddArmyMonster(string monsterName, int count)
    {
        var ml = Get<Transform>((int)Transforms.MyMonsterList);
        var prevItem = ResourceManager.instance.Instantiate("UI/RADE/DoneItem", ml);
        prevItem.GetComponent<DoneItem>().ItemInit(monsterName, "Monster", count);
    }

    private void AddArmyMagic(string magicName, int count)
    {
        var ml = Get<Transform>((int)Transforms.MyMagicList);
        var prevItem = ResourceManager.instance.Instantiate("UI/RADE/DoneItem", ml);
        prevItem.GetComponent<DoneItem>().ItemInit(magicName, "Magic", count);
        
    }
    
    private IEnumerator UpdateArmyCor(IResponse response)
    {
        if (response is not ResponseGetTaskComplete dto) yield break;
        if (gameObject.activeSelf == false) yield break;

        if (dto.type == "Monster")
        {
            var prevItem = Get<Transform>((int)Transforms.MyMonsterList).Find($"{dto.name}");
                
            if (prevItem == null)
            {
                AddArmyMonster(dto.name, 1);
            }
            else
            {
                prevItem.GetComponent<DoneItem>().AddCount(1);
            }
        }
        else if (dto.type == "Magic")
        {
            var prevItem = Get<Transform>((int)Transforms.MyMagicList).Find($"{dto.name}");
                
            if (prevItem == null)
            {
                AddArmyMagic(dto.name, 1);
            }
            else
            {
                prevItem.GetComponent<DoneItem>().AddCount(1);
            }
        }
        
        yield return null;
    }
    
    public async void UpdateArmyPanel()
    {
        var magicList = Get<Transform>((int)Transforms.MyMagicList);
        var monsterList = Get<Transform>((int)Transforms.MyMonsterList);

        for (int i = 0; i < monsterList.childCount; i++)
        {
            var item = monsterList.GetChild(i).gameObject;
            Destroy(item);
        }
        
        for (int i = 0; i < magicList.childCount; i++)
        {
            var item = magicList.GetChild(i).gameObject;
            Destroy(item);
        }

        var response = await NetworkManager.instance.Get<ResponseGetArmyDTO>("/army");
        if (response.state == true)
        {
            var info = response.armyInfo;
            foreach (var monster in info.selectMonsterMap)
            {
                if (monster.Value == 0) continue;
                var prevItem = monsterList.Find($"{monster.Key}");
                if (prevItem == null)
                {
                    AddArmyMonster(monster.Key, monster.Value);
                }
                else
                {
                    prevItem.GetComponent<DoneItem>().CountChange(monster.Value);
                }
            }

            foreach (var magic in info.selectMagicMap)
            {
                if (magic.Value == 0) continue;
                var prevItem = magicList.Find($"{magic.Key}");
                if (prevItem == null)
                {
                    AddArmyMagic(magic.Key, magic.Value);
                }
                else
                {
                    prevItem.GetComponent<DoneItem>().CountChange(magic.Value);
                }
            }
        }
    }
}
