using System;
using System.Collections.Generic;
using System.Linq;
using Cloth.Entities;
using Cloth.Utils;
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
            List<ClothRecord> allClothRecords = await CharacterInventories.GetInventoryForCharacterAsync(player.character.Id);

            Panel panel = Context.PanelHelper.Create(PanelUtils.SetTitlePanel("Sac à dos", $"[emplacements: {allClothRecords.Count} / 10]"), UIPanel.PanelType.TabPrice, player, () => BackpackMenuPanel(player));

            foreach (ClothType clothType in Enum.GetValues(typeof(ClothType)))
            {
                List<ClothRecord> clothRecords = allClothRecords.Where(i => i.ClothModels.ClothType == (int)clothType).ToList();
                ClothRecord equippedClothRecord = clothRecords.Where(i => i.CharacterInventories.IsEquipped).FirstOrDefault();

                panel.AddTabLine($"[{(clothRecords.Count > 0 ? $"Quantité: {clothRecords.Count}" : "Vide")}] {clothType}", _ =>
                {
                    if(clothRecords.Count > 0) BackpackClothTypePanel(player, clothType, clothRecords, equippedClothRecord);
                    else
                    {
                        player.Notify("Cloths", $"Vous n'avez aucun {clothType} dans votre sac à dos", Life.NotificationManager.Type.Info);
                        panel.Refresh();
                    }
                });
            }

            panel.PreviousButton();
            panel.NextButton("Sélectionner", () => panel.SelectTab());
            panel.CloseButton();

            panel.Display();
        }

        public void BackpackClothTypePanel(Player player, ClothType clothType, List<ClothRecord> clothRecords, ClothRecord equippedClothRecord)
        {
            Panel panel = Context.PanelHelper.Create(PanelUtils.SetTitlePanel("Sac à dos", $"Vos {clothType}"), UIPanel.PanelType.TabPrice, player, () => BackpackClothTypePanel(player, clothType, clothRecords, equippedClothRecord));

            foreach (var clothRecord in clothRecords)
            {
                panel.AddTabLine($"{(clothRecord.CharacterInventories.IsEquipped ? "[Équipé] " :"")}{clothRecord.ClothModels.Name}", _ =>
                {
                    if(!clothRecord.CharacterInventories.IsEquipped) ClothUtils.EquipClothing(player, clothRecord, equippedClothRecord);
                    else
                    {
                        player.Notify("Cloth", "Vous portez déjà ce vêtement", Life.NotificationManager.Type.Warning);
                        panel.Refresh();
                    }
                });
            }

            panel.NextButton("Sélectionner", () => panel.SelectTab());
            // Faire un bouton déséquipe le vêtement sans rien mettre à la place (ex: torse nu)
            panel.PreviousButton();        
            panel.CloseButton();

            panel.Display();
        }
    }
}
