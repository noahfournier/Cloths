using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cloth.Config;
using Cloth.Entities;
using Cloth.Utils;
using Life.InventorySystem;
using Life.Network;
using Life.UI;
using ModKit.Helper;
using mk = ModKit.Helper.TextFormattingHelper;

namespace Cloth.Panels.Backpack
{
    public class BackpackPanels
    {
        public ModKit.ModKit Context { get; set; }

        public BackpackPanels(ModKit.ModKit context)
        {
            Context = context;
        }

        public async void BackpackMenuPanel(Player player)
        {
            List<ClothRecord> allClothRecords = await CharacterInventories.GetInventoryForCharacterAsync(player.character.Id);

            Panel panel = Context.PanelHelper.Create(PanelUtils.SetTitlePanel("Sac à dos", $"[emplacements: {allClothRecords.Count} / 10]"), UIPanel.PanelType.TabPrice, player, () => BackpackMenuPanel(player));

            foreach (ClothType clothType in Enum.GetValues(typeof(ClothType)))
            {
                List<ClothRecord> clothRecords = allClothRecords.Where(i => i.ClothModels.ClothType == (int)clothType).ToList();
                ClothRecord equippedClothRecord = clothRecords.Where(i => i.CharacterInventories.IsEquipped).FirstOrDefault();

                panel.AddTabLine($"[{(clothRecords.Count > 0 ? $"Quantité: {clothRecords.Count}" : "Vide")}] {clothType}", _ =>
                {
                    if(clothRecords.Count > 0) BackpackClothTypePanel(player, clothType, clothRecords, equippedClothRecord);
                    else
                    {
                        player.Notify("Cloths", $"Vous n'avez aucun {clothType} dans votre sac à dos", Life.NotificationManager.Type.Info);
                        panel.Refresh();
                    }
                });
            }

            panel.PreviousButton();
            panel.NextButton("Sélectionner", () => panel.SelectTab());
            panel.CloseButton();

            panel.Display();
        }

        public void BackpackClothTypePanel(Player player, ClothType clothType, List<ClothRecord> clothRecords, ClothRecord equippedClothRecord)
        {
            Panel panel = Context.PanelHelper.Create(PanelUtils.SetTitlePanel("Sac à dos", $"Vos {clothType}"), UIPanel.PanelType.TabPrice, player, () => BackpackClothTypePanel(player, clothType, clothRecords, equippedClothRecord));

            foreach (ClothRecord clothRecord in clothRecords)
            {
                panel.AddTabLine($"{(clothRecord.CharacterInventories.IsEquipped ? "[Équipé] " :"")}{clothRecord.ClothModels.Name}", _ =>
                {
                    if(!clothRecord.CharacterInventories.IsEquipped) ClothUtils.EquipClothing(player, clothRecord, equippedClothRecord);
                    else
                    {
                        player.Notify("Cloth", "Vous portez déjà ce vêtement", Life.NotificationManager.Type.Warning);
                        panel.Refresh();
                    }
                });
            }

            panel.NextButton("Sélectionner", () => panel.SelectTab());
            panel.NextButton("Autres", () => BackpackOtherActionsPanel(player, clothRecords[panel.selectedTab]));
            panel.PreviousButton();        
            panel.CloseButton();

            panel.Display();
        }

        public void BackpackOtherActionsPanel(Player player, ClothRecord clothRecord)
        {
            Panel panel = Context.PanelHelper.Create(PanelUtils.SetTitlePanel("Sac à dos", "Quelle action réaliser pour votre vêtement ?"), UIPanel.PanelType.TabPrice, player, () => BackpackOtherActionsPanel(player, clothRecord));

            panel.AddTabLine($"{mk.Color($"Vous êtes sur votre {Enum.GetName(typeof(ClothType), clothRecord.ClothModels.ClothType)}", mk.Colors.Warning)}<br>{mk.Color("Nom: ", mk.Colors.Info) }{clothRecord.ClothModels.Name}", _ =>
            {
                player.Notify("Cloths", "Choisissez l'action que vous souhaitez réaliser.", Life.NotificationManager.Type.Info);
                panel.Refresh();
            });

            panel.AddTabLine("Offrir", _ =>
            {
                if(!clothRecord.CharacterInventories.IsEquipped)
                {
                    var nearbyPlayers = EnvironmentUtils.DetectNearbyPlayers(player, false);
                    if (nearbyPlayers.Count > 0) SelectPlayerToTransferClothingPanel(player, clothRecord, nearbyPlayers);
                    else
                    {
                        player.Notify("Cloths", "Aucun joueur n'est à proximité", Life.NotificationManager.Type.Warning);
                        panel.Refresh();
                    }
                }
                else
                {
                    player.Notify("Cloths", "Vous devez déséquiper ce vêtement avant de l'offrir", Life.NotificationManager.Type.Warning);
                    panel.Refresh();
                }
            });

            panel.NextButton("Sélectionner", () => panel.SelectTab());
            panel.PreviousButton();
            panel.CloseButton();

            panel.Display();
        }

