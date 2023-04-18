using System;
using TMPro;
using UnityEngine;

public class JobCreateBtn : BaseUI
{
    private enum Texts
    {
        ObjectName
    }

    private string _name;
    private string _type;
    private int _summonCapacity;
    private Transform _jobScroll;
    private Action<int, string> _action;
    
    public override void Init()
    {
        Bind<TMP_Text>(typeof(Texts));
    }

    public void ItemInit(string text, string type, int summonCapacity, Action<int, string> action, Transform t)
    {
        Get<TMP_Text>((int)Texts.ObjectName).text = text;
        gameObject.BindEvent(ClickHandler);
        _name = text;
        _type = type;
        _jobScroll = t;
        _action = action;
        _summonCapacity = summonCapacity;
    }

    private async void ClickHandler()
    {
        _action.Invoke(_summonCapacity, _type);
        
        var jBtn = ResourceManager.instance.Instantiate(
            "UI/RADE/JobItem", _jobScroll);
        
        
        jBtn.GetComponent<JobItem>().ItemInit(_name, _type, _summonCapacity, _action);
        var response = await NetworkManager.instance.Post<RequestCreateTaskDTO, ResponseCreateTaskDTO>(
            "/task", 
            new RequestCreateTaskDTO()
            {
                name = _name,
                type = _type, 
            } 
        );
        jBtn.name = response.taskId;
    }
}
