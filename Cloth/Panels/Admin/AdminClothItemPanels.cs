using System;
using System.Collections.Generic;
using System.Linq;
using Cloth.Entities;
using Cloth.Utils;
using Life.InventorySystem;
using Life.Network;
using Life.UI;
using ModKit.Helper;
using ModKit.Utils;
using mk = ModKit.Helper.TextFormattingHelper;

namespace Cloth.Panels.Admin
{
    public class AdminClothItemPanels
    {
        public ModKit.ModKit Context { get; set; }

        public AdminClothItemPanels(ModKit.ModKit context)
        {
            Context = context;
        }

        public void SelectClothTypePanel(Player player)
        {
            Panel panel = Context.PanelHelper.Create(PanelUtils.SetTitlePanel("Générer un vêtement", "Choisir le type selon le sexe"), UIPanel.PanelType.TabPrice, player, () => SelectClothTypePanel(player));

            foreach (var clothType in Enum.GetValues(typeof(ClothType)))
            {
                List<ClothModels> maleModels = Cloth.CacheManager.ClothModelsCache.Cache.Where(m => m.Value.SexId == 0 && m.Value.ClothType == (int)clothType).Select(m => m.Value).ToList();
                if(maleModels.Count > 0) panel.AddTabLine($"{mk.Color("[H]", mk.Colors.Info)} {clothType}", _ => SelectClothModelPanel(player, maleModels));

                List<ClothModels> femaleModels = Cloth.CacheManager.ClothModelsCache.Cache.Where(m => m.Value.SexId == 1 && m.Value.ClothType == (int)clothType).Select(m => m.Value).ToList();
                if (femaleModels.Count > 0) panel.AddTabLine($"{mk.Color("[F]", mk.Colors.Purple)} {clothType}", _ => SelectClothModelPanel(player, femaleModels));
            }

            panel.PreviousButton();
            panel.NextButton("Sélectionner", () => panel.SelectTab());
            panel.CloseButton();

            panel.Display();
        }

        private void SelectClothModelPanel(Player player, List<ClothModels> models)
        {
            Panel panel = Context.PanelHelper.Create(PanelUtils.SetTitlePanel("Générer un vêtement", "Choisir le modèle du vêtement"), UIPanel.PanelType.TabPrice, player, () => SelectClothModelPanel(player, models));

            foreach (ClothModels model in models)
            {
                panel.AddTabLine($"{model.Name}", _ => { });
            }

            panel.NextButton("Sélectionner", () => SetCharacterPanel(player, models[panel.selectedTab]));
            panel.AddButton("Prévisualiser", _ =>
            {
                ClothUtils.PreviewClothing(player, models[panel.selectedTab]);
                panel.Refresh();
            });
            panel.PreviousButton();
            panel.CloseButton();

            panel.Display();
        }

        private void SetCharacterPanel(Player player, ClothModels model)
        {
            var nearbyPlayers = EnvironmentUtils.DetectNearbyPlayers(player);

            Panel panel = Context.PanelHelper.Create(PanelUtils.SetTitlePanel("Générer un vêtement", "Définir le destinataire du vêtement"), UIPanel.PanelType.TabPrice, player, () => SetCharacterPanel(player, model));

            foreach(var p in nearbyPlayers)
            {
                panel.AddTabLine($"{p.Value.FullName}", async _ =>
                {
                    ClothItems clothItem = new ClothItems();
                    clothItem.ClothModelId = model.Id;
                    clothItem.IsDirty = false;
                    clothItem.CreatedAt = DateUtils.GetCurrentTime();
                    if (await clothItem.Save())
                    {
                        CharacterInventories characterInventories = new CharacterInventories();
                        characterInventories.CharacterId = p.Key;
                        characterInventories.ClothItemId = clothItem.Id;
                        characterInventories.IsEquipped = false;

                        if (await characterInventories.Save())
                        {
                            player.Notify("Cloths", $"Succès lors de la livraison du vêtement \"{model.Name}\" à {p.Value.FullName}", Life.NotificationManager.Type.Success);
                            p.Value.Notify("Cloths", $"Vous venez de reçevoir le vêtement \"{model.Name}\" dans votre sac à dos", Life.NotificationManager.Type.Success);
                            panel.Close();
                            return;
                        }
                        else player.Notify("Cloths", "Erreur lors de la livraison du vêtement", Life.NotificationManager.Type.Error);
                    }
                    else player.Notify("Cloths", "Erreur lors de la génération du vêtement", Life.NotificationManager.Type.Error);

                    panel.Refresh();
                });
            }

            panel.AddButton("Confirmer", _ => panel.SelectTab());
            panel.PreviousButton();
            panel.CloseButton();

            panel.Display();
        }
    }
}
