using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class SpellcardBase : MonoBehaviour
{
    protected EnemyBase spellOwner;

    protected static WaitForSeconds _waitForSeconds1_1 = new WaitForSeconds(1f);
    protected static WaitForSeconds _waitForSeconds10 = new WaitForSeconds(10f);

    [SerializeField] protected float Health = 500;
    
    [SerializeField] protected float SpellScore = 2_500_000;
    float MinScore;
    float ScoreDrainPerSec;

    [SerializeField] protected float SpellTime = 40f;
    public float GetSpellTime => SpellTime;

    [SerializeField] [Range(0, 10f)] protected float DamageIntakeRatio = 1f;
    
    // Only spellcard will display its name to the HUD
    [SerializeField] protected string Name;
    
    // Time it takes before the spell starts shooting
    [SerializeField] protected float PrepareTime = 1.5f;

    public struct SpellData
    {
        public SpellType SpellType;
        public string Name;
        public float Health; 
        public BulletData.BulletType[] BulletTypes;
        public SpellClearType SpellClearType;
    }

    public SpellData GetData()
    {
        return new SpellData()
        {
            SpellType = spellType,
            Health = this.Health,
            Name = this.Name,
            BulletTypes = bulletsUse.Distinct().ToArray(),
            SpellClearType = clearType
        };
    }

    public enum SpellType
    {
        NON_SPELL,
        SPELLCARD
    }

    public SpellType spellType;

    public enum SpellClearType
    {
        DEFEAT,
        SURVIVAL,
    }

    public SpellClearType clearType;

    [SerializeField] protected BulletData.BulletType[] bulletsUse;

    public Vector3 PlayerPosition => PlayerManager._instance.PlayerPosition;
    public Vector3 GetDirectionToPlayer(Vector3 currentPos)
    {
        return GetDirectionToPoint(currentPos, PlayerPosition);
    }

    public Vector3 GetDirectionToPoint(Vector3 currentPos, Vector3 toPos)
    {
        return (toPos - currentPos).normalized;
    }

    public enum AngleType
    {
        DEGREE,
        RADIAN,
    }
    public float GetAngleToPlayer(Vector3 currentPos, AngleType angleType)
    {
        return GetAngleToPoint(currentPos, PlayerPosition, angleType);
    }

    public float GetAngleToPoint(Vector3 from, Vector3 to, AngleType angleType)
    {
        Vector3 direction = GetDirectionToPoint(from, to);
        float rad = Mathf.Atan2(direction.y, direction.x);
        return angleType == AngleType.RADIAN ? rad : rad * Mathf.Rad2Deg;
    }

    [HideInInspector] public bool IsShooting = false;

    protected virtual void Start()
    {
        MinScore = SpellScore * 0.25f;
        ScoreDrainPerSec = (SpellScore - MinScore) / SpellTime;
        spellOwner = GetComponentInParent<EnemyBase>();
        ScreenCorners = new float[]
        {
            GameManager._instance.cornerLeftDown.position.x,
            GameManager._instance.cornerLeftDown.position.y,
            GameManager._instance.cornerRightUp.position.x,
            GameManager._instance.cornerRightUp.position.y,
        };
    }

    [HideInInspector] public bool IsPrepared = false;
    public void Shoot() 
    {
        StartCoroutine(AttackPrepare());
        IsShooting = true;
    }

    protected virtual IEnumerator AttackPrepare()
    {
        yield return new WaitForSeconds(PrepareTime);
        OnSpellPrepareFinished();

        yield return InitializeSpellCard();
        StartCoroutine(AttackShoot());
    }

    protected void OnSpellPrepareFinished()
    {
        IsPrepared = true;
        BossHealthBar._instance.SetSpellName(GetData());
        GameManager._instance.SetBackground(spellType);

        if (spellType == SpellType.SPELLCARD)
        {
            spellOwner.IsUsingSpellcard = true;
        }

        if (clearType == SpellClearType.SURVIVAL) spellOwner.DisableHitbox();
    }

    private void Update()
    {
        if (IsShooting && IsPrepared)
        {
            UpdateSpellScore();
            SpellTime -= Time.deltaTime;
            if (SpellTime < 0)
            {
                OnAttackFinish();
            }
        }
    }

    void UpdateSpellScore()
    {
        if (SpellCaptureIsInvalid) return;

        if (clearType != SpellClearType.SURVIVAL)
        {
            SpellScore -= ScoreDrainPerSec * Time.deltaTime;
        }
        
        SpellScore = Mathf.Max(SpellScore, MinScore);

        SpellcardManager._instance.SetSpellPoint((int)SpellScore);
    }

    protected abstract IEnumerator AttackShoot();

    protected virtual IEnumerator InitializeSpellCard()
    {
        SpellcardManager._instance.RegisterSpellcard(this, spellOwner.getIcon);
        while (SpellcardManager._instance.IsPlaying)
        {
            spellOwner.MakeInvulnerable(Time.deltaTime + 0.1f);
            yield return null;
        }
    }

    public virtual void OnAttackFinish()
    {
        StopAllCoroutines();
        ProjectileManager._instance.ClearShootsOfType(bulletsUse);
        spellOwner.SetCurrentSpellHealth(0f);

        if (spellType == SpellType.SPELLCARD)
        {
            SetSpellCardStatus();
            if (clearType == SpellClearType.SURVIVAL) spellOwner.EnableHitbox();
        }

        SpellTime = 0f;
        this.enabled = false;
        IsShooting = false;

        BossHealthBar._instance.ClearSpell();
        BossHealthBar._instance.UpdateSpellTimer(this);
        SoundManager._instance.PlaySound(SfxData.SFXType.ENEMY_VANISH, SoundManager.SfxChannel.EFFECT);
    }

    void SetSpellCardStatus()
    {
        bool isDefeated = spellOwner.getCurrentSpellHealth <= 0;
        bool isTimedOut = SpellTime <= 0 && !isDefeated;
        SpellcardManager.SpellcardFinishType finishType;
        if (SpellCaptureIsInvalid)
        {
            finishType = SpellcardManager.SpellcardFinishType.FAILED;
        }
        else if (isDefeated)
        {
            finishType = SpellcardManager.SpellcardFinishType.CAPTURED;
        }
        else if (isTimedOut)
        {
            finishType = clearType == SpellClearType.SURVIVAL 
                ? SpellcardManager.SpellcardFinishType.CAPTURED 
                : SpellcardManager.SpellcardFinishType.FAILED;
        }
        else
        {
            finishType = SpellcardManager.SpellcardFinishType.FAILED;
        }
        OnSpellCardClear(finishType);
    }

    bool SpellCaptureIsInvalid = false;
    public void PlayerCheatedThroughSpell()
    {
        SpellCaptureIsInvalid = true;
        SpellcardManager._instance.SetSpellFailed();
    }

    public void OnSpellCardClear(SpellcardManager.SpellcardFinishType spellcardFinishType)
    {
        spellOwner.IsUsingSpellcard = false;
        GameManager._instance.SetBackground(SpellType.NON_SPELL);
        if (spellcardFinishType == SpellcardManager.SpellcardFinishType.CAPTURED)
        {
            OnSpellCardCapture();
        }
        else if (spellcardFinishType == SpellcardManager.SpellcardFinishType.FAILED)
        {
            OnSpellCardFailed();
        }
    }

    public void OnSpellCardFailed()
    {
        SpellcardManager._instance.ShowSpellcardFailMessage();
        SpellcardManager._instance.OnSpellcardFinish(SpellcardManager.SpellcardFinishType.FAILED);
    }

    public void OnSpellCardCapture()
    {
        GameManager._instance.AddScore(SpellScore);
        SpellcardManager._instance.ShowSpellcardCaptureMessage((int) SpellScore);
        SpellcardManager._instance.OnSpellcardFinish(SpellcardManager.SpellcardFinishType.CAPTURED);
    }

    public float GetSpellCardDamage(float damageIn)
    {
        return IsPrepared ? damageIn * DamageIntakeRatio : 0f;
    }

    public DefaultProjectile CreateSimpleProjectile(BulletData.BulletType bulletType, float speed, Vector3 position, float scale = 1.0f)
    {
        return ProjectileManager._instance.CreateSimpleProjectile(
                bulletType,
                DefaultProjectile.TargetType.PLAYER,
                1f,
                speed,
                position,
                scale
            );
    }

    protected float[] ScreenCorners;
    protected bool hasShotOverbound(Vector3 shootPosition)
    {
        return
            shootPosition.x < ScreenCorners[0]
            ||
            shootPosition.y < ScreenCorners[1]
            ||
            shootPosition.x > ScreenCorners[2]
            ||
            shootPosition.y > ScreenCorners[3];
    }
}
