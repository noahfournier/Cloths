using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Clothes.Config;
using Clothes.Entities;
using Clothes.Entities.CompositeEntities;
using Clothes.Utils;
using Life.InventorySystem;
using Life.Network;
using Life.UI;
using ModKit.Helper;
using mk = ModKit.Helper.TextFormattingHelper;

namespace Clothes.Panels.Backpack
{
    public class BackpackPanels
    {
        public string title = "Sac à dos";
        public ModKit.ModKit Context { get; set; }

        public BackpackPanels(ModKit.ModKit context)
        {
            Context = context;
        }

        public async void BackpackMenuPanel(Player player)
        {
            List<ClothRecord> allClothRecords = await CharacterInventories.GetInventoryForCharacterAsync(player.character.Id);

            Panel panel = Context.PanelHelper.Create(PanelUtils.SetTitlePanel(title, $"[emplacements: {allClothRecords.Count} / {ClothesConfig.Data.MaxBackpackSlots}]"), UIPanel.PanelType.Tab, player, () => BackpackMenuPanel(player));

            foreach (ClothType clothType in Enum.GetValues(typeof(ClothType)))
            {
                List<ClothRecord> clothRecords = allClothRecords.Where(i => i.ClothModels.ClothType == (int)clothType).ToList();
                ClothRecord equippedClothRecord = clothRecords.Where(i => i.CharacterInventories.IsEquipped).FirstOrDefault();

                panel.AddTabLine($"{mk.Color($"[{(clothRecords.Count > 0 ? $"Quantité: {clothRecords.Count}" : "Vide")}]",mk.Colors.Info)} {ClothUtils.ClothTypeTranslater(clothType)}", _ =>
                {
                    if(clothRecords.Count > 0) BackpackClothTypePanel(player, clothType, clothRecords, equippedClothRecord);
                    else
                    {
                        player.Notify(PanelUtils.pluginName, $"Vous n'avez aucun {clothType} dans votre sac à dos", Life.NotificationManager.Type.Info);
                        panel.Refresh();
                    }
                });
            }


            panel.NextButton($"{mk.Color("Sélectionner", mk.Colors.Success)}", () => panel.SelectTab());
            panel.PreviousButton($"{mk.Color("Retour", mk.Colors.Info)}");
            panel.CloseButton($"{mk.Color("Fermer", mk.Colors.Error)}");

            panel.Display();
        }

        public void BackpackClothTypePanel(Player player, ClothType clothType, List<ClothRecord> clothRecords, ClothRecord equippedClothRecord)
        {
            Panel panel = Context.PanelHelper.Create(PanelUtils.SetTitlePanel(title, $"Vos {ClothUtils.ClothTypeTranslater(clothType)}"), UIPanel.PanelType.Tab, player, () => BackpackClothTypePanel(player, clothType, clothRecords, equippedClothRecord));

            foreach (ClothRecord clothRecord in clothRecords)
            {
                panel.AddTabLine($"{mk.Color($"{(clothRecord.CharacterInventories.IsEquipped ? "[Équipé] " : "")}",mk.Colors.Info)}{clothRecord.ClothModels.Name}", _ =>
                {
                    ClothUtils.EquipClothing(player, clothRecord, equippedClothRecord);

                    if(equippedClothRecord == clothRecord) equippedClothRecord = null;
                    else equippedClothRecord = clothRecord;

                    panel.Refresh();
                });
            }

            panel.NextButton($"{mk.Color("Sélectionner", mk.Colors.Success)}", () => panel.SelectTab());
            panel.NextButton($"{mk.Color("Autres", mk.Colors.Warning)}", () => BackpackOtherActionsPanel(player, clothRecords[panel.selectedTab]));
            panel.PreviousButton($"{mk.Color("Retour", mk.Colors.Info)}");
            panel.CloseButton($"{mk.Color("Fermer", mk.Colors.Error)}");

            panel.Display();
        }

        public void BackpackOtherActionsPanel(Player player, ClothRecord clothRecord)
        {
            Panel panel = Context.PanelHelper.Create(PanelUtils.SetTitlePanel(title, "Choisissez une action"), UIPanel.PanelType.TabPrice, player, () => BackpackOtherActionsPanel(player, clothRecord));

            panel.AddTabLine($"{mk.Color($"Vous êtes sur votre {ClothUtils.ClothTypeTranslater((ClothType)clothRecord.ClothModels.ClothType)}", mk.Colors.Warning)}<br>{mk.Color("Nom: ", mk.Colors.Info) }{clothRecord.ClothModels.Name}", _ =>
            {
                player.Notify(PanelUtils.pluginName, "Choisissez l'action que vous souhaitez réaliser.", Life.NotificationManager.Type.Info);
                panel.Refresh();
            });

            panel.AddTabLine("Offrir","",PanelUtils.GetItemIconId(1076), _ =>
            {
                if(!clothRecord.CharacterInventories.IsEquipped)
                {
                    var nearbyPlayers = EnvironmentUtils.DetectNearbyPlayers(player, false);
                    if (nearbyPlayers.Count > 0) SelectPlayerToTransferClothingPanel(player, clothRecord, nearbyPlayers);
                    else
                    {
                        player.Notify(PanelUtils.pluginName, "Aucun joueur n'est à proximité", Life.NotificationManager.Type.Warning);
                        panel.Refresh();
                    }
                }
                else
                {
                    player.Notify(PanelUtils.pluginName, "Vous devez déséquiper ce vêtement avant de l'offrir", Life.NotificationManager.Type.Warning);
                    panel.Refresh();
                }
            });

            panel.NextButton($"{mk.Color("Sélectionner", mk.Colors.Success)}", () => panel.SelectTab());
            panel.PreviousButton($"{mk.Color("Retour", mk.Colors.Info)}");
            panel.CloseButton($"{mk.Color("Fermer", mk.Colors.Error)}");

            panel.Display();
        }

