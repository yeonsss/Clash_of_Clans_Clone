using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera _camera;
    public Plane Plane;
    Transform touchObjectTransform = null;
    LayerMask mask;
    
    private void Awake()
    {
        if (_camera == null) _camera = Camera.main;
        mask = LayerMask.GetMask("Ground") | LayerMask.GetMask("Object");
    }

    protected Vector3 PlanePositionDelta(Touch touch)
    {
        if (touch.phase != TouchPhase.Moved) return Vector3.zero;

        var rayBefore = _camera.ScreenPointToRay(touch.position - touch.deltaPosition);
        var rayNow = _camera.ScreenPointToRay(touch.position);
        if (Plane.Raycast(rayBefore, out var enterBefore) && Plane.Raycast(rayNow, out var enterNow))
        {
            return rayBefore.GetPoint(enterBefore) - rayNow.GetPoint(enterNow);
        }

        return Vector3.zero;
    }

    protected Vector3 PlanePosition(Vector2 screenPos)
    {
        var rayCast = _camera.ScreenPointToRay(screenPos);
        if (Plane.Raycast(rayCast, out var enterNow))
        {
            return rayCast.GetPoint(enterNow);
        }
            
        return Vector3.zero;
    }

    private void Update()
    {
        Touch touch = default;

        if (Input.touchCount >= 1)
        {
            Plane.SetNormalAndPosition(transform.up, transform.position);
        }

        var Delta1 = Vector3.zero;
        var Delta2 = Vector3.zero;

        // Drag
        if (Input.touchCount >= 1)
        {
            touch = Input.GetTouch(0);
            
            // TouchPhase.Stationary => 건물 움직임

            if (touch.phase == TouchPhase.Began)
            {
                var touchRay = _camera.ScreenPointToRay(touch.position);
                if (Physics.Raycast(touchRay, out var hit, 100f, mask))
                {
                    touchObjectTransform = hit.transform;    
                }
            }

            if (touch.phase == TouchPhase.Moved)
            {
                if (touchObjectTransform == null) return;

                if (touchObjectTransform.CompareTag("Board"))
                {
                    Delta1 = PlanePositionDelta(touch);
                
                    var position = _camera.transform.position;
                    float newX = Mathf.Clamp(position.x + Delta1.x, -50f, 0f);
                    float newY = Mathf.Clamp(position.z + Delta1.z, -50f, 0f);
                
                    position = new Vector3(newX, position.y, newY);
                    _camera.transform.position = position;
                    // _camera.transform.Translate(Delta1, Space.World);
                    return;
                }
            }
            
            if (touch.phase == TouchPhase.Ended)
            {
                touchObjectTransform = null;
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

            _camera.transform.position = Vector3.LerpUnclamped(pos1, _camera.transform.position, 1 / zoom);
        }
    }
}
