using Clothes.Points;
using Life;
using Life.Network;
using Life.UI;
using ModKit.Helper;

namespace Clothes.Panels.Admin
{
    public class AdminClothShopPanels
    {
        public ModKit.ModKit Context { get; set; }

        public AdminClothShopPanels(ModKit.ModKit context)
        {
            Context = context;
        }

        public void ClothShopMenuPanel(Player player)
        {

            Panel panel = Context.PanelHelper.Create("Clothes - Points de ventes", UIPanel.PanelType.TabPrice, player, () => ClothShopMenuPanel(player));

            panel.AddTabLine("Créer un point de vente", _ =>
            {
                ClothShopPoint clothShop = new ClothShopPoint(Context);
                SetNamePanel(player, clothShop);
            });

            panel.AddTabLine("Liste des points de ventes", async _ => await Context.MCheckpointHelper.OpenMenu<ClothShopPoint>(player));

            panel.PreviousButton();
            panel.NextButton("Sélectionner", () => panel.SelectTab());
            panel.CloseButton();

            panel.Display();
        }

        public void SetNamePanel(Player player, ClothShopPoint clothShop)
        {

            Panel panel = Context.PanelHelper.Create(PanelUtils.SetTitlePanel("Créer un point de vente","Définir le nom de la boutique"), UIPanel.PanelType.Input, player, () => SetNamePanel(player, clothShop));

            panel.inputPlaceholder = "Comment souhaitez-vous nommer cette boutique de vêtements";
           
            panel.NextButton("Confirmer", () =>
            {
                if(panel.inputText.Length >= 3)
                {
                    clothShop.Name = panel.inputText;
                    ConfirmCreatePanel(player, clothShop);
                } else
                {
                    player.Notify("Clothes","Le nom d'une boutique de vêtement doit faire au minimum 3 caractères", NotificationManager.Type.Warning);
                    panel.Refresh();
                }
            });
            panel.PreviousButton();
            panel.CloseButton();

            panel.Display();
        }

        public void ConfirmCreatePanel(Player player, ClothShopPoint clothShop)
        {

            Panel panel = Context.PanelHelper.Create(PanelUtils.SetTitlePanel("Créer un point de vente", "Confirmer la création du point de vente"), UIPanel.PanelType.Text, player, () => SetNamePanel(player, clothShop));

            panel.TextLines.Add("Votre point est presque prêt !");
            panel.TextLines.Add($"Pour configurer le contenu d'un point de vente \"{clothShop.Name}\"");
            panel.TextLines.Add("Veuillez intéragir avec lui en service admin");
            panel.TextLines.Add("");
            panel.TextLines.Add("Voulez-vous générer le point sur votre position ?");
            panel.TextLines.Add("Sa position peut être modifiée ultérieurement.");

            panel.CloseButtonWithAction("Générer", async () =>
            {
                return await PanelUtils.QueryCreateResponse(player, clothShop.Create(player));
            });
            panel.CloseButtonWithAction("Sauvegarder", async () =>
            {
                return await PanelUtils.QueryCreateResponse(player, clothShop.Save());

            });
            panel.Previous();
            panel.CloseButton();

            panel.Display();
        }
    }
}
