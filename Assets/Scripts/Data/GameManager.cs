using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[Singleton]
public class GameManager : MonoBehaviour
{
    public static GameManager _instance;
    private void Awake()
    {
        if (!_instance) _instance = this;
        else Destroy(gameObject);
    }

    public Transform cornerLeftDown, cornerRightDown, cornerLeftUp, cornerRightUp;
    public Vector3 CenterScreen =>
        (cornerLeftDown.position + cornerLeftUp.position + cornerRightDown.position + cornerRightUp.position) / 4;

    public static LayerMask EnemyLayer = 8, PlayerLayer = 9, ProjectileEnemyLayer = 6, ProjectilePlayerLayer = 7;
}
