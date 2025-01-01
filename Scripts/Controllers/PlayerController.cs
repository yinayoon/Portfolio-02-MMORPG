using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// 방어 : 마우스 오른쪽(1) 버튼
public class PlayerController : BaseController
{
    int _mask = (1 << (int)Define.Layer.Ground) | (1 << (int)Define.Layer.Monster);

    public PlayerStat _stat;
    int _level;
    bool _stopAttack = false;

    // 찾을 GameObject의 태그
    GameObject mapTrackMarkerEffectObj;

    [Header ("- GameObject")]
    public GameObject walkDustEffectObj;
    public GameObject runDustEffectObj;
    public GameObject attackEffectObj;
    public GameObject levelUpEffectObj;
    //public GameObject shieldEffectObj;
    public GameObject shieldHitEffectObj;
    public GameObject getHpEffectObj;

    [Header("- ParticleSystem")]
    public ParticleSystem mapTrackMarkerEffect;
    public ParticleSystem walkEffect;
    public ParticleSystem runEffect;
    public ParticleSystem attackEffect;
    public ParticleSystem levelUpEffect;
    //public ParticleSystem shieldEffect;
    public ParticleSystem shieldHitEffect;
    public ParticleSystem getHpEffect;

    [Header("- AudioClip")]
    public AudioClip walkSoundClip;
    public AudioClip runSoundClip;
    public AudioClip attackSoundClip;
    public AudioClip[] humanAtackSoundClip;
    public AudioClip humanAtackDieClip;
    public AudioClip levelUpSoundClip;
    //public AudioClip shieldSoundClip;
    public AudioClip shieldHitSoundClip;
    public AudioClip getHpSoundClip;

    [Header("- AudioSource")]
    public AudioSource walkAudioSource;
    public AudioSource runAudioSource;
    public AudioSource attackAudioSource;
    public AudioSource humanAudioSource;
    public AudioSource levelUpAudioSource;
    //public AudioSource shieldAudioSource;
    public AudioSource shieldHitAudioSource;
    public AudioSource getHpAudioSource;

    int voiceSoundIdx;
    Animator anim;
    bool dieSign;

    public override void Init()
    {
        dieSign = false;
        voiceSoundIdx = 0;
        anim = GetComponent<Animator>();

        walkDustEffectObj.SetActive(false);
        runDustEffectObj.SetActive(false);
        attackEffectObj.SetActive(false);
        shieldHitEffectObj.SetActive(false);
        getHpEffectObj.SetActive(false);

        walkAudioSource.clip = walkSoundClip;
        runAudioSource.clip = runSoundClip;
        attackAudioSource.clip = attackSoundClip;
        humanAudioSource.clip = humanAtackSoundClip[voiceSoundIdx];
        levelUpAudioSource.clip = levelUpSoundClip;
        shieldHitAudioSource.clip = shieldHitSoundClip;
        getHpAudioSource.clip = getHpSoundClip;

        mapTrackMarkerEffectObj = GameObject.FindWithTag("Map Track Marker");
        mapTrackMarkerEffect = mapTrackMarkerEffectObj.GetComponent<ParticleSystem>();

        WorldObjectType = Define.WorldObject.Player;
        _stat = gameObject.GetComponent<PlayerStat>();
        Managers.Input.MouseAction_Left -= OnMouseEvent_Left;
        Managers.Input.MouseAction_Left += OnMouseEvent_Left;

        Managers.Input.MouseAction_Right -= OnMouseEvent_Right;
        Managers.Input.MouseAction_Right += OnMouseEvent_Right;

        _level = 1;

        //if (gameObject.GetComponentInChildren<UI_HPBar>() == null)
        //    Managers.UI.MakeWorldSpaceUI<UI_HPBar>(transform);
    }

    protected override void OnUpdate() 
    {
        LevelUpCheck();
    }

    // 레벨업 효과
    void LevelUpCheck()
    {
        int newLevel = _stat.Level;

        if (_level != newLevel)
        {
            if (!levelUpEffectObj.activeSelf)
            {
                levelUpEffectObj.SetActive(true);
            }
            levelUpAudioSource.Play();
            levelUpEffect.Stop();
            levelUpEffect.Play();
            _level = newLevel;
        }
    }

