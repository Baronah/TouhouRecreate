using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.PlayerLoop;
using static BulletData;
using static SpellcardBase;

public class EnemyBase : MonoBehaviour
{
    public static WaitForSeconds _waitForSeconds0_1 = new WaitForSeconds(0.1f);

    protected SpellcardBase[][] spellcards;
    protected SpellData[][] spellDatas;
    protected int currentLifeIndex = 0, currentSpellcardIndex = 0;
    public SpellcardBase currentActiveSpellcard => spellcards[currentLifeIndex][currentSpellcardIndex];
    public SpellData[] getSpellDatas => spellDatas[currentLifeIndex];

    protected float[][] mHealth;
    protected float[][] health;
    
    public float[] getMaxHealth => mHealth[currentLifeIndex];
    public float[] getHealth => health[currentLifeIndex];
    public float getCurrentSpellHealth => health[currentLifeIndex][currentSpellcardIndex];
    public void SetCurrentSpellHealth(float value) => health[currentLifeIndex][currentSpellcardIndex] = value;

    public bool IsAlive => health.Any(h => h.Any(hh => hh > 0));

    public int getRemainingLives => mHealth.Length - 1 - currentLifeIndex;

    public float GetSumHealth() => mHealth[currentLifeIndex].Sum();

    [SerializeField] protected string Name;
    public string getName => Name;

    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected SpriteRenderer glowSprite;
    [SerializeField] protected Collider2D hitbox;
    protected virtual void Start() 
    { 
        if (!spriteRenderer) spriteRenderer = transform.Find("Sprite").GetComponent<SpriteRenderer>();
        Icon = spriteRenderer.sprite;

        if (!glowSprite) glowSprite = transform.Find("GlowSprite").GetComponent<SpriteRenderer>();

        if (!hitbox) hitbox = GetComponent<Collider2D>();

        InitializeArrays();
        GetSpellcardsData();
    }

    public virtual void Update()
    {

    }

    public virtual void DisableHitbox()
    {
        spriteRenderer.color = new Color(1,1,1,0.25f);
        hitbox.enabled = false;
    }

    public virtual void EnableHitbox()
    {
        spriteRenderer.color = Color.white;
        hitbox.enabled = true;
    }

    Sprite Icon;
    public Sprite getIcon => Icon;

    [SerializeField] int[] spellsSplit;
    public int[] getSpellSplits => spellsSplit;

    public bool IsUsingSpellcard = false;

    private void InitializeArrays()
    {
        int lifeCount = spellsSplit.Length;

        spellcards = new SpellcardBase[lifeCount][];
        spellDatas = new SpellData[lifeCount][];
        mHealth = new float[lifeCount][];
        health = new float[lifeCount][];

        for (int i = 0; i < lifeCount; i++)
        {
            spellcards[i] = new SpellcardBase[spellsSplit[i]];
            spellDatas[i] = new SpellData[spellsSplit[i]];
            mHealth[i] = new float[spellsSplit[i]];
            health[i] = new float[spellsSplit[i]];
        }
    }

    protected void GetSpellcardsData()
    {
        SpellcardBase[] allSpells = GetComponentsInChildren<SpellcardBase>();
        SpellData[] allSpellDatas = allSpells.Select(s => s.GetData()).ToArray();

        int index = 0;
        for (int i = 0; i < spellsSplit.Length; ++i)
        {
            for (int j = 0; j < spellsSplit[i]; ++j)
            {
                spellcards[i][j] = allSpells[index];
                spellDatas[i][j] = allSpellDatas[index];
                mHealth[i][j] = health[i][j] = allSpellDatas[index].Health;

                index++;
            }
        }
    }

    public virtual void OnProjectileHit(DefaultProjectile projectile)
    {
        if (IsInvulnerbale) return;

        float damage = currentActiveSpellcard.GetSpellCardDamage(projectile.GetDamage);
        health[currentLifeIndex][currentSpellcardIndex] -= damage;
        if (damage > 0) OnDamageTake();
        if (health[currentLifeIndex][currentSpellcardIndex] <= 0) currentActiveSpellcard.OnAttackFinish();
    }

    void OnDamageTake()
    {
        if (PulseSpriteCoroutine == null)
            PulseSpriteCoroutine = StartCoroutine(PulseSprite());
    }

    Coroutine PulseSpriteCoroutine = null;
    Color pulseColor = new(1, 1, 1, 0.1f),
          normalColor = Color.clear;
    IEnumerator PulseSprite()
    {
        glowSprite.color = pulseColor;
        yield return _waitForSeconds0_1;
        glowSprite.color = normalColor;

        PulseSpriteCoroutine = null;
    }

    protected virtual void OnDeath()
    {
        gameObject.SetActive(false);
        ProjectileManager._instance.ClearShootsOfType(GetAllBulletTypes());
    }


    float InvulnerableTimer = 0;
    public bool IsInvulnerbale => InvulnerableTimer > 0;
    public void MakeInvulnerable(float duration)
    {
        InvulnerableTimer = Mathf.Max(InvulnerableTimer, duration);
    }

    private void FixedUpdate()
    {
        if (InvulnerableTimer > 0) InvulnerableTimer -= Time.fixedDeltaTime;
    }

    public BulletType[] GetBulletTypesOfSpell(int index)
    {
        return getSpellDatas[index].BulletTypes.Distinct().ToArray();
    }

    public BulletType[] GetAllBulletTypes()
    {
        return spellDatas[currentLifeIndex].SelectMany(s => s.BulletTypes).Distinct().ToArray();
    }
}
