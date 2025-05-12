using System;
using System.Collections.Generic;
using System.Linq;
using Clothes.Entities;
using Clothes.Utils;
using Life.InventorySystem;
using Life.Network;
using Life.UI;
using ModKit.Helper;
using ModKit.Utils;
using Newtonsoft.Json;
using mk = ModKit.Helper.TextFormattingHelper;

namespace Clothes.Panels.Admin
{
    public class AdminClothModelPanels
    {
        public ModKit.ModKit Context { get; set; }

        public AdminClothModelPanels(ModKit.ModKit context)
        {
            Context = context;
        }

        public async void ClothModelMenuPanel(Player player)
        {
            List<ClothModels> allCustomModels = await ClothModels.Query(c => c.ClothData != null);
            List<ClothModels> customShirts = allCustomModels.Where(i => i.ClothType == (int)ClothType.Shirt).ToList();
            List<ClothModels> customPants = allCustomModels.Where(i => i.ClothType == (int)ClothType.Pants).ToList();

            Panel panel = Context.PanelHelper.Create(PanelUtils.SetTitlePanel("Liste des modèles", "Sélectionner le type de vêtement"), UIPanel.PanelType.TabPrice, player, () => ClothModelMenuPanel(player));

            panel.AddTabLine($"{PanelUtils.GetQuantityTagTabLine(customShirts.Count)} {ClothUtils.ClothTypeTranslater(ClothType.Shirt)}", _ =>
            {
                if (customShirts.Count > 0) ListPanel(player, customShirts, ClothType.Shirt);
                else
                {
                    player.Notify(PanelUtils.pluginName, "Vous n'avez aucun modèle de tshirts", Life.NotificationManager.Type.Info);
                    panel.Refresh();
                }
            });

            panel.AddTabLine($"{PanelUtils.GetQuantityTagTabLine(customPants.Count)} {ClothUtils.ClothTypeTranslater(ClothType.Pants)}", _ =>
            {
                if (customPants.Count > 0) ListPanel(player, customPants, ClothType.Pants);
                else
                {
                    player.Notify(PanelUtils.pluginName, "Vous n'avez aucun modèle de pantalons", Life.NotificationManager.Type.Info);
                    panel.Refresh();
                }
            });

            
            panel.NextButton($"{mk.Color("Sélectionner", mk.Colors.Success)}", () => panel.SelectTab());
            panel.PreviousButton($"{mk.Color("Retour", mk.Colors.Info)}");
            panel.CloseButton($"{mk.Color("Fermer", mk.Colors.Error)}");

            panel.Display();
        }

        public void ListPanel(Player player, List<ClothModels> customModels, ClothType clothType)
        {
            Panel panel = Context.PanelHelper.Create(PanelUtils.SetTitlePanel("Liste des modèles", $"Vos modèles de {ClothUtils.ClothTypeTranslater(clothType)}"), UIPanel.PanelType.TabPrice, player, () => ListPanel(player, customModels, clothType));

            foreach (ClothModels clothModel in customModels)
            {
                panel.AddTabLine($"{clothModel.Name}", _ => EditClothModelPanel(player, clothModel));
            }

            if(customModels.Count > 0)
            {
                panel.NextButton($"{mk.Color("Modifier", mk.Colors.Success)}", () => panel.SelectTab());
                panel.NextButton($"{mk.Color("Supprimer", mk.Colors.Warning)}", () => ConfirmDeletePanel(player, customModels, customModels[panel.selectedTab]));
            }
            panel.PreviousButton($"{mk.Color("Retour", mk.Colors.Info)}");
            panel.CloseButton($"{mk.Color("Fermer", mk.Colors.Error)}");

            panel.Display();
        }

        public void ConfirmDeletePanel(Player player, List<ClothModels> customModels, ClothModels model)
        {
            Panel panel = Context.PanelHelper.Create(PanelUtils.SetTitlePanel("Supprimer un modèle", $"Demande de confirmation"), UIPanel.PanelType.Text, player, () => ConfirmDeletePanel(player, customModels, model));

            panel.TextLines.Add("Voulez-vous vraiment supprimer définitivement");
            panel.TextLines.Add($"[{ClothUtils.ClothTypeTranslater((ClothType)model.ClothType)}] {model.Name}");
            
            panel.PreviousButtonWithAction($"{mk.Color("Confirmer la suppression", mk.Colors.Success)}", async () =>
            {
                if (await model.Delete())
                {
                    player.Notify("Clothes", "Modèle supprimé avec succès", Life.NotificationManager.Type.Success);
                    customModels.Remove(model);
                    return true;
                }
                else
                {
                    player.Notify("Clothes", "Erreur lors de la supression du modèle", Life.NotificationManager.Type.Error);
                    return false;
                }
                
            });
            panel.PreviousButton($"{mk.Color("Retour", mk.Colors.Info)}");
            panel.CloseButton($"{mk.Color("Fermer", mk.Colors.Error)}");

            panel.Display();
        }

