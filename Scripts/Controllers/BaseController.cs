using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseController : MonoBehaviour
{
    [SerializeField]
    protected Vector3 _destPos;

    [SerializeField]
    protected Define.State _state = Define.State.Idle;

    [SerializeField]
    protected GameObject _lockTarget;

    public GameObject LockTarget
    {
        get
        {
            return _lockTarget;
        }
        set
        {
            _lockTarget = value;
        }
    }

    protected bool keycodeR = false;
    public Define.WorldObject WorldObjectType { get; protected set; } = Define.WorldObject.Unknown;
    
    int attackIdx = 0;
    public virtual Define.State State
    {
        get { return _state; }
        set
        {
            _state = value;

            Animator anim = GetComponent<Animator>();
            switch (_state)
            {
                case Define.State.Die:
                    anim.CrossFade("DIE", 0.1f);
                    break;
                case Define.State.Idle:
                    anim.CrossFade("WAIT", 0.1f);
                    break;
                case Define.State.Moving:
                    anim.CrossFade("MOVE", 0.1f);
                    break;
                case Define.State.Runing:
                    anim.CrossFade("RUN", 0.1f);
                    break;
                case Define.State.Attack:
                    if (attackIdx == 0) // ATTACK 1 애니메이션 실행
                        anim.CrossFade("ATTACK 1", 0.1f, -1, 0);
                    if (attackIdx == 1) // ATTACK 2 애니메이션 실행
                        anim.CrossFade("ATTACK 2", 0.1f, -1, 0);

                    // 애니메이션 랜덤 선택 
                    int randomInt = Random.Range(0, 2);
                    attackIdx = randomInt;

                    // 애니메이션 순차 선택 
                    //attackIdx++;
                    //if (attackIdx > 1) attackIdx = 0;

                    break;
                case Define.State.Shield:
                    anim.CrossFade("SHIELD", 0.1f, -1, 0);
                    break;
                case Define.State.Skill:
                    
                    break;
            }
        }
    }

    private void Start()
    {
        attackIdx = 0;

        Init();
    }

    void Update()
    {
        // 뛸지 걸을지 구분
        if (Input.GetKeyDown(KeyCode.R))
        {  
            if (keycodeR)
            {
                keycodeR = false;
            }
            else
            {
                keycodeR = true;
            }
        }

        switch (State)
        {
            case Define.State.Die:
                UpdateDie();
                break;
            case Define.State.Moving:
                UpdateMoving();
                break;
            case Define.State.Runing:
                UpdateRuning();
                break;
            case Define.State.Idle:
                UpdateIdle();
                break;
            case Define.State.Attack:
                UpdateAttack();
                break;
            case Define.State.Shield:
                UpdateShield();
                break;
            case Define.State.Skill:
                UpdateSkill();
                break;
        }

        OnUpdate();
    }

    public abstract void Init();
    protected virtual void OnUpdate() { }
    protected virtual void UpdateDie() { }
    protected virtual void UpdateMoving() { }
    protected virtual void UpdateRuning() { }
    protected virtual void UpdateIdle() { }
    protected virtual void UpdateAttack() { }
    protected virtual void UpdateShield() { }
    protected virtual void UpdateSkill() { }

}
