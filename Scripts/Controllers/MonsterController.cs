using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : BaseController
{
    Stat _stat;

    [SerializeField]
    float _scanRange = 10;

    [SerializeField]
    float _attackRange = 2;

    public GameObject walkDustEffectObj;
    public GameObject attackEffectObj;
    public ParticleSystem walkEffect;
    public ParticleSystem attackEffect;
    public AudioSource attackAudioSource;
    public AudioClip attackSoundClip;
    public AudioSource monsterAudioSource;
    public AudioClip[] monsterSoundClip;

    int voiceSoundIdx;
    GameObject UI_HPBarGo;

    public override void Init()
    {
        walkDustEffectObj.SetActive(false);
        attackEffectObj.SetActive(false);

        attackAudioSource.clip = attackSoundClip;
        monsterAudioSource.clip = monsterSoundClip[voiceSoundIdx];

        WorldObjectType = Define.WorldObject.Monster;
        _stat = gameObject.GetComponent<Stat>();

        if (gameObject.GetComponentInChildren<UI_HPBar>() == null)
        {
            Managers.UI.MakeWorldSpaceUI<UI_HPBar>(transform);
            UI_HPBarGo = gameObject.GetComponentInChildren<UI_HPBar>().gameObject;
        }
    }

    protected override void OnUpdate() 
    {

    }

    protected override void UpdateIdle()
    {
        // TODO : 메니저가 생기면 옮기자
        GameObject player = Managers.Game.GetPlayer();
        if (player == null)
            return; 

        float distance = (player.transform.position - transform.position).magnitude;

        if (distance <= _scanRange)
        {
            _lockTarget = player;
            State = Define.State.Moving;
            return;
        }
    }

    protected override void UpdateMoving()
    {
        // 플레이어가 내 사정거리보다 가까우면 공격
        if (_lockTarget != null)
        {
            _destPos = _lockTarget.transform.position;
            float distance = (_destPos - transform.position).magnitude;
            if (distance <= _attackRange)
            {
                NavMeshAgent nma = gameObject.GetOrAddComponent<NavMeshAgent>();
                nma.SetDestination(transform.position);
                if (_stat.Hp > 0)
                    State = Define.State.Attack;
                else if (_stat.Hp <= 0)
                    State = Define.State.Die;

                return;
            }
            else if (distance > _scanRange) // 멀면 대기 상태로
            {
                NavMeshAgent nma = gameObject.GetOrAddComponent<NavMeshAgent>();
                nma.SetDestination(transform.position);
                if (_stat.Hp > 0)
                    State = Define.State.Idle;
                else if (_stat.Hp <= 0)
                    State = Define.State.Die;

                _lockTarget = null;

                return;
            }
        }

        // 이동
        Vector3 dir = _destPos - transform.position;
        if (dir.magnitude < 0.1f)
        {
            State = Define.State.Idle;
        }
        else
        {
            // TODO
            NavMeshAgent nma = gameObject.GetOrAddComponent<NavMeshAgent>();
            nma.SetDestination(_destPos);
            nma.speed = _stat.MoveSpeed;

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);
        }
    }

    protected override void UpdateAttack()
    {
        if (_lockTarget != null)
        {
            Vector3 dir = _lockTarget.transform.position - transform.position;
            Quaternion quat = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, quat, 20 * Time.deltaTime);
        }
    }

    void OnWalkEvent()
    {
        if (!walkDustEffectObj.activeSelf)
        {
            walkDustEffectObj.SetActive(true);
        }

        walkEffect.Stop();
        walkEffect.Play();
    }

    public void PlayerShieldEffectOn()
    {
        GameObject monster = _lockTarget.gameObject;
        PlayerController playerController = monster.GetComponent<PlayerController>();

        if (!playerController.shieldHitEffectObj.activeSelf)
        {
            playerController.shieldHitEffectObj.SetActive(true);
            playerController.shieldHitEffect.Stop();
            playerController.shieldHitEffect.Play();
        }
    }

    void OnHitEvent()
    {    
        if (!attackEffectObj.activeSelf)
        {
            attackEffectObj.SetActive(true);
        }
        monsterAudioSource.clip = monsterSoundClip[voiceSoundIdx];

        attackEffect.Stop();
        attackEffect.Play();
        
        //PlayerShieldEffectOn(); // 플레이어 쉴드 이펙트 실행

        attackAudioSource.Play();
        monsterAudioSource.Play();

        voiceSoundIdx++;

        if (voiceSoundIdx >= monsterSoundClip.Length)
        {
            voiceSoundIdx = 0;
        }

        if (_lockTarget != null)
        {
            // 체력
            Stat targetStat = _lockTarget.GetComponent<Stat>();
            targetStat.OnAttacked(_stat);

            if (targetStat.Hp > 0)
            {
                float distance = (_lockTarget.transform.position - transform.position).magnitude;
                if (distance <= _attackRange) 
                {
                    //State = Define.State.Attack;

                    if (_stat.Hp > 0)
                        State = Define.State.Attack;
                    //else if (_stat.Hp <= 0)
                    //    State = Define.State.Die;
                }
                else
                    State = Define.State.Moving;
            }
            else
            {
                State = Define.State.Idle;
            }
        }
        else
        {
            State = Define.State.Idle;
        }
    }

    protected override void UpdateDie()
    {        
        if (UI_HPBarGo != null)
            Managers.Game.Despawn(UI_HPBarGo);
    }
}
