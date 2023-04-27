using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using static Define;
using System.Threading.Tasks;


public class NetworkManager : Singleton<NetworkManager>
{
    public string sid;

    public override void Init()
    {
        UnityWebRequest.ClearCookieCache();
    }

    public async Task<T> Get<T>(string url)
    {
        try
        {
            var reqURL = SERVER_URI + url;
            using var www = UnityWebRequest.Get(reqURL);
            www.SetRequestHeader("Content-Type", "application/json");

            var result = www.SendWebRequest();

            while (!result.isDone) await Task.Yield();

            if (www.result != UnityWebRequest.Result.Success)
            {
                throw new Exception($"Field : {www.error}");
            }

            var data = JsonSerializationConverter.Deserialize<T>(www.downloadHandler.text);
            return data;
        }
        catch (Exception e)
        {
            // 실패시 로그인페이지로 이동시켜야 한다...
            return default;
        }
    }
    
    public async Task<T2> Post<T1, T2>(string url, T1 body)
    {
        try
        {
            var reqURL = SERVER_URI + url;
            string serializeBody = JsonConvert.SerializeObject(body);
            using var www = UnityWebRequest.Post(reqURL, serializeBody);

            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(serializeBody);
            www.uploadHandler = new UploadHandlerRaw(jsonToSend);
            www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("credentials", "include");

            var result = www.SendWebRequest();

            while (!result.isDone) await Task.Yield();

            if (www.result != UnityWebRequest.Result.Success)
            {
                throw new Exception($"Field : {www.error}");
            }

            if (sid == null) sid = www.GetResponseHeader("Set-Cookie");

            var data = JsonSerializationConverter.Deserialize<T2>(www.downloadHandler.text);
            return data;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.StackTrace);
            // 실패시 로그인페이지로 이동시켜야 한다...
            return default;
        }
    }
    
    public async Task<T> Delete<T>(string url)
    {
        try
        {
            var reqURL = SERVER_URI + url;
            using var www = UnityWebRequest.Delete(reqURL);
            www.SetRequestHeader("Content-Type", "application/json");

            var result = www.SendWebRequest();

            while (!result.isDone) await Task.Yield();

            if (www.result != UnityWebRequest.Result.Success)
            {
                throw new Exception($"Field : {www.error}");
            }

            var data = JsonSerializationConverter.Deserialize<T>(www.downloadHandler.text);
            return data;
        }
        catch (Exception e)
        {
            // 실패시 로그인페이지로 이동시켜야 한다...
            return default;
        }
    }
}
