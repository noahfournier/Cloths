using Life.Network;
using Life.UI;
using ModKit.Helper;
using mk = ModKit.Helper.TextFormattingHelper;
using Clothes.Entities;
using ModKit.Utils;

namespace Clothes.Panels.Admin
{
    public class AdminPanels
    {
        AdminClothModelPanels _adminClothModelPanels { get; }
        AdminClothItemPanels _adminClothItemPanels { get; }
        AdminClothShopPanels _adminClothShopPanels { get; }
        public ModKit.ModKit Context { get; set; }

        public AdminPanels(ModKit.ModKit context)
        {
            _adminClothModelPanels = new AdminClothModelPanels(context);
            _adminClothItemPanels = new AdminClothItemPanels(context);
            _adminClothShopPanels = new AdminClothShopPanels(context);
            Context = context;
        }

        public void AdminMenuPanel(Player player)
        {
            Panel panel = Context.PanelHelper.Create("Clothes - Menu Admin", UIPanel.PanelType.TabPrice, player, () => AdminMenuPanel(player));

            panel.AddTabLine($"{mk.Color("Créer un modèle", mk.Colors.Verbose)}", _ =>
            {
                ClothModels model = new ClothModels();
                model.CreatedAt = DateUtils.GetCurrentTime();
                _adminClothModelPanels.SelectSexIdPanel(player, model);
            });

            panel.AddTabLine($"{mk.Color("Liste des modèles", mk.Colors.Verbose)}", _ =>
            {
                _adminClothModelPanels.ClothModelMenuPanel(player);
            });

            panel.AddTabLine($"{mk.Color("Générer un vêtement", mk.Colors.Verbose)}", _ =>
            {
                _adminClothItemPanels.SelectClothTypePanel(player);
            });

            panel.AddTabLine($"{mk.Color("Points de ventes", mk.Colors.Verbose)}", _ =>
            {
                _adminClothShopPanels.ClothShopMenuPanel(player);
            });

            panel.PreviousButton();
            panel.NextButton("Sélectionner", () => panel.SelectTab());
            panel.CloseButton();

            panel.Display();
        }
    }
}
