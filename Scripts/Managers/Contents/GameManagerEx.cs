using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerEx
{
    // int <-> GameObject

    GameObject _player;
    //Dictionary<int, GameObject> _player = new Dictionary<int, GameObject>();
    HashSet<GameObject> _monsters = new HashSet<GameObject>();
    HashSet<GameObject> _posions = new HashSet<GameObject>();

    public Action<int> OnSpawnMonsterEvent;
    public Action<int> OnSpawnPotionEvent;

    public GameObject GetPlayer() { return _player; }

    public GameObject Spawn(Define.WorldObject type, string path, Transform parent = null)
    {
        GameObject go = Managers.Resource.Instantiate(path, parent);

        switch (type)
        {
            case Define.WorldObject.Monster:
                _monsters.Add(go);
                if (OnSpawnMonsterEvent != null)
                    OnSpawnMonsterEvent.Invoke(1);
                break;
            case Define.WorldObject.Potion:
                _posions.Add(go);
                if (OnSpawnPotionEvent != null)
                    OnSpawnPotionEvent.Invoke(1);
                break;
            case Define.WorldObject.Player:
                _player = go;
                break;
        }

        return go;
    }

    public Define.WorldObject GetWorldObjectType(GameObject go)
    {
        BaseController bc = go.GetComponent<BaseController>();

        if (bc == null)
            return Define.WorldObject.Unknown;

        return bc.WorldObjectType;
    }

    public void Despawn(GameObject go)
    {
        Define.WorldObject type = GetWorldObjectType(go); 

        switch (type)
        {
            case Define.WorldObject.Monster:
                {
                    if (_monsters.Contains(go))
                    {
                        _monsters.Remove(go);
                        if (OnSpawnMonsterEvent != null)
                            OnSpawnMonsterEvent.Invoke(-1);
                    }
                }
                break;
            case Define.WorldObject.Potion:
                {
                    if (_posions.Contains(go))
                    {
                        _posions.Remove(go);
                        if (OnSpawnPotionEvent != null)
                            OnSpawnPotionEvent.Invoke(-1);
                    }
                }
                break;
            case Define.WorldObject.Player:
                {
                    if (_player == go)
                        _player = null;
                }
                break;
        }

        Managers.Resource.Destroy(go);
    }
}
