using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static SpellcardBase;

public class BossHealthBar : MonoBehaviour
{
    public static BossHealthBar _instance;
    private void Awake()
    {
        _instance = this;
    }

    [SerializeField] TMP_Text BossName;

    [SerializeField] GameObject LifesIndicatorContainer;
    [SerializeField] GameObject LifeIndicatorPrefab;

    EnemyBase attached;
    [SerializeField] Transform EnemyIndicator;
    public void AttachHUDToBoss(EnemyBase test)
    {
        SetHUDEmpty();

        attached = test;
        EnemyIndicator.gameObject.SetActive(true);
        BossName.text = attached.getName;

        InitializeHealthBar();
        InitializeLifesIndicator();
    }

    [SerializeField] TMP_Text SpellNameText, SpellTimer;
    [SerializeField] GameObject SpellNameBar;
    public void SetSpellName(SpellData data)
    {
        SpellNameText.text = data.Name;
        SpellNameBar.SetActive(data.SpellType == SpellType.SPELLCARD);
        if (SpellNameBar.activeSelf) SpellcardManager._instance.DoSpellAnimation();
    }

    public void ClearSpell() => SpellNameBar.SetActive(false);

    [SerializeField] GameObject healthBarParent;
    [SerializeField] float maxWidth = 700;
    [SerializeField] GameObject healthBarPref;

    List<HealthBarScript> healthBars = new();

    void InitializeHealthBar()
    {
        healthBars.Clear();
        Vector3 initializePosition = Vector3.zero;

        SpellData[] spellDatas = attached.getSpellDatas;
        Color[] healthBarColors = HealthBarScript.GetHealthBarColorsForSpells(spellDatas);

        int count = spellDatas.Length;
        float healthSum = attached.GetSumHealth();

        for (int i = 0; i < count; ++i)
        {
            int index = count - 1 - i;
            SpellData data = spellDatas[index];
            float health = data.Health;
            float width = Mathf.Lerp(0, maxWidth, health / healthSum);

            GameObject o = Instantiate(healthBarPref, healthBarParent.transform);
            o.transform.localPosition = initializePosition;

            RectTransform rect = o.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(width, rect.sizeDelta.y);

            initializePosition += new Vector3(width, 0);

            HealthBarScript healthBarScript = o.GetComponent<HealthBarScript>();
            healthBarScript.SetColor(healthBarColors[index]);

            healthBars.Add(healthBarScript);
        }

        healthBars.Reverse();

        UpdateHealthBar();
    }

    List<GameObject> LifesInd = new();
    void InitializeLifesIndicator()
    {
        foreach (GameObject obj in LifesInd)
        {
            Destroy(obj);
        }
        LifesInd.Clear();

        int count = attached.getRemainingLives;
        float currentX = 0;
        for (int i = 0; i < count; i++)
        {
            GameObject o = Instantiate(LifeIndicatorPrefab, LifesIndicatorContainer.transform);
            o.transform.localPosition = new(currentX, 0);
            LifesInd.Add(o);
            currentX += 25;
        }
    }

    private void Update()
    {
        if (attached)
        {
            Vector3 bossPos = Camera.main.WorldToScreenPoint(attached.transform.position);
            EnemyIndicator.position = new(bossPos.x, EnemyIndicator.position.y);
            UpdateHealthBar();
        }
        UpdateSpellTimer(SpellcardManager._instance.GetCurrentSpell());
    }

    void UpdateHealthBar()
    {
        for (int i = 0; i < healthBars.Count; ++i)
        {
            healthBars[i].getHealthSlider.maxValue = attached.getMaxHealth[i];
            healthBars[i].getHealthSlider.value = attached.getHealth[i];
        }
    }

    public void UpdateSpellTimer(SpellcardBase spell)
    {
        if (spell)
        {
            int spellTime = (int)Mathf.Min(spell.GetSpellTime, 99);
            SpellTimer.text = String.Format("{0:D2}", spellTime);
        }
        else
        {
            SpellTimer.text = "N/A";
        }
    }

    public void SetHUDEmpty()
    {
        EnemyIndicator.gameObject.SetActive(false);
        SpellTimer.text = string.Empty;
        attached = null;
        SpellNameText.text = BossName.text = string.Empty;
        SpellNameBar.SetActive(false);
        for (int i = 0; i < healthBars.Count; ++i) Destroy(healthBars[i].gameObject);
        healthBars.Clear();
    }
}
