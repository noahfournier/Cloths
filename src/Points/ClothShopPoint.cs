using System.Threading.Tasks;
using Life.Network;
using ModKit.Helper.CheckpointHelper;
using ModKit.ORM;
using SQLite;
using System.Collections.Generic;
using System.Collections;
using ModKit.Helper;
using Life.UI;
using Life.InventorySystem;
using System;
using Clothes.Utils;
using Clothes.Entities;
using Clothes.Panels;
using System.Linq;
using Clothes.Entities.CompositeEntities;
using mk = ModKit.Helper.TextFormattingHelper;
using static Life.InventorySystem.Item;

namespace Clothes.Points
{
    public class ClothShopPoint : ModEntity<ClothShopPoint>, IMCheckpoint
    {
        [AutoIncrement][PrimaryKey] public int Id { get; set; }

        public string ClothModelList { get; set; } = "[]";

        [Ignore]
        public List<int> ClothModelIds
        {
            get => ListConverter.ReadJson(ClothModelList);
            set => ClothModelList = ListConverter.WriteJson(value);
        }

        #region NE PAS TOUCHER
        public string Name { get; set; } = "Default";

        public string MCheckpointIdList { get; set; } = "[]";

        [Ignore]
        public List<int> MCheckpointIds
        {
            get => ListConverter.ReadJson(MCheckpointIdList);
            set => MCheckpointIdList = ListConverter.WriteJson(value);
        }

        [Ignore] public ModKit.ModKit Context { get; set; }

        public ClothShopPoint() { }
        public ClothShopPoint(ModKit.ModKit context)
        {
            Context = context;
        }
        #endregion

        private void OnPlayerTrigger(Player player)
        {
            ClothShopMenuPanel(player);
        }

        #region PANELS
        public async void ClothShopMenuPanel(Player player)
        {
            List<ClothModels> clothModels = await ClothModels.QueryAll();
            List<ClothModels> clothModelsForSale = clothModels.Where(i => ClothModelIds.Contains(i.Id)).ToList();

            Panel panel = Context.PanelHelper.Create($"{Name}", UIPanel.PanelType.TabPrice, player, () => ClothShopMenuPanel(player));

            foreach (ClothType clothType in Enum.GetValues(typeof(ClothType)))
            {
                List<ClothModels> clothModelsByType = clothModelsForSale.Where(i => i.ClothType == (int)clothType).ToList();
                if(clothModelsByType.Count > 0 || player.serviceAdmin)
                {
                    panel.AddTabLine($"{PanelUtils.GetQuantityTagTabLine(clothModelsByType.Count)} {ClothUtils.ClothTypeTranslater(clothType)}", _ => ClothModelForSalePanel(player, clothType, clothModelsByType));
                }
            }

            panel.NextButton($"{mk.Color("Sélectionner", mk.Colors.Success)}", () => panel.SelectTab());
            panel.NextButton($"{mk.Color("Vendre", mk.Colors.Warning)}", () => BackpackToClothShopPanel(player));
            if (player.IsAdmin && player.serviceAdmin) panel.NextButton($"{mk.Color("Configurer", mk.Colors.Orange)}", () => SetClothModelListPanel(player, (ClothType)panel.selectedTab));
            panel.CloseButton($"{mk.Color("Fermer", mk.Colors.Error)}");

            panel.Display();
        }

        public async void BackpackToClothShopPanel(Player player)
        {
            List<ClothRecord> clothRecords = await CharacterInventories.GetInventoryForCharacterAsync(player.character.Id);

            Panel panel = Context.PanelHelper.Create($"{Name} - Vendre", UIPanel.PanelType.TabPrice, player, () => BackpackToClothShopPanel(player));

            foreach (ClothRecord clothRecord in clothRecords)
            {
                panel.AddTabLine($"{ClothUtils.ClothTypeTranslater((ClothType)clothRecord.ClothModels.ClothType)} - {PanelUtils.GetClothModelTabLine(clothRecord.ClothModels)}", _ =>
                {
                    double amount = Math.Round(clothRecord.ClothModels.Price / 2, 2);
                    ConfirmClothingSalePanel(player, clothRecord, amount);
                });
            }

            panel.NextButton("Sélectionner", () => panel.SelectTab());
            panel.PreviousButton();

            panel.Display();
        }

        private void ConfirmClothingSalePanel(Player player, ClothRecord clothRecord, double amount)
        {
            Panel panel = Context.PanelHelper.Create($"{Name} - Confirmer la vente", UIPanel.PanelType.Text, player, () => ConfirmClothingSalePanel(player, clothRecord, amount));

            panel.TextLines.Add("Voulez-vous vraiment vendre");
            panel.TextLines.Add($"{Enum.GetName(typeof(ClothType), clothRecord.ClothModels.ClothType)} \"{clothRecord.ClothModels.Name}\"");
            panel.TextLines.Add($"pour {amount}€");

            panel.PreviousButtonWithAction("Confirmer", async () =>
            {
                if (clothRecord.CharacterInventories.IsEquipped) ClothUtils.EquipClothing(player, clothRecord);

                if (await clothRecord.CharacterInventories.Delete())
                {
                    if (await clothRecord.ClothItems.Delete())
                    {
                        player.AddMoney(amount, $"Vente d'un vêtement à la boutique \"{Name}\"");
                        player.Notify("Clothes", $"Vous venez de vendre votre vêtement pour {amount}€", Life.NotificationManager.Type.Success);
                        return true;
                    }
                    else player.Notify("Clothes", "Erreur lors de la suppression du vêtement", Life.NotificationManager.Type.Error);
                }
                else player.Notify("Clothes", "Erreur lors du retrait du vêtement de votre sac à dos", Life.NotificationManager.Type.Error);
                
                return false;
            });
            panel.PreviousButton();
            panel.CloseButton();

            panel.Display();
        }

