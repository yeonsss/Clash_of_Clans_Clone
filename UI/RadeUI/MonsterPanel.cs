using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class MonsterPanel : BaseUI
{
    private enum Buttons
    {
        MonsterJobDelAllBtn,
        MonsterAccelerationButton,
    }
        
    private enum Texts
    {
        MonsterLimitCount,
        MonsterJobCount,
    }

    private enum Lists
    {
        MonsterScroll,
        MonsterSelectList,
    }
    
    public override void Init()
    {
        Bind<TMP_Text>(typeof(Texts));
        Bind<Transform>(typeof(Lists));
        Bind<Button>(typeof(Buttons));
        
        FillMonsterList();
    }

    private void Start()
    {
        SocketManager.instance.AddEventHandler(SocketEvent.GET_TASK_START, GetTaskStartCor);
        SocketManager.instance.AddEventHandler(SocketEvent.GET_TASK_COMPLETE, GetTaskCompleteCor);
    }

    private IEnumerator GetTaskStartCor(IResponse response)
    {
        if (response is not ResponseGetTaskStartDTO dto) yield break;
        var type = Enum.Parse<MonsterName>(dto.name);
        JobStart(dto.taskId, dto.name, DataManager.instance.monsterDict[type].SpawnTime);
        yield return null;
    }
    
    private IEnumerator GetTaskCompleteCor(IResponse response)
    {
        if (response is not ResponseGetTaskComplete dto) yield break;
        JobDone(dto.taskId);
        yield return null;
    }
    
    private void JobCountChange(int addCount, string type)
    {
        try
        {
            if (type == "Monster")
            {
                var jobCountText = Get<TMP_Text>((int)Texts.MonsterJobCount).text;
                var maxCountText = Get<TMP_Text>((int)Texts.MonsterLimitCount).text;
                var newCount = Mathf.Clamp(int.Parse(jobCountText) + addCount, 0, int.Parse(maxCountText)) ;
                Get<TMP_Text>((int)Texts.MonsterJobCount).text = newCount.ToString();    
            }
            else if (type == "Magic")
            {
                // magic job
            }
            
        }
        catch (Exception e)
        {
            // ignored
        }
    }
    
    IEnumerator JobProgress(float remainingTime, float endTime, Transform jobItem)
    {
        var start = endTime - remainingTime;
        var startTime = Time.realtimeSinceStartup;
        float timer = 0f;
    
        while(true) {
            timer = Time.realtimeSinceStartup;
            var deltaTime = timer - startTime;
            
            if (start + deltaTime >= endTime)
            {
                // UpdateMonsterPanel();
                Debug.Log("complete");
                yield break;
            }
    
            if (jobItem == null) yield break;

            var progress = Mathf.Lerp(0, 1, (start + deltaTime) / endTime);
            jobItem.GetComponent<JobItem>().ProgressChange(progress);
            yield return null;
        }
    }
    
    public async void UpdateMonsterPanel()
    {
        var response = await NetworkManager.instance.Get<ResponseGetArmyDTO>("/army");
        if (response.state == true)
        {
            var info = response.armyInfo;
            Get<TMP_Text>((int)Texts.MonsterLimitCount).text = info.monsterProdMaxCount.ToString();
            Get<TMP_Text>((int)Texts.MonsterJobCount).text = info.monsterProdCurCount.ToString();
        }
    
        var response2 = await NetworkManager.instance.Get<ResponseGetTaskListDTO>("/task/list");
        var jobScroll = Get<Transform>((int)Lists.MonsterScroll);
        var cCount = jobScroll.childCount;
    
        List<string> childNameList = new List<string>();
        for (int i = 0; i < cCount; i++)
        {
            childNameList.Add(jobScroll.GetChild(i).name);
        }
    
        Dictionary<string, CreateTask> taskInfo = new Dictionary<string, CreateTask>();
        List<string> taskIdList = new List<string>();
        foreach (var t in response2.taskList)
        {
            taskInfo.Add(t._id, t);
            taskIdList.Add(t._id);
        }
    
        var deleteList = childNameList.Except(taskIdList);
        var createList = taskIdList.Except(childNameList);
    
        foreach (var id in deleteList)
        {
            var job = jobScroll.Find(id);
            if (job != null) Destroy(job);
        }
    
        foreach (var id in createList)
        {
            var ti = taskInfo[id];
            var data = DataManager.instance.GetMonsterData(ti.name);
    
            var jBtn = ResourceManager.instance.Instantiate("UI/RADE/JobItem", jobScroll);
            jBtn.name = ti._id;
            
            jBtn.GetComponent<JobItem>().ItemInit(
                ti.name,
                "Monster",
                data.SummonCapacity,
                JobCountChange
            );
        }
    
        foreach (var t in taskInfo)
        {
            if (t.Value.isStart == true)
            {
                JobStart(t.Value._id, t.Value.name, t.Value.remainingTime);
            }
        }
    }
    
    public void JobDone(string taskId)
    {
        
        var job = Get<Transform>((int)Lists.MonsterScroll).Find(taskId);
        if (job == null) return;
        
        Destroy(job.gameObject);
    }
    
    public void JobStart(string taskId, string monsterName, float remainingTime)
    {
        
        var job = Get<Transform>((int)Lists.MonsterScroll).Find(taskId);
        if (job == null) return;
        
        MonsterName type = Enum.Parse<MonsterName>(monsterName);
        var endTime = DataManager.instance.monsterDict[type].SpawnTime;
        StartCoroutine(JobProgress(remainingTime, endTime, job));
    }
    
    private void FillMonsterList()
    {
        var mList = Get<Transform>((int)Lists.MonsterSelectList);
        var jList = Get<Transform>((int)Lists.MonsterScroll);
        foreach (var mn in DataManager.instance.monsterDict)
        {
            var btn = ResourceManager.instance.Instantiate(
                "UI/RADE/JobCreateBtn", 
                mList
            );
            
            var data = DataManager.instance.GetMonsterData(mn.Value.Name);

            btn.GetComponent<JobCreateBtn>().ItemInit(
                mn.Value.Name,
                "Monster",
                data.SummonCapacity,
                JobCountChange,
                jList
            );
        }
    }
}
