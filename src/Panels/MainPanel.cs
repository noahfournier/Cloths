using Life.Network;
using Life.UI;
using ModKit.Helper;
using Clothes.Panels.Backpack;
using Clothes.Panels.Admin;
using Life.AreaSystem;
using Clothes.Panels.Wardrobe;

namespace Clothes.Panels
{
    public class MainPanel
    {
        AdminPanels _adminPanels { get; }
        BackpackPanels _backpackPanels { get; }
        WardrobePanels _wardrobePanels { get; }
        public ModKit.ModKit Context { get; set; }

        public MainPanel(ModKit.ModKit context)
        {
            _adminPanels = new AdminPanels(context);
            _backpackPanels = new BackpackPanels(context);
            _wardrobePanels = new WardrobePanels(context);
            Context = context;
        }

        public void MenuPanel(Player player)
        {
            Panel panel = Context.PanelHelper.Create("Cloths - Menu", UIPanel.PanelType.TabPrice, player, () => MenuPanel(player));

            panel.AddTabLine("Menu Admin", _ => _adminPanels.AdminMenuPanel(player));
            panel.AddTabLine("Sac à dos", _ => _backpackPanels.BackpackMenuPanel(player));
            panel.AddTabLine("Garde-robe", _ =>
            {
                AreaObject wardrobe = Utils.EnvironmentUtils.DetectNearbyWardrobe(player);
                if (wardrobe.areaInstance.lifeArea.permissions.HasPermission(player.character.Id))
                {
                    _wardrobePanels.WardrobeMenuPanel(player, wardrobe.areaInstance.lifeArea);
                }
                else
                {
                    player.Notify("Clothes", "Vous n'avez pas la permission d'accéder à cette garde-robe", Life.NotificationManager.Type.Info);
                    panel.Refresh();
                }
            });

            panel.NextButton("Sélectionner", () => panel.SelectTab());
            panel.CloseButton();

            panel.Display();
        }
    }
}
