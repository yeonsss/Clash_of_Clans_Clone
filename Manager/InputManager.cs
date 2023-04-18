using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using static Define;

public class InputManager : Singleton<InputManager>
{
    private enum ControlPhase
    {
        CameraControl,
        ObjectControl,
        SpawnControl,
        None
    }
    private ControlPhase _phase;
    
    private UnityEvent<Touch> _began;
    private UnityEvent<Touch> _stationary;
    private UnityEvent<Touch> _moved;
    private UnityEvent<Touch> _ended;
    private UnityEvent<Touch> _canceled;
    public UnityEvent<float, float> inputEvent;
    
    private Plane Plane;
    private float mZcoord;
    private LayerMask mask;
    private Vector3 _mousePosOffset;

    private float _timer = 0;
    private Transform touchObj;

    private bool _touchStart = false;

    public void Init()
    {
        inputEvent = new UnityEvent<float, float>();
        _began = new UnityEvent<Touch>();
        _stationary = new UnityEvent<Touch>();
        _moved = new UnityEvent<Touch>();
        _ended = new UnityEvent<Touch>();
        _canceled = new UnityEvent<Touch>();

        mask = LayerMask.GetMask("Ground") 
               | LayerMask.GetMask("Object")
               | LayerMask.GetMask("UI");
        
        _phase = ControlPhase.None;
        inputEvent.AddListener(BattleManager.instance.TouchSpawnEventHandler);
        
        _began.AddListener(SetPlanePos);
        
        _moved.AddListener(MonsterSpawn);
        _moved.AddListener(MoveCamera);
        _moved.AddListener(MoveBuild);
        
        _stationary.AddListener(MonsterSpawn);
        _stationary.AddListener(MoveBuild);
        
        _ended.AddListener(MonsterSpawnOnce);
        _ended.AddListener(ClickBuildOnce);
        _ended.AddListener(CompleteMoveBuild);
    }
    
    private Transform TouchObject(Touch touch)
    {
        var touchRay = Camera.main.ScreenPointToRay(touch.position);
        if (Physics.Raycast(touchRay, out var hit, 100f, mask))
        {
            return hit.transform;
        }

        return null;
    }

    private void SetPlanePos(Touch touch)
    {
        Plane.SetNormalAndPosition(transform.up, transform.position);
        touchObj = TouchObject(touch);
        if (touchObj == null) return;
        
        mZcoord = Camera.main.WorldToScreenPoint(touchObj.position).z;
        _mousePosOffset = touchObj.position - GetMouseWorldPosition(mZcoord);
    }

    private void MoveCamera(Touch touch)
    {
        if (_phase != ControlPhase.CameraControl) return;

        if (touchObj != null && touchObj.CompareTag("Board"))
        {
            var posDelta = PlanePositionDelta(touch);
                
            var position = Camera.main.transform.position;
            float newX = Mathf.Clamp(position.x + posDelta.x, -50f, 0f);
            float newY = Mathf.Clamp(position.z + posDelta.z, -50f, 0f);
                
            position = new Vector3(newX, position.y, newY);
            Camera.main.transform.position = position;
        }
    }

    private void MoveBuild(Touch touch)
    {
        if (_phase != ControlPhase.ObjectControl)
        {
            if (!touchObj.CompareTag("Building")) return;
            if (touchObj.GetComponent<Build>().buildActive == true)
                return;
        }

        if (touchObj.CompareTag("Building"))
        {
            if (touchObj.GetComponent<Build>().buildActive != false)
            {
                touchObj.GetComponent<Build>().buildActive = false;
            }

            Vector3 pos = GetMouseWorldPosition(mZcoord) + _mousePosOffset;
            
            Debug.Log(_mousePosOffset);
            Debug.Log(pos);
            
            pos.y = 0;
            var xSize = touchObj.GetComponent<Build>().XSize;
            var ySize = touchObj.GetComponent<Build>().YSize;

            if (xSize % 2 == 0 || ySize % 2 == 0)
            {
                pos.x = Mathf.Floor(pos.x) - 0.5f;
                pos.z = Mathf.Floor(pos.z) - 0.5f;
            }
            else
            {
                pos.x = Mathf.Floor(pos.x);
                pos.z = Mathf.Floor(pos.z);
            }

            if (pos.z < Y_MIN + 1 || pos.z >= Y_MAX - 1)
            {
                return;
            }
    
            if (pos.x < X_MIN + 1 || pos.x >= X_MAX - 1)
            {
                return;
            }
                    
            touchObj.transform.position = pos;
            
        }
    }

    private async void CompleteMoveBuild(Touch touch)
    {
        if (_phase != ControlPhase.ObjectControl)
        {
            if (!touchObj.CompareTag("Building")) return;
            if (touchObj.GetComponent<Build>().buildActive == true)
                return;
        }

        if (touchObj.CompareTag("Building"))
        {
            var bc = touchObj.GetComponent<Build>();
            if (bc.m_BuildChoiceComplete == true && bc.m_CollisionCheck == false)
            {
                if (bc.buildActive == false)
                {
                    var build = touchObj.GetComponent<Build>();
                    if (build == null) return;
                    bc.buildActive = true;
                            
                    var position = touchObj.position;
                    var requestDto = new RequestMoveBuildDTO()
                    {
                        buildId = build.buildId,
                        posX = position.x,
                        posY = position.z
                    };
                            
                    await NetworkManager.instance.Post<RequestMoveBuildDTO, ResponseMoveBuildDTO>(
                        "/build/position", requestDto
                    );
                }
            }
        }
    }

