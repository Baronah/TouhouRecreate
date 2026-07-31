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
    }

    public void Shoot() 
    {
        BossHealthBar._instance.SetSpellName(GetData());
        StartCoroutine(SpellcardShoot());
        IsShooting = true;
    }

    protected abstract IEnumerator SpellcardShoot();

    protected virtual IEnumerator InitializeSpellCardPreEffect()
    {
        var effect = GameManager._instance.InitializeSpellCardEffect(spellOwner.getIcon);
        while (effect.IsPlaying)
        {
            spellOwner.MakeInvulnerable(Time.deltaTime + 0.1f);
            yield return null;
        }
    }

    public void OnSpellCardFinish()
    {
        StopAllCoroutines();
        this.enabled = false;
        IsShooting = false;
        ProjectileManager._instance.ClearShootsOfType(bulletsUse);
    }

    public float GetSpellCardDamage(float damageIn)
    {
        return damageIn * DamageIntakeRatio;
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
}