    protected override void UpdateIdle() 
    {

    }

    protected override void UpdateMoving()
    {
        // 몬스터가 내 사정거리보다 가까우면 공격
        if (_lockTarget != null)
        {
            _destPos = _lockTarget.transform.position;
            float distance = (_destPos - transform.position).magnitude;
            if (distance <= 1)
            {
                State = Define.State.Attack;
                return;
            }
        }

        // 이동
        Vector3 dir = _destPos - transform.position;
        dir.y = 0;

        if (dir.magnitude < 0.1f)
        {
            State = Define.State.Idle;
        }
        else
        {
            Debug.DrawRay(transform.position + Vector3.up * 0.5f, dir.normalized, Color.green);
            if (Physics.Raycast(transform.position, dir, 1.0f, LayerMask.GetMask("Block")))
            {
                if (Input.GetMouseButton(0) == false)
                    State = Define.State.Idle;
                return;
            }

            float moveSpeed = _stat.MoveSpeed;
            float moveDist = Mathf.Clamp(moveSpeed * Time.deltaTime, 0, dir.magnitude);
            transform.position += dir.normalized * moveDist;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);
        }
    }

    protected override void UpdateRuning()
    {
        // 몬스터가 내 사정거리보다 가까우면 공격
        if (_lockTarget != null)
        {
            _destPos = _lockTarget.transform.position;
            float distance = (_destPos - transform.position).magnitude;
            if (distance <= 2)
            {
                State = Define.State.Attack;
                return;
            }
        }

        // 이동
        Vector3 dir = _destPos - transform.position;
        dir.y = 0;

        if (dir.magnitude < 0.1f)
        {
            State = Define.State.Idle;
        }
        else
        {
            Debug.DrawRay(transform.position + Vector3.up * 0.5f, dir.normalized, Color.green);
            if (Physics.Raycast(transform.position, dir, 1.0f, LayerMask.GetMask("Block")))
            {
                if (Input.GetMouseButton(0) == false)
                    State = Define.State.Idle;
                return;
            }

            float moveSpeed = _stat.RunSpeed;
            float moveDist = Mathf.Clamp(moveSpeed * Time.deltaTime, 0, dir.magnitude);
            transform.position += dir.normalized * moveDist;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);
        }
    }

    protected override void UpdateAttack() // 일반 공격
    {
        if (_lockTarget != null)
        {
            Vector3 dir = _lockTarget.transform.position - transform.position;
            dir.y = 0;
            Quaternion quat = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, quat, 20 * Time.deltaTime);
        }
    }

    protected override void UpdateSkill() // 스킬 공격
    {
        if (_lockTarget != null)
        {
            Vector3 dir = _lockTarget.transform.position - transform.position;
            dir.y = 0;
            Quaternion quat = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, quat, 20 * Time.deltaTime);
        }
    }

    void OnHitEvent()
    {
        if (!attackEffectObj.activeSelf)
        {
            attackEffectObj.SetActive(true);
        }
        humanAudioSource.clip = humanAtackSoundClip[voiceSoundIdx];

        attackEffect.Stop();
        attackEffect.Play();
        attackAudioSource.Play();
        humanAudioSource.Play();

        voiceSoundIdx++;

        if (voiceSoundIdx >= humanAtackSoundClip.Length)
        {
            voiceSoundIdx = 0;
        }

        if (_lockTarget != null)
        {
            Stat targetStat = _lockTarget.GetComponent<Stat>();
            targetStat.OnAttacked(_stat);
        }

        if (_stopAttack)
        {
            State = Define.State.Idle;
        }
        else
        {
            State = Define.State.Attack;
        }
    }

    void OnWalkEvent()
    {
        if (!walkDustEffectObj.activeSelf)
        {
            walkDustEffectObj.SetActive(true);
        }

        walkEffect.Stop();
        walkAudioSource.Play();
        walkEffect.Play();
    }

    void OnRunEvent()
    {
        if (!runDustEffectObj.activeSelf)
        {
            runDustEffectObj.SetActive(true);
        }

        runEffect.Stop();
        runAudioSource.Play();        
        runEffect.Play();
    }

    void OnMouseEvent_Left(Define.MouseEvent_Left evt)
    {
        switch (State)
        {
            case Define.State.Idle:
                OnMouseEvent_IdleRun(evt);
                break;
            case Define.State.Moving:
                OnMouseEvent_IdleRun(evt);
                break;
            case Define.State.Runing:
                OnMouseEvent_IdleRun(evt);
                break;
            case Define.State.Attack:
                {
                    if (evt == Define.MouseEvent_Left.PointerUp)
                        _stopAttack = true;
                }
                break;
            case Define.State.Shield:
                OnMouseEvent_IdleRun(evt);
                break;
        }
    }

    void OnMouseEvent_Right(Define.MouseEvent_Right evt)
    {
        //OnMouseEvent_Shield(evt);
    }

    void OnMouseEvent_IdleRun(Define.MouseEvent_Left evt)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool raycastHit = Physics.Raycast(ray, out hit, 30.0f, _mask);

        Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green);

        switch (evt)
        {
            case Define.MouseEvent_Left.PointerDown:
                {
                    if (raycastHit)
                    {
                        _destPos = hit.point;
                        if (keycodeR)
                            State = Define.State.Runing;
                        else
                            State = Define.State.Moving;

                        _stopAttack = false;

                        mapTrackMarkerEffect.transform.position = hit.point;
                        PlayAssignedEffect();

                        if (hit.collider.gameObject.layer == (int)Define.Layer.Monster)
                            _lockTarget = hit.collider.gameObject;
                        else
                            _lockTarget = null;
                    }
                }
                break;
            case Define.MouseEvent_Left.Press:
                {
                    if (_lockTarget == null && raycastHit)
                    {
                        _destPos = hit.point;
                    }
                }
                break;
            case Define.MouseEvent_Left.PointerUp:
                _stopAttack = true;
                break;
        }
    }

    void OnMouseEvent_Shield(Define.MouseEvent_Right evt)
    {
        if (_lockTarget == null)
            return;

        switch (evt)
        {
            case Define.MouseEvent_Right.PointerDown:
                {
                    State = Define.State.Shield;                    
                }
                break;
            case Define.MouseEvent_Right.Press:
                {
                    if (_lockTarget != null)
                    {
                        Stat targetStat = _lockTarget.GetComponent<Stat>();
                        if (targetStat.Hp <= 0)
                            return;

                        Vector3 dir = _lockTarget.transform.position - transform.position;
                        dir.y = 0;
                        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);
                    }
                }
                break;
            case Define.MouseEvent_Right.PointerUp:
                {
                    State = Define.State.Idle;
                }                
                break;
        }
    }

    void OnShileldEvent()
    {

    }

    protected override void UpdateShield() 
    {

    }

    void PlayAssignedEffect()
    {
        if (mapTrackMarkerEffect != null)
        {
            // 이펙트가 이미 재생 중이면 중지하고 다시 재생
            mapTrackMarkerEffect.Stop();
            mapTrackMarkerEffect.Play();
        }
    }


    protected override void UpdateDie() 
    {
        //Debug.Log("Die");

        if (!dieSign)
        {
            Debug.Log("Die!");
            StartCoroutine("DelayDead");
            dieSign = true;
        }
    }

    IEnumerator DelayDead()
    {
        yield return new WaitForSeconds(2);
        Managers.Game.Despawn(this.gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!getHpEffectObj.activeSelf)
        {
            getHpEffectObj.SetActive(true);
        }

        getHpEffect.Stop();
        getHpEffect.Play();
        getHpAudioSource.Play();

        int _plusHp;

        // 충돌한 오브젝트가 "Potion" 태그를 가진 경우
        if (other.CompareTag("HP_Small"))
        {
            _plusHp = 50;
            _stat.Hp += _plusHp;
        }
        else if (other.CompareTag("HP_Middle"))
        {
            _plusHp = 100;
            _stat.Hp += _plusHp;
        }
        else if (other.CompareTag("HP_Large"))
        {
            _plusHp = 150;
            _stat.Hp += _plusHp;
        }

        if (_stat.Hp > _stat.MaxHp) // 만약 hp가 maxHp보다 크면
        {
            _stat.Hp = _stat.MaxHp; // maxHp로 대입
        }


        Destroy(other.gameObject); // 포션 오브젝트 제거

    }
}
