using mk = ModKit.Helper.TextFormattingHelper;

namespace Cloth.Panels
{
    public static class PanelUtils
    {
        public static string SetTitlePanel(string title, string subtitle)
        {
            return $"{mk.Color($"{mk.Size($"Cloths - {title}", 12)}", mk.Colors.Info)}<br>{subtitle}";
        }
    }
}