    private void MonsterSpawn(Touch touch)
    {
        if (_phase != ControlPhase.SpawnControl) return;

        var touchObjForSpawn = TouchObject(touch);
        if (touchObjForSpawn == null) return;
        
        if (touchObjForSpawn.CompareTag("Board"))
        {
            var areaType = touchObjForSpawn.GetComponent<Area>().type;
            if (areaType is AreaType.Building or AreaType.NoSpawnArea)
                return;

            var pos = touchObjForSpawn.position;
            inputEvent.Invoke(pos.x, pos.z);
        }
    }
    
    private void MonsterSpawnOnce(Touch touch)
    {
        if (C_SceneManager.instance.m_currentScene != "BattleScene") return; 
        if (_timer > ClickHoldTime) return;

        var touchObjForSpawn = TouchObject(touch);
        if (touchObjForSpawn == null)
        {
            BattleManager.instance.ClearSpawnState();
            return;
        };
        
        if (touchObjForSpawn.CompareTag("Board"))
        {
            var areaType = touchObjForSpawn.GetComponent<Area>().type;
            if (areaType is AreaType.Building or AreaType.NoSpawnArea)
                return;

            var pos = touchObjForSpawn.position;
            inputEvent.Invoke(pos.x, pos.z);
        }
    }
    private void ClickBuildOnce(Touch touch)
    {
        if (C_SceneManager.instance.m_currentScene != "HomeGround") return; 
        if (_timer > ClickHoldTime) return;

        // var touchObjForSpawn = TouchObject(touch);
        // TODO: 건물 조작 UI 노출
        // 돈 알림 있으면 돈 먹어지고 리턴
    }

    private void ChangePhase()
    {
        if (_timer > ClickHoldTime)
        {
            if (C_SceneManager.instance.m_currentScene == "BattleScene")
            {
                if (touchObj.CompareTag("Board"))
                    _phase = ControlPhase.SpawnControl;    
            }
            else
            {
                if (touchObj.CompareTag("Building"))
                    _phase = ControlPhase.ObjectControl;
            }
        }
    }

    private void Update()
    {
        if (Input.touchCount >= 1)
        {
            if(EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {  
                return;
            }    
        }

        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            _timer += Time.deltaTime;

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    Debug.Log("Touch Begin");
                    _touchStart = true;
                    _phase = ControlPhase.CameraControl;
                    _began.Invoke(touch);
                    _timer = 0;
                    break;
                
                case TouchPhase.Moved:
                    _moved.Invoke(touch);
                    _timer = 0;
                    break;
                
                case TouchPhase.Stationary:
                    ChangePhase();
                    _stationary.Invoke(touch);
                    break;
                
                case TouchPhase.Ended:
                    if (_touchStart == true)
                        _ended.Invoke(touch);
                    _touchStart = false;
                    _timer = 0;
                    break;
                
                case TouchPhase.Canceled:
                    if (_touchStart == true)
                        _ended.Invoke(touch);
                    _touchStart = false;
                    _timer = 0;
                    break;
            }
        }
        
        // zoom
        if (Input.touchCount >= 2)
        {
            var pos1 = PlanePosition(Input.GetTouch(0).position);
            var pos2 = PlanePosition(Input.GetTouch(1).position);

            var pos1b = PlanePosition(Input.GetTouch(0).position - Input.GetTouch(0).deltaPosition);
            var pos2b = PlanePosition(Input.GetTouch(1).position - Input.GetTouch(1).deltaPosition);

            var zoom = Vector3.Distance(pos1, pos2) / Vector3.Distance(pos1b, pos2b);

            if (zoom == 0 || zoom > 10) return;

            Camera.main.transform.position = Vector3.LerpUnclamped(pos1, Camera.main.transform.position, 1 / zoom);
        }
    }
    
    private Vector3 PlanePositionDelta(Touch touch)
    {
        if (touch.phase != TouchPhase.Moved) return Vector3.zero;

        var rayBefore = Camera.main.ScreenPointToRay(touch.position - touch.deltaPosition);
        var rayNow = Camera.main.ScreenPointToRay(touch.position);
        if (Plane.Raycast(rayBefore, out var enterBefore) && Plane.Raycast(rayNow, out var enterNow))
        {
            return rayBefore.GetPoint(enterBefore) - rayNow.GetPoint(enterNow);
        }

        return Vector3.zero;
    }
    
    private Vector3 PlanePosition(Vector2 screenPos)
    {
        var rayCast = Camera.main.ScreenPointToRay(screenPos);
        if (Plane.Raycast(rayCast, out var enterNow))
        {
            return rayCast.GetPoint(enterNow);
        }
            
        return Vector3.zero;
    }
    
    
}
