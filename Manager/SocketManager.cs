using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using SocketIOClient;
using UnityEngine;
using static Define;

public class EventDictionary
{

    private Dictionary<SocketEvent, SocketEventHandler> _dict = new();
    
    public void Add(SocketEvent se, SocketEventHandler eh)
    {
        if (_dict.ContainsKey(se))
        {
            _dict[se] += eh;
        }
        else
        {
            _dict.Add(se, eh);
        }
    }
    
    public void Unregister(SocketEvent se, SocketEventHandler eh)
    {
        if (_dict.ContainsKey(se))
        {
            _dict[se] -= eh;
        }
    }

    public SocketEventHandler this[SocketEvent eventId] => _dict[eventId];
}

public class SocketManager : Singleton<SocketManager>
{
    private Uri uri = new Uri(SERVER_URI);
    private SocketIOUnity socket;
    private EventDictionary eventDict;

    public void Init()
    {
        socket = new SocketIOUnity(uri, new SocketIOOptions()
        {
            Path = "/socket",
            ExtraHeaders = new Dictionary<string, string>()
        });

        eventDict = new EventDictionary();
        
        // very important - unityThread -
        socket.On(SocketEvent.GET_TASK_START.ToString(), response =>
        {
            var dto = response.GetValue<ResponseGetTaskStartDTO>();
            UnityThread.executeCoroutine(StartSocketEvent(SocketEvent.GET_TASK_START, dto));
        });
        
        socket.On(SocketEvent.GET_TASK_COMPLETE.ToString(), response =>
        {
            var dto = response.GetValue<ResponseGetTaskComplete>();
            UnityThread.executeCoroutine(StartSocketEvent(SocketEvent.GET_TASK_COMPLETE, dto));
        });
        
        socket.On(SocketEvent.GET_BUILD_STORAGE.ToString(), response =>
        {
            var dto = response.GetValue<ResponseGetBuildStorageDto>();
            UnityThread.executeCoroutine(StartSocketEvent(SocketEvent.GET_BUILD_STORAGE, dto));
        });
    }

    public void Send(SocketEvent evt, IRequest request)
    {
        socket.Emit(evt.ToString(), request);
    }

    public void AddEventHandler(SocketEvent evt, SocketEventHandler handler)
    {
        eventDict.Add(evt, handler);
    }
    
    private IEnumerator StartSocketEvent(SocketEvent id, IResponse response)
    {
        foreach (var @delegate in eventDict[id].GetInvocationList())
        {
            yield return @delegate.DynamicInvoke(response);
        }
    }

    public IEnumerator Connect()
    {
        socket.Options.ExtraHeaders.Add("Cookie", NetworkManager.instance.sid);
        Debug.Log("socket connect");
        yield return socket.ConnectAsync();
    }
}