        private void SelectPlayerToTransferClothingPanel(Player player, ClothRecord clothRecord, Dictionary<int, Player> nearbyPlayers)
        {
            Panel panel = Context.PanelHelper.Create(PanelUtils.SetTitlePanel(title, "Choisissez le destinataire"), UIPanel.PanelType.TabPrice, player, () => SelectPlayerToTransferClothingPanel(player, clothRecord, nearbyPlayers));

            foreach (var p in nearbyPlayers)
            {
                panel.AddTabLine($"{p.Value.FullName}", async _ =>
                {
                    var query = await CharacterInventories.Query(i => i.CharacterId == player.character.Id);
                    var backpackItemsCount = query.Count();

                    if (backpackItemsCount < ClothesConfig.Data.MaxBackpackSlots) ConfirmClothingTransferPanel(player, clothRecord, p.Value);
                    else
                    {
                        player.Notify(PanelUtils.pluginName, $"Le sac à dos de {p.Value.FullName} est plein !", Life.NotificationManager.Type.Warning);
                        panel.Refresh();
                    }
                });
            }

            panel.NextButton($"{mk.Color("Sélectionner", mk.Colors.Success)}", () => panel.SelectTab());
            panel.PreviousButton($"{mk.Color("Retour", mk.Colors.Info)}");
            panel.CloseButton($"{mk.Color("Fermer", mk.Colors.Error)}");

            panel.Display();
        }

        private void ConfirmClothingTransferPanel(Player player, ClothRecord clothRecord, Player target)
        {
            Panel panel = Context.PanelHelper.Create(PanelUtils.SetTitlePanel(title, "Confirmer l'envoi du vêtement"), UIPanel.PanelType.Text, player, () => ConfirmClothingTransferPanel(player, clothRecord, target));

            panel.TextLines.Add("Voulez-vous vraiment donner");
            panel.TextLines.Add($"{ClothUtils.ClothTypeTranslater((ClothType)clothRecord.ClothModels.ClothType)} \"{clothRecord.ClothModels.Name}\"");
            panel.TextLines.Add($"à {target.FullName} ?");

            panel.CloseButtonWithAction($"{mk.Color("Confirmer", mk.Colors.Success)}", () =>
            {
                RequestClothingTransferPanel(target, clothRecord, player);
                return Task.FromResult(true);
            });
            panel.PreviousButton($"{mk.Color("Retour", mk.Colors.Info)}");
            panel.CloseButton($"{mk.Color("Fermer", mk.Colors.Error)}");

            panel.Display();
        }

        private void RequestClothingTransferPanel(Player player, ClothRecord clothRecord, Player fromPlayer)
        {
            Panel panel = Context.PanelHelper.Create(PanelUtils.SetTitlePanel(title, "Demande d'ajout d'un vêtement"), UIPanel.PanelType.Text, player, () => RequestClothingTransferPanel(player, clothRecord, fromPlayer));

            panel.TextLines.Add($"{fromPlayer.FullName} souhaite vous offrir");
            panel.TextLines.Add($"{ClothUtils.ClothTypeTranslater((ClothType)clothRecord.ClothModels.ClothType)} \"{clothRecord.ClothModels.Name}\"");


            panel.AddButton($"{mk.Color("Accepter", mk.Colors.Success)}", async _ =>
            {
                clothRecord.CharacterInventories.CharacterId = player.character.Id;
                
                if(await clothRecord.CharacterInventories.Save())
                {
                    fromPlayer.Notify(PanelUtils.pluginName, $"{player.FullName} accepte votre vêtement.", Life.NotificationManager.Type.Info);
                    player.Notify(PanelUtils.pluginName, $"Vous recevez le vêtement \"{clothRecord.ClothModels.Name}\" !", Life.NotificationManager.Type.Success);
                }
                else
                {
                    fromPlayer.Notify(PanelUtils.pluginName, $"Erreur lors du transfert", Life.NotificationManager.Type.Error);
                    player.Notify(PanelUtils.pluginName, $"Erreur lors du transfert", Life.NotificationManager.Type.Error);
                }

                panel.Close();
            });
            panel.AddButton($"{mk.Color("Refuser", mk.Colors.Error)}", _ =>
            {
                fromPlayer.Notify(PanelUtils.pluginName, $"{player.FullName} refuse votre vêtement", Life.NotificationManager.Type.Info);
                panel.Close();
            });

            panel.Display();
        }
    }
}
