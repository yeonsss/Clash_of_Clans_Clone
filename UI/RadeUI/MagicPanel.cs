using System;
using UnityEngine;

public class MagicPanel : BaseUI
{
//     private enum Buttons
//     {
//         MagicJobDelAllBtn,
//         MagicAccelerationButton,
//     }
//     
//     private enum Texts
//     {
//         MagicLimitCount,
//         MagicJobCount,
//     }
//
//     private enum Transforms
//     {
//         MagicScroll,
//         MagicSelectList
//     }
//
//     private void Start()
//     {
//         SocketManager.instance.AddEventHandler(SocketEvent.GET_TASK_START, GetTaskStartCor);
//         SocketManager.instance.AddEventHandler(SocketEvent.GET_TASK_COMPLETE, GetTaskCompleteCor);
//     }
//
//     public void JobStart(string taskId, string monsterName, float remainingTime)
//     {
//         
//         var job = monsterPanel[MonsterPanel.MonsterJobScroll].Find(taskId);
//         if (job == null) return;
//          
//         var slider = job.Find("JobSlider");
//         MonsterName type = Enum.Parse<MonsterName>(monsterName);
//         var endTime = DataManager.instance.monsterDict[type].SpawnTime;
//         StartCoroutine(JobProgress(remainingTime, endTime, slider));
//     }
//     
//     public void JobDone(string taskId)
//     {
//         var job = monsterPanel[MonsterPanel.MonsterJobScroll].Find(taskId);
//         if (job == null) return;
//         
//         Destroy(job.gameObject);
//     }
//     
//     public override void Init()
//     {
//         FillMagicList();
//     }
//     
//     private async void UpdateMagicPanel()
//     {
//         var response = await NetworkManager.instance.Get<ResponseGetArmyDTO>("/army");
//         if (response.state == true)
//         {
//             var info = response.armyInfo;
//             magicPanel[MagicPanel.LimitCount].GetComponent<TMP_Text>().text = info.magicProdMaxCount.ToString();
//             magicPanel[MagicPanel.JobCount].GetComponent<TMP_Text>().text = info.magicProdCurCount.ToString();
//         }
//     }
//     
//     private void JobCountChange(int addCount, string type)
//     {
//         try
//         {
//             if (type == "Monster")
//             {
//                 var jobCountText = monsterPanel[MonsterPanel.JobCount].GetComponent<TMP_Text>().text;
//                 var maxCountText = monsterPanel[MonsterPanel.LimitCount].GetComponent<TMP_Text>().text;
//                 var newCount = Mathf.Clamp(int.Parse(jobCountText) + addCount, 0, int.Parse(maxCountText)) ;
//                 monsterPanel[MonsterPanel.JobCount].GetComponent<TMP_Text>().text = newCount.ToString();    
//             }
//             else if (type == "Magic")
//             {
//                 // magic job
//             }
//             
//         }
//         catch (Exception e)
//         {
//             // ignored
//         }
//     }
//     
//     private IEnumerator GetTaskStartCor(IResponse response)
//     {
//         if (response is not ResponseGetTaskStartDTO dto) yield break;
//         var type = Enum.Parse<MonsterName>(dto.name);
//         JobStart(dto.taskId, dto.name, DataManager.instance.monsterDict[type].SpawnTime);
//         yield return null;
//     }
//
//     private IEnumerator GetTaskCompleteCor(IResponse response)
//     {
//         if (response is not ResponseGetTaskComplete dto) yield break;
//         JobDone(dto.taskId);
//         yield return null;
//     }
//     
//     // TODO: job을 받아왔을 때 진행중인 녀석이면 바로 시작.
//     IEnumerator JobProgress(float remainingTime, float endTime, Transform progressBar)
//     {
//         var start = endTime - remainingTime;
//         var startTime = Time.realtimeSinceStartup;
//         float timer = 0f;
//
//         while(true) {
//             timer = Time.realtimeSinceStartup;
//             var deltaTime = timer - startTime;
//             
//             if (start + deltaTime >= endTime)
//             {
//                 // UpdateMonsterPanel();
//                 Debug.Log("complete");
//                 yield break;
//             }
//
//             if (progressBar == null) yield break;
//             progressBar.GetComponent<Slider>().value = Mathf.Lerp(0, 1, (start + deltaTime) / endTime);
//             yield return null;
//         }
//     }
//     
//     private void FillMagicList()
//     {
//         
//     }
//     
}
