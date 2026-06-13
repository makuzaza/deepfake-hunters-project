// PuppeteerTheme.cs  -  Assets/_Project/Scripts/Core
// The Style-Guide palette as the single source of colour truth. Flat fills only;
// shading is a single contrast tint (no gradients, no dithering, no outlines).
using UnityEngine;

public static class PuppeteerTheme
{
    static Color Hex(string hex)
    {
        ColorUtility.TryParseHtmlString(hex.StartsWith("#") ? hex : "#" + hex, out var c);
        c.a = 1f; return c;
    }

    // --- Raw palette (12) ---
    public static readonly Color Cream  = Hex("#eae0cc");
    public static readonly Color Sage   = Hex("#819794");
    public static readonly Color Teal   = Hex("#485d5c");
    public static readonly Color Rose   = Hex("#ceb2af");
    public static readonly Color Green  = Hex("#98a686");
    public static readonly Color Red    = Hex("#8e3943");
    public static readonly Color Tan    = Hex("#caae87");
    public static readonly Color Olive  = Hex("#687861");
    public static readonly Color Maroon = Hex("#5e4149");
    public static readonly Color Clay   = Hex("#bd8a86");
    public static readonly Color Mauve  = Hex("#946f70");
    public static readonly Color Ink    = Hex("#162329");

    // --- Semantic roles ---
    public static readonly Color Background = Ink;
    public static readonly Color PanelDark  = Teal;
    public static readonly Color PanelMid   = Maroon;
    public static readonly Color PanelSoft  = Olive;
    public static readonly Color TextLight  = Cream;
    public static readonly Color TextMuted  = Sage;
    public static readonly Color Accent     = Tan;
    public static readonly Color Positive   = Green;
    public static readonly Color Danger     = Red;

    public static readonly string[] AllHex =
    {
        "#eae0cc","#819794","#485d5c","#ceb2af","#98a686","#8e3943",
        "#caae87","#687861","#5e4149","#bd8a86","#946f70","#162329"
    };
    public static Color FromHex(string hex) => Hex(hex);
}
