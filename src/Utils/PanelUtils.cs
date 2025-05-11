using System.Threading.Tasks;
using Clothes.Entities;
using Life.Network;
using ModKit.Helper;
using mk = ModKit.Helper.TextFormattingHelper;

namespace Clothes.Panels
{
    public static class PanelUtils
    {
        public static int crossIcon = 196;
        public static int unknowIcon = 966;
        public static int blankIcon = 1153;
        public static int femaleTop = 1110;
        public static int maleTop = 1111;

        /// <summary>
        /// Formats and returns a title panel string with a specified title and subtitle.
        /// </summary>
        /// <param name="title">The main title of the panel.</param>
        /// <param name="subtitle">The subtitle of the panel.</param>
        /// <returns>A formatted string representing the title panel.</returns>
        public static string SetTitlePanel(string title, string subtitle)
        {
            return $"{mk.Color($"{mk.Size($"Clothes - {title}", 12)}", mk.Colors.Info)}<br>{subtitle}";
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

        /// <summary>
        /// Handles the response of a creation query and notifies the player of the result.
        /// </summary>
        /// <param name="player">The player to notify.</param>
        /// <param name="task">The task representing the creation operation.</param>
        /// <returns>A boolean indicating whether the creation was successful.</returns>
        public async static Task<bool> QueryCreateResponse(Player player, Task<bool> task)
        {
            if (await task)
            {
                player.Notify("Clothes", "Création enregistrée avec succès", Life.NotificationManager.Type.Success);
                return true;
            }
            else
            {
                player.Notify("Clothes", "Erreur lors de la sauvegarde de votre création", Life.NotificationManager.Type.Error);
                return false;
            }
        }

        /// <summary>
        /// Generates a formatted tab line for a cloth model, including a sex tag and the model name.
        /// </summary>
        /// <param name="clothModel">The cloth model to generate the tab line for.</param>
        /// <returns>A string representing the formatted tab line.</returns>
        public static string GetClothModelTabLine(ClothModels clothModel)
        {
            string sexTag = clothModel.SexId == 0 ? mk.Color("[H]", mk.Colors.Info) : mk.Color("[F]", mk.Colors.Purple);
            return sexTag + " " + clothModel.Name;
        }


        /// <summary>
        /// Generates a formatted tab line for a cloth model, including the price and sale status.
        /// </summary>
        /// <param name="clothModel">The cloth model to generate the tab line for.</param>
        /// <param name="isForSale">A boolean indicating whether the cloth model is for sale.</param>
        /// <returns>A string representing the formatted tab line.</returns>
        public static string GetClothModelPriceTabLine(ClothModels clothModel, bool isForSale)
        {
            string price = mk.Color($"{clothModel.Price}€", mk.Colors.Warning);
            string status = mk.Color($"{(isForSale ? "En boutique" : "En retrait")}", isForSale ? mk.Colors.Success : mk.Colors.Error);
            return $"{mk.Align($"{price}<br>{status}", mk.Aligns.Center)}";
        }

    }
}
