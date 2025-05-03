using System;
using Cloth.Entities;
using Cloth.Utils;
using Life.InventorySystem;
using Life.Network;
using Life.UI;
using ModKit.Helper;
using ModKit.Utils;
using Newtonsoft.Json;
using mk = ModKit.Helper.TextFormattingHelper;

namespace Cloth.Panels.Admin
{
    public class AdminClothModelPanels
    {
        public ModKit.ModKit Context { get; set; }

        public AdminClothModelPanels(ModKit.ModKit context)
        {
            Context = context;
        }

        public void SelectSexIdPanel(Player player, ClothModels model)
        {
            Panel panel = Context.PanelHelper.Create(PanelUtils.SetTitlePanel("Créer un modèle", "Choisir le sexe"), UIPanel.PanelType.TabPrice, player, () => SelectSexIdPanel(player, model));

            panel.AddTabLine("Masculin", _ => model.SexId = 0);
            panel.AddTabLine("Féminin", _ => model.SexId = 1);

            panel.PreviousButton();
            panel.NextButton("Sélectionner", () =>
            {
                panel.SelectTab();
                SelectClothTypePanel(player, model);
            });
            panel.CloseButton();

            panel.Display();
        }

        public void SelectClothTypePanel(Player player, ClothModels model)
        {
            Panel panel = Context.PanelHelper.Create(PanelUtils.SetTitlePanel("Créer un modèle", "Choisir le type"), UIPanel.PanelType.TabPrice, player, () => SelectClothTypePanel(player, model));

            panel.AddTabLine($"{ClothType.Shirt}", _ => model.ClothType = (int)ClothType.Shirt);
            panel.AddTabLine($"{ClothType.Pants}", _ => model.ClothType = (int)ClothType.Pants);

            panel.PreviousButton();
            panel.NextButton("Sélectionner", () =>
            {
                panel.SelectTab();
                SelectClothIdPanel(player, model);
            });
            panel.CloseButton();

            panel.Display();
        }

        public void SelectClothIdPanel(Player player, ClothModels model)
        {
            Panel panel = Context.PanelHelper.Create(PanelUtils.SetTitlePanel("Créer un modèle", "Choisir le vêtement de base"), UIPanel.PanelType.TabPrice, player, () => SelectClothIdPanel(player, model));

            foreach (var cloth in ClothUtils.BaseClothing)
            {
                if (cloth.ClothType == model.ClothType && cloth.SexId == model.SexId)
                {
                    panel.AddTabLine($"{cloth.Name}", _ => model.ClothId = cloth.ClothId);
                }
            }

            panel.PreviousButton();
            panel.NextButton("Sélectionner", () =>
            {
                panel.SelectTab();
                SetClothDataPanel(player, model);
            });
            panel.CloseButton();

            panel.Display();
        }

        public void SetClothDataPanel(Player player, ClothModels model)
        {
            Panel panel = Context.PanelHelper.Create(PanelUtils.SetTitlePanel("Créer un modèle", "Choisir le flocage"), UIPanel.PanelType.Input, player, () => SetClothDataPanel(player, model));

            panel.SetInputPlaceholder("Renseigner l'url du flocage à appliquer");

            panel.PreviousButton();
            panel.NextButton("Sélectionner", async () =>
            {
                if (await InputUtils.IsValidImageUrlAsync(panel.inputText))
                {
                    ClothData clothData = new ClothData();
                    clothData.clothName = $"c{model.SexId}{model.ClothType}{model.ClothId}-{model.CreatedAt}";
                    clothData.url = panel.inputText;
                    model.ClothData = JsonConvert.SerializeObject(clothData, Formatting.Indented);
                    SetPricePanel(player, model);
                }
                else
                {
                    player.Notify("Cloth", "URL invalide", Life.NotificationManager.Type.Warning);
                    panel.Refresh();
                }
            });
            panel.CloseButton();

            panel.Display();
        }

