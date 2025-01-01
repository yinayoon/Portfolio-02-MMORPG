using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    Define.CameraMode _mode = Define.CameraMode.QuarterView;
    
    //[SerializeField]
    public Vector3 Delta = new Vector3(0.0f, 6.0f, -5.0f);
    
    [SerializeField]
    GameObject _player = null;

    public void SetPlayer(GameObject player) { _player = player; }

    void Start()
    {
        
    }

    void LateUpdate()
    {
        if (_mode == Define.CameraMode.QuarterView)
        {
            if (_player.IsValid() == false)
            {
                return;
            }

            RaycastHit hit;
            if (Physics.Raycast(_player.transform.position, Delta, out hit, Delta.magnitude, 1 << (int)Define.Layer.Block) && transform.tag == "MainCamera")
            {
                float dist = (hit.point - _player.transform.position).magnitude * 0.8f;
                transform.position = _player.transform.position + Vector3.up + Delta.normalized * dist;
            }
            else
            {
                transform.position = _player.transform.position + Delta;
                transform.LookAt(_player.transform);
            }            
        }        
    }

    public void SetQuaterView(Vector3 delta)
    {
        _mode = Define.CameraMode.QuarterView;
        Delta = delta;
    }
}
