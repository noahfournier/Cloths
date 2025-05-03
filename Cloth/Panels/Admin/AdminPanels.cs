using Life.Network;
using Life.UI;
using ModKit.Helper;
using mk = ModKit.Helper.TextFormattingHelper;
using Cloth.Entities;
using ModKit.Utils;

namespace Cloth.Panels
{
    public class AdminPanels
    {
        AdminClothModelPanels _adminClothModelPanels { get; }
        public ModKit.ModKit Context { get; set; }

        public AdminPanels(ModKit.ModKit context)
        {
            _adminClothModelPanels = new AdminClothModelPanels(context);
            Context = context;
        }

        public void AdminMenuPanel(Player player)
        {
            Panel panel = Context.PanelHelper.Create("Cloths - Menu Admin", UIPanel.PanelType.TabPrice, player, () => AdminMenuPanel(player));

            panel.AddTabLine($"{mk.Color("Créer un modèle", mk.Colors.Verbose)}", _ =>
            {
                ClothModel model = new ClothModel();
                model.CreatedAt = DateUtils.GetCurrentTime();
                _adminClothModelPanels.SelectSexIdPanel(player, model);
            });

            panel.AddTabLine($"{mk.Color("Générer un vêtement", mk.Colors.Verbose)}", _ =>
            {
                //to do
            });

            panel.AddButton("Retour", _ => AAMenu.AAMenu.menu.AdminPluginPanel(player));
            panel.NextButton("Sélectionner", () => panel.SelectTab());
            panel.CloseButton();

            panel.Display();
        }

        #region ClothModel
        
        #endregion

    }
}
