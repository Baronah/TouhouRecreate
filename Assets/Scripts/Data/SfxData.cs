using UnityEngine;

[CreateAssetMenu(fileName = "SfxData", menuName = "ScriptableObjects/SfxData", order = 2)]
public class SfxData : ScriptableObject
{
    public AudioClip[] sfxs;
    public enum SFXType
    {
        BOOMM,
        BOSS_DEFEAT,
        BREAK,
        BRIGHT,
        CANCLE,
        CHARGE_1,
        CHARGE_2,
        CHARGE_3,
        CHARGE_4,
        ELEC_BLAST,
        ENEMY_VANISH,
        FAMILIAR,
        FROG,
        LASER_1,
        LASER_2,
        LASER_3,
        LASER_4,
        LASER_5,
        MASTERSPRAK,
        OK,
        PAUSE,
        REVERSE,
        SELECT,
        SHATTER_1,
        SHATTER_2,
        SHOOT_1,
        SHOOT_2,
        SHOOT_3,
        SHOOT_4,
        SHOOT_5,
        SHOOT_6,
        SHOOT_7,
        SHOOT_8,
        SLASH_1,
        SLASH_2,
        SPELLCARD,
        SPELL_FAIL,
        SPELL_CAPTURE,
        PLAYER_EXPLODE,
        PLAYER_SHOOT,
        PLAYER_GRAZE,
    };
}