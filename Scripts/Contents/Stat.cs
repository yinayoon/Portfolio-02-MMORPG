using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stat : MonoBehaviour
{
    [SerializeField]
    protected int _level;
    [SerializeField]
    protected int _hp;
    [SerializeField]
    protected int _maxHp;
    [SerializeField]
    protected int _attack;
    [SerializeField]
    protected int _defense;
    [SerializeField]
    protected float _moveSpeed;

    MonsterController monsterController;

    public int Level { get { return _level; } set { _level = value; } }
    public int Hp { get { return _hp; } set { _hp = value; } }
    public int MaxHp { get { return _maxHp; } set { _maxHp = value; } }
    public int Attack { get { return _attack; } set { _attack = value; } }
    public int Defense { get { return _defense; } set { _defense = value; } }
    public float MoveSpeed { get { return _moveSpeed; } set { _moveSpeed = value; } }

    public int exp = 2;
    private void Start()
    {
        monsterController = GetComponent<MonsterController>();
    }

    public virtual void OnAttacked(Stat attacker)
    {
        int damage = Mathf.Max(0, attacker.Attack - Defense);
        Hp -= damage;
        if (Hp <= 0)
        {
            Hp = 0;

            if (monsterController != null)
                monsterController.State = Define.State.Die;

            OnDead(attacker);

            // 사망
            transform.gameObject.layer = 0;
        }
    }    

    protected virtual void OnDead(Stat attacker)
    {
        PlayerStat playerStat = attacker as PlayerStat;
        if (playerStat != null)
        {
            playerStat.Exp += exp;
            UI_HP_Bar_Scene.CurrentExp = playerStat.Exp;
        }

        StartCoroutine(DelayAction(2.0f));
    }

    protected virtual void OnMonsterDeadSound()
    { 
    
    }

    private IEnumerator DelayAction(float delayTime)
    {       
        yield return new WaitForSeconds(delayTime);
        Managers.Game.Despawn(gameObject);
    }
}
