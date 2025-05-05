using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cloth.Entities;
using Life.InventorySystem;
using Life.Network;
using Newtonsoft.Json;

namespace Cloth.Utils
{
    public static class ClothUtils
    {
        public struct ClothModelStruct
        {
            /// <summary>
            /// Identifier for the clothing item.
            /// </summary>
            public int ClothId { get; }

            /// <summary>
            /// Identifier for the type of clothing.
            /// </summary>
            public int ClothType { get; }

            /// <summary>
            /// Identifier for the gender associated with the clothing.
            /// </summary>
            public int SexId { get; }

            /// <summary>
            /// Name or label of the clothing item.
            /// </summary>
            public string Name { get; }

            /// <summary>
            /// Default unit price for clothing items that reference this model.
            /// </summary>
            public double Price { get; set; }

            public ClothModelStruct(int clothId, int clothType, int sexId, string name, double price = 0)
            {
                ClothId = clothId;
                ClothType = clothType;
                SexId = sexId;
                Name = name;
                Price = price;
            }
        }

        /// <summary>
        /// List of the game's basic clothing items.
        /// </summary>
        public static List<ClothModelStruct> BaseClothing = new List<ClothModelStruct>
        {
            new ClothModelStruct(3, (int)ClothType.Hat, 0, "Casquette de policier"),
            new ClothModelStruct(4, (int)ClothType.Hat, 0, "Casque de pompier"),
            new ClothModelStruct(5, (int)ClothType.Hat, 0, "Casque de protection"),
            new ClothModelStruct(6, (int)ClothType.Hat, 0, "Casque du SWAT"),
            new ClothModelStruct(7, (int)ClothType.Hat, 0, "Chapeau de cowboy"),
            new ClothModelStruct(8, (int)ClothType.Hat, 0, "Bonnet de Noël"),
            new ClothModelStruct(10, (int)ClothType.Shirt, 0, "Gilet de policier"),
            new ClothModelStruct(11, (int)ClothType.Shirt, 0, "Manteau de pompier"),
            new ClothModelStruct(12, (int)ClothType.Shirt, 0, "Blouse de médecin"),
            new ClothModelStruct(13, (int)ClothType.Shirt, 0, "Gilet jaune"),
            new ClothModelStruct(14, (int)ClothType.Shirt, 0, "Haut du SWAT"),
            new ClothModelStruct(-1, (int)ClothType.Shirt, 0, "Torse nu"),
            new ClothModelStruct(10, (int)ClothType.Pants, 0, "Pantalon de policier"),
            new ClothModelStruct(11, (int)ClothType.Pants, 0, "Pantalon de pompier"),
            new ClothModelStruct(12, (int)ClothType.Pants, 0, "Pantalon de médecin"),
            new ClothModelStruct(13, (int)ClothType.Pants, 0, "Pantalon de chantier"),
            new ClothModelStruct(14, (int)ClothType.Pants, 0, "Pantalon du SWAT"),
            new ClothModelStruct(-1, (int)ClothType.Pants, 0, "Sous-vêtement"),
            new ClothModelStruct(10, (int)ClothType.Shoes, 0, "Chaussures de policier"),
            new ClothModelStruct(11, (int)ClothType.Shoes, 0, "Chaussures de pompier"),
            new ClothModelStruct(12, (int)ClothType.Shoes, 0, "Chaussures de médecin"),
            new ClothModelStruct(13, (int)ClothType.Shoes, 0, "Chaussures de chantier"),
            new ClothModelStruct(14, (int)ClothType.Shoes, 0, "Chaussures du SWAT"),
            new ClothModelStruct(-1, (int)ClothType.Shoes, 0, "Pieds nu"),
            new ClothModelStruct(3, (int)ClothType.Hat, 1, "Casquette de policier"),
            new ClothModelStruct(4, (int)ClothType.Hat, 1, "Casque de pompier"),
            new ClothModelStruct(5, (int)ClothType.Hat, 1, "Casque de protection"),
            new ClothModelStruct(6, (int)ClothType.Hat, 1, "Casque du SWAT"),
            new ClothModelStruct(7, (int)ClothType.Hat, 1, "Chapeau de cowboy"),
            new ClothModelStruct(8, (int)ClothType.Hat, 1, "Bonnet de Noël"),
            new ClothModelStruct(10, (int)ClothType.Shirt, 1, "Gilet de policier"),
            new ClothModelStruct(11, (int)ClothType.Shirt, 1, "Manteau de pompier"),
            new ClothModelStruct(12, (int)ClothType.Shirt, 1, "Blouse de médecin"),
            new ClothModelStruct(13, (int)ClothType.Shirt, 1, "Gilet jaune"),
            new ClothModelStruct(14, (int)ClothType.Shirt, 1, "Haut du SWAT"),
            new ClothModelStruct(-1, (int)ClothType.Shirt, 1, "Torse nu"),
            new ClothModelStruct(9, (int)ClothType.Pants, 1, "Pantalon de policier"),
            new ClothModelStruct(10, (int)ClothType.Pants, 1, "Pantalon de pompier"),
            new ClothModelStruct(11, (int)ClothType.Pants, 1, "Pantalon de médecin"),
            new ClothModelStruct(12, (int)ClothType.Pants, 1, "Pantalon de chantier"),
            new ClothModelStruct(13, (int)ClothType.Pants, 1, "Pantalon du SWAT"),
            new ClothModelStruct(-1, (int)ClothType.Pants, 1, "Sous-vêtement"),
            new ClothModelStruct(10, (int)ClothType.Shoes, 1, "Chaussures de policier"),
            new ClothModelStruct(11, (int)ClothType.Shoes, 1, "Chaussures de pompier"),
            new ClothModelStruct(12, (int)ClothType.Shoes, 1, "Chaussures de médecin"),
            new ClothModelStruct(13, (int)ClothType.Shoes, 1, "Chaussures de chantier"),
            new ClothModelStruct(14, (int)ClothType.Shoes, 1, "Chaussures du SWAT"),
            new ClothModelStruct(-1, (int)ClothType.Shoes, 1, "Pieds nu")
        };