        private void SelectPlayerToTransferClothingPanel(Player player, ClothRecord clothRecord, Dictionary<int, Player> nearbyPlayers)
        {
            Panel panel = Context.PanelHelper.Create(PanelUtils.SetTitlePanel("Sac à dos", "Définir le destinataire du vêtement"), UIPanel.PanelType.TabPrice, player, () => SelectPlayerToTransferClothingPanel(player, clothRecord, nearbyPlayers));

            foreach (var p in nearbyPlayers)
            {
                panel.AddTabLine($"{p.Value.FullName}", async _ =>
                {
                    List<ClothRecord> clothRecords = await CharacterInventories.GetInventoryForCharacterAsync(p.Value.character.Id);
                    if (clothRecords.Count < ClothsConfig.Data.MaxBackpackSlots) ConfirmClothingTransferPanel(player, clothRecord, p.Value);
                    else
                    {
                        player.Notify("Cloths", $"Le sac à dos de {p.Value.FullName} est plein !", Life.NotificationManager.Type.Warning);
                        panel.Refresh();
                    }
                });
            }

            panel.AddButton("Sélectionner", _ => panel.SelectTab());
            panel.PreviousButton();
            panel.CloseButton();

            panel.Display();
        }

        private void ConfirmClothingTransferPanel(Player player, ClothRecord clothRecord, Player target)
        {
            Panel panel = Context.PanelHelper.Create(PanelUtils.SetTitlePanel("Sac à dos", "Confirmer l'envoi du vêtement"), UIPanel.PanelType.Text, player, () => ConfirmClothingTransferPanel(player, clothRecord, target));

            panel.TextLines.Add("Voulez-vous vraiment donner");
            panel.TextLines.Add($"{Enum.GetName(typeof(ClothType), clothRecord.ClothModels.ClothType)} \"{clothRecord.ClothModels.Name}\"");
            panel.TextLines.Add($"à {target.FullName} ?");

            panel.CloseButtonWithAction("Confirmer", () =>
            {
                RequestClothingTransferPanel(target, clothRecord, player);
                return Task.FromResult(true);
            });
            panel.PreviousButton();
            panel.CloseButton();

            panel.Display();
        }

        private void RequestClothingTransferPanel(Player player, ClothRecord clothRecord, Player fromPlayer)
        {
            Panel panel = Context.PanelHelper.Create(PanelUtils.SetTitlePanel("Sac à dos", "Demande d'ajout d'un vêtement"), UIPanel.PanelType.Text, player, () => RequestClothingTransferPanel(player, clothRecord, fromPlayer));

            panel.TextLines.Add($"{fromPlayer.FullName} souhaite vous offrir");
            panel.TextLines.Add($"{Enum.GetName(typeof(ClothType), clothRecord.ClothModels.ClothType)} \"{clothRecord.ClothModels.Name}\"");


            panel.AddButton("Accepter", async _ =>
            {
                clothRecord.CharacterInventories.CharacterId = player.character.Id;
                
                if(await clothRecord.CharacterInventories.Save())
                {
                    fromPlayer.Notify("Cloths", $"{player.FullName} accepte votre vêtement.", Life.NotificationManager.Type.Info);
                    player.Notify("Cloths", $"Vous recevez le vêtement \"{clothRecord.ClothModels.Name}\" !", Life.NotificationManager.Type.Success);
                }
                else
                {
                    fromPlayer.Notify("Cloths", $"Erreur lors du transfert", Life.NotificationManager.Type.Error);
                    player.Notify("Cloths", $"Erreur lors du transfert", Life.NotificationManager.Type.Error);
                }

                panel.Close();
            });
            panel.AddButton("Refuser", _ =>
            {
                fromPlayer.Notify("Cloths", $"{player.FullName} refuse votre vêtement", Life.NotificationManager.Type.Info);
                panel.Close();
            });
            panel.PreviousButton();
            panel.CloseButton();

            panel.Display();
        }
    }
}
