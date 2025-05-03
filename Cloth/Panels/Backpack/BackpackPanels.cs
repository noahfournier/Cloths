using System;
using System.Collections.Generic;
using Cloth.Entities;
using Life.InventorySystem;
using Life.Network;
using Life.UI;
using ModKit.Helper;

namespace Cloth.Panels.Backpack
{
    public class BackpackPanels
    {
        public ModKit.ModKit Context { get; set; }

        public BackpackPanels(ModKit.ModKit context)
        {
            Context = context;
        }

        public async void BackpackMenuPanel(Player player)
        {
            var inventoryItems = await CharacterInventories.GetInventoryForCharacterAsync(player.character.Id);

            Panel panel = Context.PanelHelper.Create(PanelUtils.SetTitlePanel("Sac à dos", $"[emplacements: {inventoryItems.Count} / 10]"), UIPanel.PanelType.TabPrice, player, () => BackpackMenuPanel(player));

            foreach (var clothType in Enum.GetValues(typeof(ClothType)))
            {
                panel.AddTabLine($"{clothType}", _ =>
                {
                    //code
                });
            }

            panel.AddButton("Retour", _ => AAMenu.AAMenu.menu.AdminPluginPanel(player));
            panel.NextButton("Sélectionner", () => panel.SelectTab());
            panel.CloseButton();

            panel.Display();
        }
    }
}
