using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager
{
    public Action KeyAction = null;
    public Action<Define.MouseEvent_Left> MouseAction_Left = null;
    public Action<Define.MouseEvent_Right> MouseAction_Right = null;

    bool _pressed_Left = false;
    bool _pressed_Right = false;
    float _pressedTime = 0;

    public void OnUpdate()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.anyKey && KeyAction != null)
            KeyAction.Invoke();

        if (MouseAction_Left != null)
        {
            if (Input.GetMouseButton(0))
            {
                if (!_pressed_Left)
                {
                    MouseAction_Left.Invoke(Define.MouseEvent_Left.PointerDown);
                    _pressedTime = Time.time;
                    
                }
                MouseAction_Left.Invoke(Define.MouseEvent_Left.Press);
                _pressed_Left = true;
            }
            else if (Input.GetMouseButton(1))
            {
                if (!_pressed_Right)
                {
                    MouseAction_Right.Invoke(Define.MouseEvent_Right.PointerDown);
                    _pressedTime = Time.time;

                }
                MouseAction_Right.Invoke(Define.MouseEvent_Right.Press);
                _pressed_Right = true;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (_pressed_Left)
                {
                    if (Time.time < _pressedTime + 0.2f)
                    {
                        MouseAction_Left.Invoke(Define.MouseEvent_Left.Click);
                    }
                    MouseAction_Left.Invoke(Define.MouseEvent_Left.PointerUp);
                }
                _pressed_Left = false;
                _pressedTime = 0;
            }
            else if (Input.GetMouseButtonUp(1))
            {
                if (_pressed_Right)
                {
                    if (Time.time < _pressedTime + 0.2f)
                    {
                        MouseAction_Right.Invoke(Define.MouseEvent_Right.Click);
                    }
                    MouseAction_Right.Invoke(Define.MouseEvent_Right.PointerUp);
                }
                _pressed_Right = false;
                _pressedTime = 0;
            }
        }
    }

    public void Clear()
    {
        KeyAction = null;
        MouseAction_Left = null;
    }
}