        public void EditClothModelPanel(Player player, ClothModels model)
        {
            Panel panel = Context.PanelHelper.Create(PanelUtils.SetTitlePanel("Modifier un modèle", $"Choisir la valeur à modifier"), UIPanel.PanelType.TabPrice, player, () => EditClothModelPanel(player, model));

            panel.AddTabLine($"{mk.Color("Vêtement de base :", mk.Colors.Info)} {ClothUtils.GetClothName(model)}", _ => PanelUtils.NotEditableValue(player, panel));
            panel.AddTabLine($"{mk.Color("Type :", mk.Colors.Info)} {ClothUtils.ClothTypeTranslater((ClothType)model.ClothType)}", _ => PanelUtils.NotEditableValue(player, panel));
            panel.AddTabLine($"{mk.Color("Sexe :", mk.Colors.Info)} {(model.SexId == 0 ? "Masculin" : "Féminin")}", _ => PanelUtils.NotEditableValue(player, panel));

            panel.AddTabLine($"{mk.Color("Nom :", mk.Colors.Warning)} {model.Name}", _ => SetNamePanel(player, model, false));        
            panel.AddTabLine($"{mk.Color("Prix :", mk.Colors.Warning)} {model.Price}€", _ => SetPricePanel(player, model, false));
            panel.AddTabLine($"{mk.Color("Url du flocage :", mk.Colors.Warning)} {JsonConvert.DeserializeObject<ClothData>(model.ClothData).url}", _ =>
            {
                SetClothDataPanel(player, model, false);
            });

            panel.AddTabLine($"{mk.Color("Date de création :", mk.Colors.Info)} {DateUtils.FormatUnixTimestamp(model.CreatedAt)}", _ => PanelUtils.NotEditableValue(player, panel));
            panel.AddTabLine($"{mk.Color("Dernière mise à jour :", mk.Colors.Info)} {DateUtils.FormatUnixTimestamp(model.CreatedAt)}", _ => PanelUtils.NotEditableValue(player, panel));

            panel.NextButton("Sélectionner", () => panel.SelectTab());
            panel.PreviousButton();
            panel.CloseButton();

            panel.Display();
        }

        #region CREATE OR UPDATE
        public void SelectSexIdPanel(Player player, ClothModels model)
        {
            Panel panel = Context.PanelHelper.Create(PanelUtils.SetTitlePanel("Créer un modèle", "Choisir le sexe"), UIPanel.PanelType.TabPrice, player, () => SelectSexIdPanel(player, model));

            panel.AddTabLine("Masculin", _ => model.SexId = 0);
            panel.AddTabLine("Féminin", _ => model.SexId = 1);

            panel.NextButton($"{mk.Color("Sélectionner", mk.Colors.Success)}", () =>
            {
                panel.SelectTab();
                SelectClothTypePanel(player, model);
            });
            panel.PreviousButton($"{mk.Color("Retour", mk.Colors.Info)}");
            panel.CloseButton($"{mk.Color("Fermer", mk.Colors.Error)}");

            panel.Display();
        }

        public void SelectClothTypePanel(Player player, ClothModels model)
        {
            Panel panel = Context.PanelHelper.Create(PanelUtils.SetTitlePanel("Créer un modèle", "Choisir le type"), UIPanel.PanelType.TabPrice, player, () => SelectClothTypePanel(player, model));

            panel.AddTabLine($"{ClothUtils.ClothTypeTranslater(ClothType.Shirt)}", _ => model.ClothType = (int)ClothType.Shirt);
            panel.AddTabLine($"{ClothUtils.ClothTypeTranslater(ClothType.Pants)}", _ => model.ClothType = (int)ClothType.Pants);

            panel.NextButton($"{mk.Color("Sélectionner", mk.Colors.Success)}", () =>
            {
                panel.SelectTab();
                SelectClothIdPanel(player, model);
            });
            panel.PreviousButton($"{mk.Color("Retour", mk.Colors.Info)}");
            panel.CloseButton($"{mk.Color("Fermer", mk.Colors.Error)}");

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

            panel.NextButton($"{mk.Color("Sélectionner", mk.Colors.Success)}", () =>
            {
                panel.SelectTab();
                SetClothDataPanel(player, model);
            });
            panel.PreviousButton($"{mk.Color("Retour", mk.Colors.Info)}");
            panel.CloseButton($"{mk.Color("Fermer", mk.Colors.Error)}");

            panel.Display();
        }

