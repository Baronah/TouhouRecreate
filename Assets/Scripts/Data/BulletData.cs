using UnityEngine;

[CreateAssetMenu(fileName = "BulletData", menuName = "ScriptableObjects/BulletData", order = 1)]
public class BulletData : ScriptableObject
{
    public static BulletData _instance;

    private void Awake()
    {
        _instance = this;
    }

    public enum BulletType
    {
        STAR_GREY = 0,
        STAR_RED = 1,
        STAR_MAGENTA = 2,
        STAR_BLUE = 3,
        STAR_CYAN = 4,
        STAR_GREEN = 5,
        STAR_YELLOW = 6,
        STAR_ORANGE = 7,

        CIRCLE_RED = 8,
        CIRCLE_BLUE = 9,
        CIRCLE_GREEN = 10,
        CIRCLE_YELLOW = 11,

        TRIANGLE_GREY = 12,
        TRIANGLE_RED = 13,
        TRIANGLE_RED_BOLD = 14,
        TRIANGLE_MAGENTA = 15,
        TRIANGLE_MAGENTA_BOLD = 16,
        TRIANGLE_BLUE = 17,
        TRIANGLE_BLUE_BOLD = 18,
        TRIANGLE_CYAN = 19,
        TRIANGLE_CYAN_BOLD = 20,
        TRIANGLE_GREEN = 21,
        TRIANGLE_GREEN_BOLD = 22,
        TRIANGLE_GREEN_YELLOW = 23,
        TRIANGLE_YELLOW = 24,
        TRIANGLE_YELLOW_BOLD = 25,
        TRIANGLE_ORANGE = 26,
        TRIANGLE_GREY_BOLD = 27,

        RICE_GREY = 28,
        RICE_DARK_RED = 29,
        RICE_RED = 30,
        RICE_PURPLE = 31,
        RICE_MAGENTA = 32,
        RICE_DARK_BLUE = 33,
        RICE_BLUE = 34,
        RICE_DARK_CYAN = 35,
        RICE_CYAN = 36,
        RICE_DARK_GREEN = 37,
        RICE_GREEN = 38,
        RICE_LIGHT_GREEN = 39,
        RICE_DARK_YELLOW = 40,
        RICE_YELLOW = 41,
        RICE_ORANGE = 42,
        RICE_WHITE = 43,

        MOCHI_GREY = 44,
        MOCHI_SKIN = 45,
        MOCHI_PINK = 46,
        MOCHI_VIOLET = 47,
        MOCHI_CYAN = 48,
        MOCHI_GREEN = 49,
        MOCHI_YELLOW = 50,
        MOCHI_LIGHT_ORANGE = 51,

        KUNAI_GREY = 52,
        KUNAI_DARK_RED = 53,
        KUNAI_RED = 54,
        KUNAI_PURPLE = 55,
        KUNAI_MAGENTA = 56,
        KUNAI_DARK_BLUE = 57,
        KUNAI_BLUE = 58,
        KUNAI_LIGHT_BLUE = 59,
        KUNAI_CYAN = 60,
        KUNAI_DARK_GREEN = 61,
        KUNAI_GREEN = 62,
        KUNAI_LIGHT_GREEN = 63,
        KUNAI_YELLOW = 64,
        KUNAI_LIGHT_YELLOW = 65,
        KUNAI_ORANGE = 66,
        KUNAI_WHITE = 67,

        CIRCLE_PINK = 68,
        CIRCLE_MINT = 69,
        CIRCLE_ORANGE = 70,
        CIRCLE_GREY = 71,

        SHARD_GREY = 72,
        SHARD_RED = 73,
        SHARD_RED_BOLD = 74,
        SHARD_MAGENTA = 75,
        SHARD_PINK = 76,
        SHARD_LIGHT_BLUE = 77,
        SHARD_BLUE = 78,
        SHARD_LIGHT_CYAN = 79,
        SHARD_CYAN = 80,
        SHARD_LIGHT_GREEN = 81,
        SHARD_GREEN = 82,
        SHARD_LIGHT_YELLOW = 83,
        SHARD_YELLOW = 84,
        SHARD_YELLOW_BOLD = 85,
        SHARD_ORANGE = 86,
        SHARD_WHITE = 87,

        STANDARD_SHOOT_GREY = 369,
        STANDARD_SHOOT_DARKRED = 370,
        STANDARD_SHOOT_RED = 371,
        STANDARD_SHOOT_PURPLE = 372,
        STANDARD_SHOOT_MAGENTA = 373,
        STANDARD_SHOOT_DARKBLUE = 374,
        STANDARD_SHOOT_BLUE = 375,
        STANDARD_SHOOT_LIGHTBLUE = 376,
        STANDARD_SHOOT_CYAN = 377,
        STANDARD_SHOOT_DARKGREEN = 378,
        STANDARD_SHOOT_GREEN = 379,
        STANDARD_SHOOT_LIGHTGREEN = 380,
        STANDARD_SHOOT_LIGHTYELLOW = 381,
    }

    public Sprite[] Bullets;
}