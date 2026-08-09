using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static SpellcardBase;

[Singleton]
public class GameManager : MonoBehaviour
{
    public static GameManager _instance;
    private void Awake()
    {
        if (!_instance)
        {
            _instance = this;
            Score = 0;
            HiScore = PlayerPrefs.GetFloat("HighScore", 0);
            TxtHiScore.text = String.Format("{0:D10}", (int)HiScore);
        }
        else Destroy(gameObject);
    }

    public Transform cornerLeftDown, cornerRightDown, cornerLeftUp, cornerRightUp;
    
    public Vector3 CenterScreen =>
        (cornerLeftDown.position + cornerLeftUp.position + cornerRightDown.position + cornerRightUp.position) / 4;

    public Vector3 CenterLeft => (cornerLeftDown.position + cornerLeftUp.position) / 2;

    public Vector3 CenterRight => (cornerRightDown.position + cornerRightUp.position) / 2;

    public Vector3 CenterUp => (cornerLeftUp.position + cornerRightUp.position) / 2;

    public Vector3 CenterDown => (cornerLeftDown.position + cornerRightDown.position) / 2;

    const float boundary = 300f;
    public bool hasShotOverbound(Vector3 shootPosition)
    {
        return
            shootPosition.x < cornerLeftDown.position.x - boundary
            ||
            shootPosition.y < cornerLeftDown.position.y - boundary
            ||
            shootPosition.x > cornerRightUp.position.x + boundary
            ||
            shootPosition.y > cornerRightUp.position.y + boundary;
    }

    public static LayerMask EnemyLayer = 8, PlayerLayer = 9, ProjectileEnemyLayer = 6, ProjectilePlayerLayer = 7;

    [SerializeField] GameObject NormalBG, SpellcardBG;
    public void SetBackground(SpellType spellType)
    {
        NormalBG.SetActive(spellType == SpellType.NON_SPELL);
        SpellcardBG.SetActive(spellType == SpellType.SPELLCARD);
    }

    private float Score = 0;
    private float HiScore = 0;
    public float GetScore => Score;
    public void AddScore(float score)
    {
        this.Score += score;
        TxtScore.text = String.Format("{0:D10}", (int)Score);

        if (Score > HiScore) HiScore = Score;
        TxtHiScore.text = String.Format("{0:D10}", (int)HiScore);
    }

    [SerializeField] TMP_Text TxtHiScore, TxtScore;

    [SerializeField] private float NaturalScoreGain = 5_000;
    private void Update()
    {
        CheckForGameOver();

        if (IsGameOver) return;
        // AddScore(NaturalScoreGain * Time.deltaTime);
        if (Input.GetKeyDown(InputManager.PauseKey))
        {
            PauseScreen._instance.TogglePause();
        }
    }

    public void OnReplaySave()
    {

    }

    void CheckForGameOver()
    {
        if (IsGameOver || !BeginTracking) return;
        IsGameOver = !PlayerManager._instance.IsPlayerAlive;

        if (IsGameOver) OnGameOver();
    }

    public bool IsGameOver { get; private set; } = false;
    public void OnGameOver()
    {
        IsGameOver = true;
        PauseScreen._instance.SetGameOverScreen();

        PlayerPrefs.SetFloat("HighScore", HiScore);
    }

    public void WaitForSecondThenShowMenu(float c, PauseScreen.DelegateWaiting forcepauseFunc)
        => StartCoroutine(C_WaitForSecondsThenShowMenu(c, forcepauseFunc));

    public IEnumerator C_WaitForSecondsThenShowMenu(float c, PauseScreen.DelegateWaiting forcepause)
    {
        yield return new WaitForSeconds(c);
        forcepause();
    }

    private HashSet<EnemyBase> TrackingEnemies = new();
    private bool BeginTracking = false;
    public void TrackEnemy(EnemyBase enemyBase)
    {
        if (TrackingEnemies.Contains(enemyBase)) return;
        TrackingEnemies.Add(enemyBase);
        BeginTracking = true;
    }
}
