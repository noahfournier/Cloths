using Life.Network;
using Life.UI;
using ModKit.Helper;
using Clothes.Panels.Backpack;
using Clothes.Panels.Admin;
using Life.AreaSystem;
using Clothes.Panels.Wardrobe;
using mk = ModKit.Helper.TextFormattingHelper;

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
            Panel panel = Context.PanelHelper.Create(PanelUtils.SetTitlePanel(), UIPanel.PanelType.TabPrice, player, () => MenuPanel(player));

            if(player.IsAdmin) panel.AddTabLine($"{mk.Color("Menu Admin", mk.Colors.Orange)}", _ => _adminPanels.AdminMenuPanel(player));
            panel.AddTabLine("Sac à dos", "", player.character.SexId == 0 ? PanelUtils.maleTopIcon : PanelUtils.femaleTopIcon, _ => _backpackPanels.BackpackMenuPanel(player));
            panel.AddTabLine("Garde-robe", "", PanelUtils.GetItemIconId(1008), _ =>
            {
                AreaObject wardrobe = Utils.EnvironmentUtils.DetectNearbyWardrobe(player);
                if(wardrobe != null)
                {
                    if (wardrobe.areaInstance.lifeArea.permissions.HasPermission(player.character.Id))
                    {
                        _wardrobePanels.WardrobeMenuPanel(player, wardrobe.areaInstance.lifeArea);
                        return;
                    }
                    else player.Notify("Clothes", "Vous n'avez pas la permission d'accéder à cette garde-robe", Life.NotificationManager.Type.Info);
                } else player.Notify("Clothes", "Il n'y a aucune penderie à votre proximité", Life.NotificationManager.Type.Info);

                panel.Refresh();
            });

            panel.NextButton($"{mk.Color("Sélectionner", mk.Colors.Success)}", () => panel.SelectTab());
            panel.CloseButton($"{mk.Color("Fermer", mk.Colors.Error)}");

            panel.Display();
        }
    }
}