        public async void SetClothModelListPanel(Player player, ClothType clothType)
        {
            List<ClothModels> clothModels = await ClothModels.Query(i => i.ClothType == (int)clothType);
            Panel panel = Context.PanelHelper.Create($"{Name} - Gestion des {clothType}", UIPanel.PanelType.TabPrice, player, () => SetClothModelListPanel(player, clothType));
           
            foreach (ClothModels clothModel in clothModels)
            {
                bool isForSale = ClothModelIds.Contains(clothModel.Id);
                panel.AddTabLine(
                    PanelUtils.GetClothModelTabLine(clothModel), 
                    PanelUtils.GetClothModelPriceTabLine(clothModel, isForSale), 
                    PanelUtils.blankIcon, 
                    async _ =>
                    {
                        if (isForSale)
                        {
                            var updatedList = ClothModelIds;
                            updatedList.Remove(clothModel.Id);
                            ClothModelIds = updatedList;
                        }
                        else
                        {
                            var updatedList = ClothModelIds;
                            updatedList.Add(clothModel.Id);
                            ClothModelIds = updatedList;
                        }
                        await PanelUtils.QueryUpdateResponse(player, Save());
                        panel.Refresh();
                    });
            }

            panel.NextButton("Ajouter/Retirer", () => panel.SelectTab());
            panel.NextButton("Modifier le prix", () => SetPricePanel(player, clothModels[panel.selectedTab]));
            panel.PreviousButton();
            panel.CloseButton();

            panel.Display();
        }

        public void SetPricePanel(Player player, ClothModels model)
        {
            Panel panel = Context.PanelHelper.Create(PanelUtils.SetTitlePanel($"Modifier un modèle", "Définir le prix"), UIPanel.PanelType.Input, player, () => SetPricePanel(player, model));

            panel.SetInputPlaceholder("Renseigner le prix en séparant les centimes par une virgule.");

            panel.PreviousButtonWithAction($"{mk.Color("Confirmer", mk.Colors.Success)}", async () =>
            {
                if (double.TryParse(panel.inputText, out double price) && price >= 0)
                {
                    model.Price = Math.Round(price, 2);
                    return await PanelUtils.QueryUpdateResponse(player, model.Save());
                }
                else player.Notify(PanelUtils.pluginName, "Prix invalide", Life.NotificationManager.Type.Warning);
                return false;
            });
            panel.PreviousButton($"{mk.Color("Retour", mk.Colors.Info)}");
            panel.CloseButton($"{mk.Color("Fermer", mk.Colors.Error)}");

            panel.Display();
        }

        public void ClothModelForSalePanel(Player player, ClothType clothType, List<ClothModels> clothModelsForSale)
        {
            Panel panel = Context.PanelHelper.Create($"{Name} - {clothType}", UIPanel.PanelType.TabPrice, player, () => ClothModelForSalePanel(player, clothType, clothModelsForSale));

            foreach (ClothModels clothModel in clothModelsForSale)
            {
                panel.AddTabLine(
                    PanelUtils.GetClothModelTabLine(clothModel),
                    $"{clothModel.Price}€",
                    PanelUtils.blankIcon,
                    async _ =>
                    {
                        if (await ClothUtils.HasAvailableSlotsInBackpack(player))
                        {
                            if (ClothUtils.CanBuyItem(player, clothModel.Price))
                            {
                                player.AddMoney(-clothModel.Price, $"Achat d'un vêtement à la boutique \"{Name}\"");
                                await ClothUtils.DeliverClothItem(player, clothModel);
                            }
                        }

                        panel.Refresh();
                    });
            }

            panel.NextButton("Acheter", () => panel.SelectTab());
            panel.AddButton("Prévisualiser", _ =>
            {
                ClothUtils.PreviewClothing(player, clothModelsForSale[panel.selectedTab]);
                panel.Refresh();
            });
            panel.PreviousButton();
            panel.CloseButton();

            panel.Display();
        }
        #endregion

        #region NE PAS TOUCHER
        public async Task<bool> Create(Player player)
        {
            return await MCheckpoint.Create(player, OnPlayerTrigger, this);
        }

        public void Generate(Player player, List<MCheckpoint> mCheckpoints)
        {
            MCheckpoint.Generate(player, OnPlayerTrigger, mCheckpoints, this);
        }


        public async Task<IList> QueryAllHandler()
        {
            List<ClothShopPoint> result = await QueryAll();
            return result;
        }
        #endregion
    }
}
