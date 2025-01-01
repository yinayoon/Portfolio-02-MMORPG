using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro 네임스페이스 추가

public class GameScene : BaseScene
{
    public CursorController cursorController;
    public PlayerController playerController;

    public PlayerStat _playerStat;
    public Image _gameOverImage;
    public TextMeshProUGUI _gameOverText;
    public AudioSource _gameOverAudioSource;
    public AudioClip _gameOverAudioClip;

    bool gameOverSign = false;

    public int monsterCount;
    public int potionCount;

    protected override void Init()
    {
        gameOverSign = false;

        _gameOverAudioSource = GetComponent<AudioSource>();

        base.Init();

        SceneType = Define.Scene.Game;
        //Managers.UI.ShowSeceneUI<UI_Inven>();
        Dictionary<int, Data.Stat> dict = Managers.Data.StatDict;
        cursorController = gameObject.GetOrAddComponent<CursorController>();
        
        GameObject player = Managers.Game.Spawn(Define.WorldObject.Player, "Player");
        player.transform.position = new Vector3(66.10419f, -2.726757f, 6.201507f);
        Camera.main.gameObject.GetOrAddComponent<CameraController>().SetPlayer(player);
        GameObject mapCamera = GameObject.FindGameObjectWithTag("MapCamera");
        
        CameraController cameraController = mapCamera.GetOrAddComponent<CameraController>();
        cameraController.SetPlayer(player);
        cameraController.Delta = new Vector3(0.0f, 30.0f, 0.0f);
        
        _playerStat = player.GetComponent<PlayerStat>();
        _gameOverImage = GameObject.FindGameObjectWithTag("GameOver").GetComponent<Image>();
        _gameOverImage.enabled = false;
        _gameOverText = _gameOverImage.transform.Find("Text_GameOver").GetComponent<TextMeshProUGUI>();
        _gameOverText.text = "";
        
        //Managers.Game.Spawn(Define.WorldObject.Monster, "Lizard");
        GameObject Go = new GameObject { name = "SpawningPool" };
        GameObject monsterGroup = new GameObject { name = "Monster Group" };
        GameObject potionGroup = new GameObject { name = "Potion Group" };
        SpawningPool Pool = Go.GetOrAddComponent<SpawningPool>();
        Pool.monsterObjParent = monsterGroup;
        Pool.potionObjParent = potionGroup;
        Pool.SetKeepMonsterCount(monsterCount);
        Pool.SetKeepPotionCount(potionCount);

        _gameOverAudioSource.clip = _gameOverAudioClip;
    }


    protected override void OnUpdate()
    {
        if (_playerStat.Hp <= 0 && !gameOverSign)
        {
            _gameOverAudioSource.Play();
            StartCoroutine("CoShowGUI");
            gameOverSign = true;
        }
    }

    public override void Clear()
    {
     
    }

    public IEnumerator CoShowGUI()
    {
        _gameOverImage.enabled = true;
        _gameOverText.text = "Game Over!";
        Color _bgColor = _gameOverImage.color;
        Color _textColor = _gameOverText.color;

        for (int i = 0; i < 10; i++)
        {
            float a = i * 0.1f;

            _bgColor.a = a;
            _textColor.a = a;

            _gameOverImage.color = _bgColor;
            _gameOverText.color = _textColor;
            yield return new WaitForSeconds(0.01f);
        }
    }
}
