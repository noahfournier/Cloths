using mk = ModKit.Helper.TextFormattingHelper;

namespace Clothes.Panels
{
    public static class PanelUtils
    {
        /// <summary>
        /// Formats and returns a title panel string with a specified title and subtitle.
        /// </summary>
        /// <param name="title">The main title of the panel.</param>
        /// <param name="subtitle">The subtitle of the panel.</param>
        /// <returns>A formatted string representing the title panel.</returns>
        public static string SetTitlePanel(string title, string subtitle)
        {
            return $"{mk.Color($"{mk.Size($"Cloths - {title}", 12)}", mk.Colors.Info)}<br>{subtitle}";
        }
    }
}
