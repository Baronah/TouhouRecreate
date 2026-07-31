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
        STAR_GREY,
        STAR_RED,
        STAR_MAGENTA,
        STAR_BLUE,
        STAR_CYAN,
        STAR_GREEN,
        STAR_YELLOW,
        STAR_ORANGE,
        CIRCLE_RED,
        CIRCLE_BLUE,
        CIRCLE_GREEN,
        CIRCLE_YELLOW,
        TRIANGLE_GREY,
        TRIANGLE_RED,
        TRIANGLE_RED_BOLD,
        TRIANGLE_MAGENTA,
        TRIANGLE_MAGENTA_BOLD,
        TRIANGLE_BLUE,
        TRIANGLE_BLUE_BOLD,
        TRIANGLE_CYAN,
        TRIANGLE_CYAN_BOLD,
        TRIANGLE_GREEN,
        TRIANGLE_GREEN_BOLD,
        TRIANGLE_GREEN_YELLOW,
        TRIANGLE_YELLOW,
        TRIANGLE_YELLOW_BOLD,
        TRIANGLE_ORANGE,
        TRIANGLE_GREY_BOLD,
        RICE_GREY,
        RICE_DARK_RED,
        RICE_RED,
        RICE_PURPLE,
        RICE_MAGENTA,
        RICE_DARK_BLUE,
        RICE_BLUE,
        RICE_DARK_CYAN,
        RICE_CYAN,
        RICE_DARK_GREEN,
        RICE_GREEN,
        RICE_LIGHT_GREEN,
        RICE_DARK_YELLOW,
        RICE_YELLOW,
        RICE_ORANGE,
        RICE_WHITE,
        MOCHI_GREY,
        MOCHI_SKIN,
        MOCHI_PINK,
        MOCHI_VIOLET,
        MOCHI_CYAN,
        MOCHI_GREEN,
        MOCHI_YELLOW,
        MOCHI_LIGHT_ORANGE,
        KUNAI_GREY,
        KUNAI_DARK_RED,
        KUNAI_RED,
        KUNAI_PURPLE,
        KUNAI_MAGENTA,
        KUNAI_DARK_BLUE,
        KUNAI_BLUE,
        KUNAI_LIGHT_BLUE,
        KUNAI_CYAN,
        KUNAI_DARK_GREEN,
        KUNAI_GREEN,
        KUNAI_LIGHT_GREEN,
        KUNAI_YELLOW,
        KUNAI_LIGHT_YELLOW,
        KUNAI_ORANGE,
        KUNAI_WHITE,
    }

    public Sprite[] Bullets;
}