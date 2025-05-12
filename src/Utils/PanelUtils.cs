using System.Threading.Tasks;
using Clothes.Entities;
using Life;
using Life.Network;
using ModKit.Helper;
using mk = ModKit.Helper.TextFormattingHelper;

namespace Clothes.Panels
{
    public static class PanelUtils
    {
        public static string pluginName = "Clothes";

        public static int crossIcon = 196;
        public static int unknowIcon = 966;
        public static int blankIcon = 1153;
        public static int femaleTopIcon = 1110;
        public static int maleTopIcon = 1111;

        /// <summary>
        /// Formats and returns a title panel string with a specified title and subtitle.
        /// </summary>
        /// <param name="title">The main title of the panel.</param>
        /// <param name="subtitle">The subtitle of the panel.</param>
        /// <returns>A formatted string representing the title panel.</returns>
        public static string SetTitlePanel(string title = null, string subtitle = null)
        {
            string primaryLine = mk.Color($"{mk.Size($"{pluginName} - {title}", 12)}", mk.Colors.Info);
            if (title != null) return subtitle != null ? $"{primaryLine}<br>{subtitle}" : primaryLine;
            return pluginName;
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

        /// <summary>
        /// Generates a formatted tab line for a quantity tag, indicating the quantity or if it is empty.
        /// </summary>
        /// <param name="count">The quantity count to generate the tab line for.</param>
        /// <returns>A string representing the formatted tab line.</returns>
        public static string GetQuantityTagTabLine(int count)
        {
            return mk.Color($"[{(count > 0 ? $"Quantité: {count}" : "Vide")}]", mk.Colors.Info);
        }

        /// <summary>
        /// Retrieves the icon ID for a given item ID.
        /// </summary>
        /// <param name="itemId">The ID of the item to retrieve the icon for.</param>
        /// <returns>The icon ID if the item is found, otherwise returns the crossIcon.</returns>
        public static int GetItemIconId(int itemId)
        {
            var item = Nova.man.item.GetItem(itemId);
            return item != null ? Nova.man.newIcons.IndexOf(item.Icon) : crossIcon;
        }

    }
}
