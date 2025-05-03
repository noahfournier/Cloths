using System;
using System.Collections.Generic;
using System.Linq;
using Cloth.Entities;
using Life.InventorySystem;
using Life.Network;
using Life.UI;
using ModKit.Helper;
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
            Panel panel = Context.PanelHelper.Create(PanelUtils.SetTitlePanel("Créer un vêtement", "Choisir le type selon le sexe"), UIPanel.PanelType.TabPrice, player, () => SelectClothTypePanel(player));

            foreach (var clothType in Enum.GetValues(typeof(ClothType)))
            {
                List<ClothModels> maleModels = Cloth.CacheManager.ClothModelsCache.Where(m => m.Value.SexId == 0 && m.Value.ClothType == (int)clothType).Select(m => m.Value).ToList();
                if(maleModels.Count > 0) panel.AddTabLine($"{mk.Color("[H]", mk.Colors.Info)} {clothType}", _ => SelectClothModelPanel(player, maleModels));

                List<ClothModels> femaleModels = Cloth.CacheManager.ClothModelsCache.Where(m => m.Value.SexId == 1 && m.Value.ClothType == (int)clothType).Select(m => m.Value).ToList();
                if (femaleModels.Count > 0) panel.AddTabLine($"{mk.Color("[F]", mk.Colors.Purple)} {clothType}", _ => SelectClothModelPanel(player, femaleModels));
            }

            panel.PreviousButton();
            panel.NextButton("Sélectionner", () => panel.SelectTab());
            panel.CloseButton();

            panel.Display();
        }

        private void SelectClothModelPanel(Player player, List<ClothModels> models)
        {
            Panel panel = Context.PanelHelper.Create(PanelUtils.SetTitlePanel("Créer un vêtement", "Choisir le modèle du vêtement"), UIPanel.PanelType.TabPrice, player, () => SelectClothModelPanel(player, models));

            foreach (ClothModels model in models)
            {
                panel.AddTabLine($"{model.Name}", _ => { });
            }

            // Sélectionner
            // Prévisualiser
            panel.PreviousButton();
            panel.CloseButton();

            panel.Display();
        }
    }
}
