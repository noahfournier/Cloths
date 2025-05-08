using Life.Network;
using Life.UI;
using ModKit.Helper;
using Clothes.Panels.Backpack;
using Clothes.Panels.Admin;

namespace Clothes.Panels
{
    public class MainPanel
    {
        AdminPanels _adminPanels { get; }
        BackpackPanels _backpackPanels { get; }
        public ModKit.ModKit Context { get; set; }

        public MainPanel(ModKit.ModKit context)
        {
            _adminPanels = new AdminPanels(context);
            _backpackPanels = new BackpackPanels(context);
            Context = context;
        }

        public void MenuPanel(Player player)
        {
            Panel panel = Context.PanelHelper.Create("Cloths - Menu", UIPanel.PanelType.TabPrice, player, () => MenuPanel(player));

            panel.AddTabLine("Menu Admin", _ => _adminPanels.AdminMenuPanel(player));
            panel.AddTabLine("Sac à dos", _ => _backpackPanels.BackpackMenuPanel(player));
            panel.AddTabLine("Garde-robe", _ => { });

            panel.NextButton("Sélectionner", () => panel.SelectTab());
            panel.CloseButton();

            panel.Display();
        }
    }
}