        public void SetClothDataPanel(Player player, ClothModels model, bool isCreating = true)
        {
            Panel panel = Context.PanelHelper.Create(PanelUtils.SetTitlePanel($"{(isCreating ? "Créer un modèle" : "Modifier un modèle")}", "Définir le flocage"), UIPanel.PanelType.Input, player, () => SetClothDataPanel(player, model, isCreating));

            panel.SetInputPlaceholder("Renseigner l'url du flocage à appliquer");

            if(isCreating)
            {
                panel.NextButton($"{mk.Color("Confirmer", mk.Colors.Success)}", async () =>
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
                        player.Notify("Clothes", "URL invalide", Life.NotificationManager.Type.Warning);
                        panel.Refresh();
                    }
                });
            } 
            else
            {
                panel.PreviousButtonWithAction($"{mk.Color("Confirmer", mk.Colors.Success)}", async () =>
                {
                    if (await InputUtils.IsValidImageUrlAsync(panel.inputText))
                    {
                        ClothData clothData = new ClothData();
                        clothData.clothName = $"c{model.SexId}{model.ClothType}{model.ClothId}-{model.CreatedAt}";
                        clothData.url = panel.inputText;
                        model.ClothData = JsonConvert.SerializeObject(clothData, Formatting.Indented);
                        return await PanelUtils.QueryUpdateResponse(player, model.Save());
                    }
                    else player.Notify("Clothes", "URL invalide", Life.NotificationManager.Type.Warning);
                    return false;
                });
            }

            panel.PreviousButton($"{mk.Color("Retour", mk.Colors.Info)}");
            panel.CloseButton($"{mk.Color("Fermer", mk.Colors.Error)}");

            panel.Display();
        }

        public void SetPricePanel(Player player, ClothModels model, bool isCreating = true)
        {
            Panel panel = Context.PanelHelper.Create(PanelUtils.SetTitlePanel($"{(isCreating ? "Créer un modèle" : "Modifier un modèle")}", "Définir le prix"), UIPanel.PanelType.Input, player, () => SetPricePanel(player, model, isCreating));

            panel.SetInputPlaceholder("Renseigner le prix en séparant les centimes par une virgule.");

            if (isCreating)
            {
                panel.NextButton($"{mk.Color("Confirmer", mk.Colors.Success)}", () =>
                {
                    if (double.TryParse(panel.inputText, out double price) && price >= 0)
                    {
                        model.Price = Math.Round(price, 2);
                        SetNamePanel(player, model);
                    }
                    else
                    {
                        player.Notify("Clothes", "Prix invalide", Life.NotificationManager.Type.Warning);
                        panel.Refresh();
                    }
                });
            }
            else
            {
                panel.PreviousButtonWithAction($"{mk.Color("Confirmer", mk.Colors.Success)}", async () =>
                {
                    if (double.TryParse(panel.inputText, out double price) && price >= 0)
                    {
                        model.Price = Math.Round(price, 2);
                        return await PanelUtils.QueryUpdateResponse(player, model.Save());
                    }
                    else player.Notify("Clothes", "Prix invalide", Life.NotificationManager.Type.Warning);
                    return false;
                });
            }
            panel.PreviousButton($"{mk.Color("Retour", mk.Colors.Info)}");
            panel.CloseButton($"{mk.Color("Fermer", mk.Colors.Error)}");

            panel.Display();
        }

        public void SetNamePanel(Player player, ClothModels model, bool isCreating = true)
        {
            Panel panel = Context.PanelHelper.Create(PanelUtils.SetTitlePanel($"{(isCreating ? "Créer un modèle" : "Modifier un modèle")}", "Définir le nom"), UIPanel.PanelType.Input, player, () => SetNamePanel(player, model, isCreating));

            panel.SetInputPlaceholder("Renseigner le nom de ce modèle (3 caractères minimum)");

            panel.PreviousButton();
            if(isCreating)
            {
                panel.NextButton($"{mk.Color("Confirmer et sauvegarder", mk.Colors.Success)}", async () =>
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
            }
            else
            {
                panel.PreviousButtonWithAction($"{mk.Color("Confirmer", mk.Colors.Success)}", async () =>
                {
                    if (panel.inputText != null && panel.inputText.Length >= 3)
                    {
                        model.Name = panel.inputText;
                        return await PanelUtils.QueryUpdateResponse(player, model.Save());
                    }
                    else player.Notify("Cloths", "Nom invalide", Life.NotificationManager.Type.Warning);
                    return false;
                });
            }
            panel.PreviousButton($"{mk.Color("Retour", mk.Colors.Info)}");
            panel.CloseButton($"{mk.Color("Fermer", mk.Colors.Error)}");

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

            panel.AddButton($"{mk.Color("Prévisualiser", mk.Colors.Info)}", _ =>
            {
                ClothUtils.PreviewClothing(player, model);
                panel.Refresh();
            });
            // voir la page détail
            // revenir au menu admin
            panel.CloseButton($"{mk.Color("Fermer", mk.Colors.Error)}");

            panel.Display();
        }
        #endregion
    }
}