        /// <summary>
        /// Gets the original name of a clothing model.
        /// </summary>
        /// <param name="model">The clothing model.</param>
        /// <returns>The original name of the clothing model or "Unknown" if not found.</returns>
        public static string GetClothName(ClothModels model)
        {
            var clothModel = BaseClothing.FirstOrDefault(c =>
                c.ClothId == model.ClothId &&
                c.ClothType == model.ClothType &&
                c.SexId == model.SexId);

            return clothModel.Name ?? "Inconnu";
        }

        /// <summary>
        /// Equips the player with a specific clothing item.
        /// </summary>
        /// <param name="player">The player to equip.</param>
        /// <param name="clothRecord">The clothing item to equip.</param>
        /// <param name="equippedClothRecord">The currently equipped clothing item, if any.</param>
        /// <returns>True if the clothing item was equipped successfully, otherwise false.</returns>
        public static bool EquipClothing(Player player, ClothRecord clothRecord, ClothRecord equippedClothRecord = null)
        {
            if (clothRecord.ClothModels == null)
            {
                player.Notify("Cloth","Erreur lors de la récupération du modèle de votre vêtement",Life.NotificationManager.Type.Error);
                return false;
            }

            ApplyClothData(player, clothRecord.ClothModels);

            if(equippedClothRecord != null)
            {
                equippedClothRecord.CharacterInventories.IsEquipped = false;
                equippedClothRecord.CharacterInventories.Save();
            }
            clothRecord.CharacterInventories.IsEquipped = true;
            clothRecord.CharacterInventories.Save();

            return true;
        }

        /// <summary>
        /// Previews a clothing item on the player.
        /// </summary>
        /// <param name="player">The player on whom to preview the clothing item.</param>
        /// <param name="model">The clothing model to preview.</param>
        public static void PreviewClothing(Player player, ClothModels model)
        {
            ApplyClothData(player, model, true);
        }

        /// <summary>
        /// Applies the data from a clothing model to the player. If <paramref name="isPreview"/> is true,
        /// the changes are applied after a delay and only if the clothing item is equipped.
        /// </summary>
        /// <param name="player">The player to whom the clothing data will be applied.</param>
        /// <param name="model">The clothing model containing the data to be applied.</param>
        /// <param name="isPreview">Indicates whether the changes are a preview. Default is false.</param>
        private static void ApplyClothData(Player player, ClothModels model, bool isPreview = false)
        {
            
            if (isPreview)
            {
                Task.Run(async () =>
                {
                    ClothRecord record = await CharacterInventories.GetEquippedClothRecordByClothTypeAsync(player.character.Id, model.ClothType);
                    if(record != null)
                    {
                        await Task.Delay(6000);
                        player.Notify("Cloths", "Prévisualisation en cours", Life.NotificationManager.Type.Info, 6);
                        ApplyClothData(player, record.ClothModels);
                    }
                });
            }

            ClothData clothData = new ClothData();
            if (model.ClothData != null) clothData = JsonConvert.DeserializeObject<ClothData>(model.ClothData);

            switch (model.ClothType)
            {
                case (int)ClothType.Hat:
                    player.setup.characterSkinData.Hat = model.ClothId;
                    break;
                case (int)ClothType.Accessory:
                    player.setup.characterSkinData.Accessory = model.ClothId;
                    break;
                case (int)ClothType.Shirt:
                    player.setup.characterSkinData.TShirt = model.ClothId;
                    if (model.ClothData != null) player.setup.characterSkinData.tshirtData = clothData;
                    else player.setup.characterSkinData.tshirtData = new ClothData();
                    break;
                case (int)ClothType.Pants:
                    player.setup.characterSkinData.Pants = model.ClothId;
                    if (model.ClothData != null) player.setup.characterSkinData.pantsData = clothData;
                    else player.setup.characterSkinData.pantsData = new ClothData();
                    break;
                case (int)ClothType.Shoes:
                    player.setup.characterSkinData.Shoes = model.ClothId;
                    break;
                default:
                    break;
            }

            if(!isPreview) player.character.Skin = player.setup.characterSkinData.SerializeToJson();
            player.setup.RpcSkinChange(player.setup.characterSkinData);
        }
    }
}
