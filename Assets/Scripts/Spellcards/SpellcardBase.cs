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
    [SerializeField] [Range(0, 10f)] protected float DamageIntakeRatio = 1f;
    [SerializeField] protected string Name;

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

    [HideInInspector] public bool IsShooting = false;

    protected virtual void Start()
    {
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
        StartCoroutine(SpellcardPrepare());
        IsShooting = true;
    }

    IEnumerator SpellcardPrepare()
    {
        yield return new WaitForSeconds(1f);
        IsPrepared = true;
        BossHealthBar._instance.SetSpellName(GetData());
        StartCoroutine(SpellcardShoot());
    }

    protected abstract IEnumerator SpellcardShoot();

    protected virtual IEnumerator InitializeSpellCardPreEffect()
    {
        SpellcardCutEffect._instance.SetSpriteAndFadeIn(spellOwner.getIcon);
        while (SpellcardCutEffect._instance.IsPlaying)
        {
            spellOwner.MakeInvulnerable(Time.deltaTime + 0.1f);
            yield return null;
        }
    }

    public void OnSpellCardFinish()
    {
        StopAllCoroutines();
        ProjectileManager._instance.ClearShootsOfType(bulletsUse);
        this.enabled = false;
        IsShooting = false;
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
