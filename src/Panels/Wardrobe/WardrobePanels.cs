using System;
using System.Collections.Generic;
using System.Linq;
using Clothes.Config;
using Clothes.Entities;
using Clothes.Entities.CompositeEntities;
using Clothes.Utils;
using Life.AreaSystem;
using Life.InventorySystem;
using Life.Network;
using Life.UI;
using ModKit.Helper;

namespace Clothes.Panels.Wardrobe
{
    public class WardrobePanels
    {
        public ModKit.ModKit Context { get; set; }

        public WardrobePanels(ModKit.ModKit context)
        {
            Context = context;
        }

        public async void WardrobeMenuPanel(Player player, LifeArea area)
        {
            string wardrobeSlots = ClothesConfig.Data.MaxWardrobeSlots > 0 ? ClothesConfig.Data.MaxWardrobeSlots.ToString() : "infini";
            List<ClothRecord> allClothRecords = await AreaInventories.GetInventoryForAreaAsync((int)area.areaId);

            Panel panel = Context.PanelHelper.Create(PanelUtils.SetTitlePanel("Garde-robe", $"[emplacements: {allClothRecords.Count} / {wardrobeSlots}]"), UIPanel.PanelType.TabPrice, player, () => WardrobeMenuPanel(player, area));

            foreach (ClothType clothType in Enum.GetValues(typeof(ClothType)))
            {
                List<ClothRecord> clothRecords = allClothRecords.Where(i => i.ClothModels.ClothType == (int)clothType).ToList();

                panel.AddTabLine($"[{(clothRecords.Count > 0 ? $"Quantité: {clothRecords.Count}" : "Vide")}] {clothType}", _ =>
                {
                    if (clothRecords.Count > 0) WardrobeToBackpack(player, clothType, clothRecords);
                    else
                    {
                        player.Notify("Cloths", $"Vous n'avez aucun {clothType} dans votre garde-robe", Life.NotificationManager.Type.Info);
                        panel.Refresh();
                    }
                });
            }

            panel.PreviousButton();
            panel.NextButton("Sélectionner", () => panel.SelectTab());
            panel.NextButton("Déposer", () => BackpackToWardrobePanel(player, area));
            panel.CloseButton();

            panel.Display();
        }

        public void WardrobeToBackpack(Player player, ClothType clothType, List<ClothRecord> clothRecords)
        {
            Panel panel = Context.PanelHelper.Create(PanelUtils.SetTitlePanel("Garde-robe", $"Vos {clothType}"), UIPanel.PanelType.TabPrice, player, () => WardrobeToBackpack(player, clothType, clothRecords));

            foreach (ClothRecord clothRecord in clothRecords)
            {
                panel.AddTabLine($"{PanelUtils.GetClothModelTabLine(clothRecord.ClothModels)}", async _ =>
                {
                    if (await ClothUtils.HasAvailableSlotsInBackpack(player))
                    {
                        if (await clothRecord.AreaInventories.Delete())
                        {
                            clothRecord.CharacterInventories = new CharacterInventories(player.character.Id, clothRecord.ClothItems.Id);
                            if (await clothRecord.CharacterInventories.Save())
                            {
                                clothRecords.Remove(clothRecord);
                                player.Notify("Clothes", "Vêtement déposé avec succès dans votre garde-robe", Life.NotificationManager.Type.Success);
                            }
                            else player.Notify("Clothes", "Erreur lors de l'ajout du vêtement à votre garde-robe", Life.NotificationManager.Type.Error);
                        }
                        else player.Notify("Clothes", "Erreur lors du retrait du vêtement de votre garde-robe", Life.NotificationManager.Type.Error);
                    }

                    panel.Refresh();
                });
            }

            panel.AddButton("Récupérer", _ => panel.SelectTab());           
            panel.PreviousButton();
            panel.CloseButton();

            panel.Display();
        }

        public async void BackpackToWardrobePanel(Player player, LifeArea area)
        {
            List<ClothRecord> clothRecords = await CharacterInventories.GetInventoryForCharacterAsync(player.character.Id);

            Panel panel = Context.PanelHelper.Create(PanelUtils.SetTitlePanel("Garde-robe", $"Sélectionner les vêtements à déposer depuis votre sac à dos"), UIPanel.PanelType.TabPrice, player, () => BackpackToWardrobePanel(player, area));

            foreach (ClothRecord clothRecord in clothRecords)
            {
                panel.AddTabLine($"{ClothUtils.ClothTypeTranslater((ClothType)clothRecord.ClothModels.ClothType)} - {PanelUtils.GetClothModelTabLine(clothRecord.ClothModels)}", async _ =>
                {
                    if(clothRecord.CharacterInventories.IsEquipped) ClothUtils.EquipClothing(player, clothRecord);

                    if(await clothRecord.CharacterInventories.Delete())
                    {
                        clothRecord.AreaInventories = new AreaInventories((int)area.areaId, clothRecord.ClothItems.Id);
                        if(await clothRecord.AreaInventories.Save())
                        {
                            clothRecords.Remove(clothRecord);
                            player.Notify("Clothes", "Vêtement déposé avec succès dans votre garde-robe", Life.NotificationManager.Type.Success);
                        } else player.Notify("Clothes", "Erreur lors de l'ajout du vêtement à votre garde-robe", Life.NotificationManager.Type.Error);
                    } else player.Notify("Clothes","Erreur lors du retrait du vêtement de votre sac à dos", Life.NotificationManager.Type.Error);

                    panel.Refresh();
                });
            }

            panel.NextButton("Sélectionner", () => panel.SelectTab());
            panel.PreviousButton();

            panel.Display();
        }
    }
}
