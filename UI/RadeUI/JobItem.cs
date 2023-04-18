using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JobItem : BaseUI
{
    private enum Buttons
    {
        JobDeleteBtn
    }
    
    private enum Texts
    {
        JobName
    }
    
    private enum Sliders
    {
        JobSlider
    }
    
    private string _type;
    private int _summonCapacity;
    private Action<int, string> _action;

    public override void Init()
    {
        Bind<Button>(typeof(Buttons));
        Bind<TMP_Text>(typeof(Texts));
        Bind<Slider>(typeof(Sliders));
    }

    public void ProgressChange(float progress)
    {
        Get<Slider>((int)Sliders.JobSlider).value = progress;
    }

    public void ItemInit(string text, string type, int summonCapacity, Action<int, string> action)
    {
        _type = type;
        _summonCapacity = summonCapacity;
        _action = action;

        Get<TMP_Text>((int)Texts.JobName).text = text;
        Get<Button>((int)Buttons.JobDeleteBtn).gameObject.BindEvent(JobDeleteHandler);
    }

    private async void JobDeleteHandler()
    {
        _action.Invoke(_summonCapacity * -1, _type);
        NetworkManager.instance.Delete<ResponseDeleteTaskDTO>($"/task/{gameObject.name}");
        Destroy(gameObject);
    }
}
