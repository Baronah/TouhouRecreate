using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputManager
{
    private static KeyCode defaultShootKey = KeyCode.Z,
                           defaultBoomKey = KeyCode.X,
                           defaultFocusKey = KeyCode.LeftShift;

    public static KeyCode ShootKey = KeyCode.Z;
    public static KeyCode BoomKey = KeyCode.X;
    public static KeyCode FocusKey = KeyCode.LeftShift;

    public static Vector2 GetMovementInput()
    {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
    }

    public static void GetFromSaves()
    {
        ShootKey = Enum.Parse<KeyCode>(PlayerPrefs.GetString("ShootKey", defaultShootKey.ToString()));
        BoomKey = Enum.Parse<KeyCode>(PlayerPrefs.GetString("BoomKey", defaultBoomKey.ToString()));
        FocusKey = Enum.Parse<KeyCode>(PlayerPrefs.GetString("FocusKey", defaultFocusKey.ToString()));
    }
}
