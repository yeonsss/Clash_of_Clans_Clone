using TMPro;
using UnityEngine.UI;

public class DoneItem : BaseUI
{
    private enum Texts
    {
        Name,
        Count
    }
    
    private enum Images
    {
        JobImage,
        EditModeImage
    }

    private string _name;
    private string _type;

    private bool _isEditMode = false; 

    public override void Init()
    {
        Bind<TMP_Text>(typeof(Texts));
        Bind<Image>(typeof(Images));
        
        gameObject.BindEvent(DeleteBtnHandler);
        transform.GetComponent<Button>().interactable = false;
        EditActive(false);
    }

    public void ItemInit(string text, string type, int count)
    {
        gameObject.name = text;
        
        Get<TMP_Text>((int)Texts.Name).text = text;
        Get<TMP_Text>((int)Texts.Count).text = count.ToString();

        _name = text;
        _type = type;
    }

    public void CountChange(int count)
    {
        Get<TMP_Text>((int)Texts.Count).text = count.ToString();
    }
    
    public void AddCount(int addCount)
    {
        var text = Get<TMP_Text>((int)Texts.Count).text;
        var i = int.Parse(text) + addCount;
        Get<TMP_Text>((int)Texts.Count).text = i.ToString();
    }

    public void EditActive(bool active)
    {
        Get<Image>((int)Images.EditModeImage).gameObject.SetActive(active);
        _isEditMode = active;
    }

    private async void DeleteBtnHandler()
    {
        if (_isEditMode == false) return;
        
        var count = Get<TMP_Text>((int)Texts.Count);
        var currentCount = int.Parse(count.text);
        currentCount -= 1;

        if (currentCount < 1) Destroy(count.gameObject);
        else count.text = currentCount.ToString();

        var dto = new RequestDeleteSelectArmyDTO()
        {
            name = _name,
            type = _type
        };
                    
        await NetworkManager.instance.Post<RequestDeleteSelectArmyDTO, ResponseDeleteSelectArmyDTO>("/army", dto);
    }
}
