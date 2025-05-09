using System.Threading.Tasks;
using Life.Network;
using Life.UI;
using ModKit.Helper;
using static Life.InventorySystem.Item;
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

        /// <summary>
        /// Notifies the player that a value is not editable and refreshes the panel.
        /// </summary>
        /// <param name="player">The player to notify.</param>
        /// <param name="panel">The panel to refresh.</param>
        public static void NotEditableValue(Player player, Panel panel)
        {
            player.Notify("Clothes", "Cette valeur n'est pas modifiable", Life.NotificationManager.Type.Info);
            panel.Refresh();
        }

        /// <summary>
        /// Handles the response of an update query and notifies the player of the result.
        /// </summary>
        /// <param name="player">The player to notify.</param>
        /// <param name="task">The task representing the update operation.</param>
        /// <returns>A boolean indicating whether the update was successful.</returns>
        public async static Task<bool> QueryUpdateResponse(Player player, Task<bool> task)
        {
            if (await task)
            {
                player.Notify("Clothes", "Modification enregistrée avec succès", Life.NotificationManager.Type.Success);
                return true;
            }
            else
            {
                player.Notify("Clothes", "Erreur lors de la sauvegarde de votre modification", Life.NotificationManager.Type.Error);
                return false;
            }
        }

    }
}
