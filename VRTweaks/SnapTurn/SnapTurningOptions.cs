﻿using UnityEngine;

public static class SnapTurningOptions
{
    public static bool EnableSnapTurning = true;
    public static int SnapAngleChoiceIndex = 1;
    public static string[] SnapAngleChoices = { "22.5", "45", "90" };
    public static float[] SnapAngles = { 22.5f, 45f, 90f };
    public static KeyCode KeybindKeyLeft = KeyCode.LeftArrow; //TODO: make these bindable in options menu
    public static KeyCode KeybindKeyRight = KeyCode.RightArrow;
}
