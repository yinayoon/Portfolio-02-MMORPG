using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    int _mask = (1 << (int)Define.Layer.Ground) | (1 << (int)Define.Layer.Monster);

    Texture2D _attackIcon;
    Texture2D _handIcon;

    GameObject _target;

    GameObject player;

    enum CursorType
    {
        None,
        Attack,
        Hand,
    }

    CursorType _cursorType = CursorType.None;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        _attackIcon = Managers.Resource.Load<Texture2D>("Textures/Cursor/Attack");
        _handIcon = Managers.Resource.Load<Texture2D>("Textures/Cursor/Hand");
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100.0f, _mask))
        {
            if (hit.collider.gameObject.layer == (int)Define.Layer.Monster)
            {
                if (_cursorType != CursorType.Attack)
                {
                    // 크리쳐 선택 이펙트 켜기
                    _target = hit.transform.gameObject;
                    if (player == null) // Player가 씬에 존재하지 않는 경우
                    {
                        _target.GetComponent<RenderingLayerManager>().layerManagingToggle = false;
                        Cursor.SetCursor(_handIcon, new Vector2(_handIcon.width / 5, 0f), CursorMode.Auto); // 커서
                    }
                    else // Player가 씬에 존재하는 경우
                    {
                        _target.GetComponent<RenderingLayerManager>().layerManagingToggle = true;
                        Cursor.SetCursor(_attackIcon, new Vector2(_attackIcon.width / 5, 0f), CursorMode.Auto); // 커서
                        _cursorType = CursorType.Attack;
                    }                                        
                }
            }
            else
            {
                if (_cursorType != CursorType.Hand)
                {
                    // 크리쳐 선택 이펙트 끄기
                    if (_target != null)
                    {
                        _target.GetComponent<RenderingLayerManager>().layerManagingToggle = false;
                        _target = null;
                    }

                    // 커서
                    Cursor.SetCursor(_handIcon, new Vector2(_handIcon.width / 3, 0f), CursorMode.Auto);
                    _cursorType = CursorType.Hand;
                }
            }
        }
    }
}
