using UnityEngine;

public static class ColourHelper
{
    // Preset Colors (customizable)
    public static Color White => new Color(1f, 1f, 1f, 1f);
    public static Color Black => new Color(0f, 0f, 0f, 1f);
    public static Color Red => new Color(1f, 0f, 0f, 1f);
    public static Color Green => new Color(0f, 1f, 0f, 1f);
    public static Color Blue => new Color(0f, 0f, 1f, 1f);

    // Example of custom shades
    public static Color SoftWhite => new Color(0.9f, 0.9f, 0.9f, 1f);
    public static Color DarkGreen => new Color(0f, 0.4f, 0f, 1f);

    // Example for your green (0,100,0)
    public static Color DeepGreen => new Color(0f, 100f / 255f, 0f, 1f);

    // Generic getter if you want dynamic configuration later
    public static Color From255(float r, float g, float b, float a = 255f)
        => new Color(r / 255f, g / 255f, b / 255f, a / 255f);
}