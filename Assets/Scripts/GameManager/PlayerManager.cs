using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[Singleton]
public class PlayerManager : MonoBehaviour
{
    public static PlayerManager _instance;
    private void Awake()
    {
        if (!_instance) _instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        SpawnPlayerAtStart();
    }

    [SerializeField] [Range(0, 8)] private short MaxLives = 8, MaxBooms = 8;
    [SerializeField] [Range(0, 8)] private short BoomsPerLives = 3;
    [SerializeField] [Range(0, 8)] private short CurrentLives = 3, CurrentBooms = 2;

    [SerializeField] Sprite LifeAvailable, LifeUnavailable, BoomAvailable, BoomUnavailable;
    [SerializeField] GameObject LifeAndBoomContainerPrefab;
    [SerializeField] GameObject LifeContainers, BoomContainers;

    List<Image> drawnLifes = new();
    List<Image> drawnBooms = new();
    public void DrawUI()
    {
        ResetAll();
        DrawnHoldersForLifesAndBooms();
        DrawnLifesAndBoomsStatus();
    }

    void ResetAll()
    {
        foreach (var item in drawnLifes)
        {
            Destroy(item.gameObject);
        }
        foreach (var item in drawnBooms)
        {
            Destroy(item.gameObject);
        }
        drawnLifes.Clear();
        drawnBooms.Clear();
    }

    void DrawnHoldersForLifesAndBooms()
    {
        Vector3 position = new(-200, 0);
        for (int i = 0; i < MaxLives; i++)
        {
            GameObject o = Instantiate(LifeAndBoomContainerPrefab, LifeContainers.transform);
            drawnLifes.Add(o.GetComponent<Image>());
            o.GetComponent<RectTransform>().localPosition = position;
            position += new Vector3(45, 0);
        }

        position = new(-200, 0);
        for (int i = 0; i < MaxBooms; i++)
        {
            GameObject o = Instantiate(LifeAndBoomContainerPrefab, BoomContainers.transform);
            drawnBooms.Add(o.GetComponent<Image>());
            o.GetComponent<RectTransform>().localPosition = position;
            position += new Vector3(45, 0);
        }
    }

    void DrawnLifesAndBoomsStatus()
    {
        for (int i = 0; i < drawnLifes.Count; i++)
        {
            drawnLifes[i].sprite = i < CurrentLives ? LifeAvailable : LifeUnavailable;
        }
        for (int i = 0; i < drawnBooms.Count; i++)
        {
            drawnBooms[i].sprite = i < CurrentBooms ? BoomAvailable : BoomUnavailable;
        }
    }

    [SerializeField] GameObject PlayerPrefab;
    private PlayerBase Player;
    public PlayerBase ActivePlayer => Player;
    public Vector3 PlayerPosition => ActivePlayer ? ActivePlayer.transform.position : GameManager._instance.CenterDown;

    public Vector3 GetPlayerRespawnPosition()
    {
        Vector3 centerDown = (GameManager._instance.cornerLeftDown.position + GameManager._instance.cornerRightDown.position) / 2;
        return centerDown - Vector3.up * 50f;
    }

    public Vector3 GetPlayerReadyPosition()
    {
        Vector3 centerDown = (GameManager._instance.cornerLeftDown.position + GameManager._instance.cornerRightDown.position) / 2;
        return centerDown + Vector3.up * 20f;
    }

    public void Register(PlayerBase player)
    {
        Player = player;
    }

    public bool CanPlayerRevive => CurrentLives > 0;
    public bool IsPlayerAlive => CanPlayerRevive || ActivePlayer;
    public void OnPlayerDeath(PlayerBase player)
    {
        SpellcardManager._instance.MakeSpellcardCaptureInvalid();
        if (CurrentLives > 0)
        {
            CurrentLives--;
            CurrentBooms = BoomsPerLives;
            DrawUI();
            StartCoroutine(SpawnPlayer());
        }
    }

    IEnumerator SpawnPlayer()
    {
        yield return new WaitForSeconds(1f);
        Instantiate(PlayerPrefab, GetPlayerRespawnPosition(), Quaternion.identity);
    }

    void SpawnPlayerAtStart()
    {
        DrawUI();
        Instantiate(PlayerPrefab, GetPlayerRespawnPosition(), Quaternion.identity);
    }
}
