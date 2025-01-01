using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro 네임스페이스 추가
using System.Collections;

public class UI_HP_Bar_Scene : UI_Base
{
    enum GameObjects
    {
        HPFrame_HealthFill,
        HPFrame_ExpFill,
        Text_Level,
        LevelUp_Bg,
        Text_LevelUp,
        Text_LevelUp_Level,
    }

    public PlayerStat _stat;
    int _level;   
    public GameObject _levelUp_Bg;
    TextMeshProUGUI _text_LevelUp;
    TextMeshProUGUI _text_LevelUp_Level;

    public static float CurrentExp = 0;

    public override void Init()
    {
        _level = 1;
        CurrentExp = 0;

        Bind<GameObject>(typeof(GameObjects));

        _levelUp_Bg = GetObject((int)GameObjects.LevelUp_Bg);
        _text_LevelUp = GetObject((int)GameObjects.Text_LevelUp).GetComponent<TextMeshProUGUI>();
        _text_LevelUp_Level = GetObject((int)GameObjects.Text_LevelUp_Level).GetComponent<TextMeshProUGUI>();

        _levelUp_Bg.SetActive(false);
        _text_LevelUp.text = "Level Up!";
        _text_LevelUp_Level.text = "";

        _stat = transform.parent.GetComponent<PlayerStat>();
        SetExpRatio();
    }

    
    private void Update()
    {
        float ratioHp = _stat.Hp / (float)_stat.MaxHp;
        SetHpRatio(ratioHp);

        CheckLevelUp();
        SetExpRatio();
    }

    public void SetHpRatio(float ratio)
    {
        GetObject((int)GameObjects.HPFrame_HealthFill).GetComponent<Image>().fillAmount = ratio;
    }
   
    public void CheckLevelUp()
    {
        int newLevel = _stat.Level;

        if (_level != _stat.Level)
        {
            _level = _stat.Level;
            SetLevel(_level); // 레벨 Text 변경

            CurrentExp = 0; // 경험치 초기화
            SetExpRatio(); // Exp 초기화

            StartCoroutine(CoShowGUI());
            _text_LevelUp.text = $"Level Up!";
            _text_LevelUp_Level.text = $"Level : {_level}";
        }
    }

    private void SetExpRatio()
    {
        Stat currentStat = _stat;
        if (currentStat != null)
        {
            Data.Stat _prevStat;
            Managers.Data.StatDict.TryGetValue(_level, out _prevStat);

            Data.Stat _curStat;
            Managers.Data.StatDict.TryGetValue(_level + 1, out _curStat);

            if (_curStat != null)
                GetObject((int)GameObjects.HPFrame_ExpFill).GetComponent<Image>().fillAmount = ExpRatioCalc(_prevStat, _curStat);
            if (_curStat == null)
                GetObject((int)GameObjects.HPFrame_ExpFill).GetComponent<Image>().fillAmount = 1;
        }
    }

    private float ExpRatioCalc(Data.Stat prevStat, Data.Stat curStat)
    {
        float maxExp = curStat.totalExp; // 현재 레밸의 최대 Exp 값을 maxExp에 대입
        float ratio = (CurrentExp - prevStat.totalExp) / (maxExp - prevStat.totalExp); // 비율 계산
        
        //Debug.Log($"Max Exp : {maxExp}, Current Exp : {CurrentExp}, Ratio : {ratio}");

        return ratio;
    }

    // LevelUp GUI 켜기
    public IEnumerator CoShowGUI()
    {
        _levelUp_Bg.SetActive(true);

        Color _bgColor = _levelUp_Bg.GetComponent<Image>().color;
        Color _text1Color = _text_LevelUp.color;
        Color _text2Color = _text_LevelUp_Level.color;

        for (int i = 0; i < 10; i++)
        {
            float a = i * 0.1f;

            _bgColor.a = a;
            _text1Color.a = a;
            _text2Color.a = a;

            _levelUp_Bg.GetComponent<Image>().color = _bgColor;
            _text_LevelUp.color = _text1Color;
            _text_LevelUp_Level.color = _text2Color;
            yield return new WaitForSeconds(0.01f);
        }

        yield return new WaitForSeconds(1.0f);

        for (int i = 0; i < 10; i++)
        {
            float a = (9 - i) * 0.1f;

            _bgColor.a = a;
            _text1Color.a = a;

            _levelUp_Bg.GetComponent<Image>().color = _bgColor;
            _text_LevelUp.color = _text1Color;
            _text_LevelUp_Level.color = _text2Color;
            yield return new WaitForSeconds(0.01f);
        }

        _levelUp_Bg.SetActive(false);
    }

    public void SetLevel(int level)
    {
        GetObject((int)GameObjects.Text_Level).GetComponent<TextMeshProUGUI>().text = level.ToString();        
    }
}
