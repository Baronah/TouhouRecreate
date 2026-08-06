using UnityEngine;

public static class ColorUtil
{
    public static Color clearWhite = new Color(1, 1, 1, 0);

    public static Color GetClearColorOf(Color color)
        => new(color.r, color.g, color.b, 0);
}