using System;
using Unity.VisualScripting;
using UnityEngine;
using static Define;
using TMPro;

public class CardController : MonoBehaviour
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int Cost { get; set; }
    public int SerialCode { get; set; }

    public CardType type;
    
    public GameObject descriptionObject;
    public GameObject contentImageObject;
    public GameObject goldCostGameObject;
    public GameObject nameObject;
    public GameObject ElixirCost;
    
    private void Start()
    {
        ElixirCost.SetActive(false);
        DisplayUpdate();
    }

    public void DisplayUpdate()
    {
        goldCostGameObject.GetComponent<TMP_Text>().text = Cost.ToString();
        descriptionObject.GetComponent<TMP_Text>().text = Description;
        nameObject.GetComponent<TMP_Text>().text = Name;
        // contentImageObject.GetComponent<Image>()
    }

    public void ClickEvent()
    {
        var currentGold = GameManager.instance.GetResource("Gold");
        if (currentGold < Cost) return;
        
        UIManager.instance.UIActiveSetting(UI.HANDUI, false);
        GameManager.instance.UseResource("Gold", Cost);
        switch (type)
        {
            case CardType.BuildCard :
                ExecuteBuildCard();
                gameObject.SetActive(false);
                break;
            
            case CardType.MagicCard :
                UIManager.instance.UIActiveSetting(UI.GAMEUI, true);
                ExecuteMagicCard();
                gameObject.SetActive(false);
                break;
            
            case CardType.MonsterCard :
                UIManager.instance.UIActiveSetting(UI.GAMEUI, true);
                ExecuteMonsterCard();
                gameObject.SetActive(false);
                break;
        }
    }

    public void ExecuteBuildCard()
    {
        Building data;
        if (DataManager.instance.buildingDict.TryGetValue(SerialCode, out data) == false)
        {
            return;
        }
        var uiPrefab = Resources.Load("Prefabs/WorldSpaceUI/BuildAcceptCancel");
        var building = Resources.Load(data.Levels[0].PrefabPath);
        building.GetComponent<Build>().serialCode = SerialCode;

        if (uiPrefab == null || building == null) return;
        
        UIManager.instance.UIActiveSetting(UI.HANDUI, false);
        GameObject bd = null;
        if (data.XSize % 2 == 0 || data.YSize % 2 == 0)
        {
            bd = Instantiate(building, new Vector3(-0.5f, 0, -0.5f), Quaternion.identity) as GameObject;    
        }
        else
        {
            bd = Instantiate(building, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        }

        if (bd == null) return;
        var ui = Instantiate(uiPrefab, bd.transform);
        ui.GetComponent<BuildFinalChoice>().Init(bd);
    }

    public void ExecuteMonsterCard()
    {

        Monster data;
        if (DataManager.instance.monsterDict.TryGetValue(SerialCode, out data) == false)
        {
            return;
        }

        if (GameManager.instance.OwnedMonsterDict.TryGetValue(data.SerialCode, out var count) == true)
        {
            GameManager.instance.OwnedMonsterDict[data.SerialCode] = count + 1; 
        }
        else
        {
            GameManager.instance.OwnedMonsterDict.Add(data.SerialCode, 1);    
        }
    }

    public void ExecuteMagicCard()
    {
        
    }
}