        public void SetPricePanel(Player player, ClothModels model)
        {
            Panel panel = Context.PanelHelper.Create(PanelUtils.SetTitlePanel("Créer un modèle", "Définir le prix"), UIPanel.PanelType.Input, player, () => SetPricePanel(player, model));

            panel.SetInputPlaceholder("Renseigner le prix en séparant les centimes par une virgule.");

            panel.PreviousButton();
            panel.NextButton("Sélectionner", () =>
            {
                if (double.TryParse(panel.inputText, out double price) && price >= 0)
                {
                    model.Price = Math.Round(price, 2);
                    SetNamePanel(player, model);
                }
                else
                {
                    player.Notify("Cloths", "Prix invalide", Life.NotificationManager.Type.Warning);
                    panel.Refresh();
                }
            });
            panel.CloseButton();

            panel.Display();
        }

        public void SetNamePanel(Player player, ClothModels model)
        {
            Panel panel = Context.PanelHelper.Create(PanelUtils.SetTitlePanel("Créer un modèle", "Définir le nom"), UIPanel.PanelType.Input, player, () => SetNamePanel(player, model));

            panel.SetInputPlaceholder("Renseigner le nom de ce modèle (3 caractères minimum)");

            panel.PreviousButton();
            panel.NextButton("Sauvegarder", async () =>
            {
                if (panel.inputText != null && panel.inputText.Length >= 3)
                {
                    model.Name = panel.inputText;
                    if (await model.Create())
                    {
                        player.Notify("Cloths", "Succès lors de l'enregistrement de votre modèle", Life.NotificationManager.Type.Success);
                        NotifyClothModelCreatedPanel(player, model);
                    }
                    else
                    {
                        player.Notify("Cloths", "Échec lors de l'enregistrement de votre modèle", Life.NotificationManager.Type.Error);
                        panel.Refresh();
                    }
                }
                else
                {
                    player.Notify("Cloths", "Nom invalide", Life.NotificationManager.Type.Warning);
                    panel.Refresh();
                }
            });
            panel.CloseButton();

            panel.Display();
        }

        public void NotifyClothModelCreatedPanel(Player player, ClothModels model)
        {
            Panel panel = Context.PanelHelper.Create(PanelUtils.SetTitlePanel("Créer un modèle", "Votre modèle est prêt !"), UIPanel.PanelType.TabPrice, player, () => NotifyClothModelCreatedPanel(player, model));

            panel.AddTabLine($"{mk.Color("Nom :", mk.Colors.Info)} {model.Name}", _ => { });
            panel.AddTabLine($"{mk.Color("Vêtement de base :", mk.Colors.Info)} {ClothUtils.GetClothName(model)}", _ => { });
            panel.AddTabLine($"{mk.Color("Type :", mk.Colors.Info)} {Enum.GetName(typeof(ClothType), model.ClothType)}", _ => { });
            panel.AddTabLine($"{mk.Color("Sexe :", mk.Colors.Info)} {(model.SexId == 0 ? "Masculin" : "Féminin")}", _ => { });
            panel.AddTabLine($"{mk.Color("Prix :", mk.Colors.Info)} {model.Price}€", _ => { });
            panel.AddTabLine($"{mk.Color("Url du flocage :", mk.Colors.Info)} {JsonConvert.DeserializeObject<ClothData>(model.ClothData).url}", _ => { });
            panel.AddTabLine($"{mk.Color("Date de création :", mk.Colors.Info)} {DateUtils.FormatUnixTimestamp(model.CreatedAt)}", _ => { });

            panel.AddButton("Prévisualiser", _ =>
            {
                ClothItems clothItem = new ClothItems();
                clothItem.ClothModelId = model.Id;
                ClothUtils.EquipCloth(player, clothItem);
                panel.Refresh();
            });
            // voir la page détail
            // revenir au menu admin
            panel.CloseButton();

            panel.Display();
        }
    }
}
