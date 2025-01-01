using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : Stat
{
    [SerializeField]
    protected int _exp;
    [SerializeField]
    protected int _gold;
    [SerializeField]
    protected float _runSpeed;
    [SerializeField]
    protected int _shieldDefense;

    PlayerController playerController;
    bool deadSign;

    public float RunSpeed { get { return _runSpeed; } set { _runSpeed = value; } }

    public int Exp 
    {
        get { return _exp; } 
        set 
        { 
            _exp = value;

            int level = Level;
            while (true)
            {
                Data.Stat stat;
                if (Managers.Data.StatDict.TryGetValue(level + 1, out stat) == false)
                    break;
                if (_exp < stat.totalExp)
                    break;
                level++;
            }

            if (level != Level)
            {
                Level = level;
                SetStat(Level);
            }
        } 
    }

    public int Gold { get { return _gold; } set { _gold = value; } }

    private void Start()
    {
        playerController = GetComponent<PlayerController>();

        deadSign = true;

        _level = 1;

        _exp = 0;
        _defense = 5;
        _moveSpeed = 2.0f;
        _gold = 0;
        _runSpeed = 5.0f;
        _shieldDefense = 10;

        SetStat(_level);
    }

    public void SetStat(int level)
    {
        Dictionary<int, Data.Stat> dict = Managers.Data.StatDict;
        Data.Stat stat = dict[level];
        _hp = stat.maxHp;
        _maxHp = stat.maxHp;
        _attack = stat.attack;
    }

    public void OnShielded(Stat attacker)
    {

    }


    public override void OnAttacked(Stat attacker)
    {
        int damage = Mathf.Max(0, attacker.Attack - Defense);
        Hp -= damage;
        if (Hp <= 0 && deadSign)
        {
            Hp = 0;

            if (playerController != null)
                playerController.State = Define.State.Die;

            OnDead(attacker);

            // 사망
            transform.gameObject.layer = 0;
            deadSign = false;
        }
    }

    protected override void OnDead(Stat attacker)
    {
        //Debug.Log("Player Dead");
        playerController.humanAudioSource.clip = playerController.humanAtackDieClip;
        playerController.humanAudioSource.Play();

        playerController.State = Define.State.Die;
        //StartCoroutine(DelayAction(2.0f));
    }
    
    private IEnumerator DelayAction(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        Managers.Game.Despawn(gameObject);
    }
}
