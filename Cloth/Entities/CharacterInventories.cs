using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ModKit.Internal;
using ModKit.ORM;
using SQLite;

namespace Cloth.Entities
{
    public class CharacterInventories : ModEntity<CharacterInventories>
    {
        [AutoIncrement][PrimaryKey] public int Id { get; set; }

        /// <summary>
        /// Character identifier
        /// </summary>
        public int CharacterId { get; set; }

        /// <summary>
        /// Reference to ClothItem
        /// </summary>
        public int ClothItemId { get; set; }

        /// <summary>
        /// Indicates whether the clothing item is equipped or not.
        /// </summary>
        public bool IsEquipped { get; set; }

        public CharacterInventories()
        {
        }


        /// <summary>
        /// Retrieves the clothing inventory for a specific character.
        /// </summary>
        /// <param name="characterId">The identifier of the character.</param>
        /// <param name="isEquipped">Indicates whether only equipped clothing items should be retrieved. Default is false.</param>
        /// <returns>A list of <see cref="ClothRecord"/> representing the character's clothing inventory.</returns>
        public static async Task<List<ClothRecord>> GetInventoryForCharacterAsync(int characterId, bool isEquipped = false)
        {
            var allClothRecords = new List<ClothRecord>();

            try
            {
                var characterInventories = isEquipped ?
                    await Query(i => i.CharacterId == characterId && i.IsEquipped) :
                    await Query(i => i.CharacterId == characterId);

                foreach (var item in characterInventories)
                {
                    var clothItem = await ClothItems.Query(item.ClothItemId);

                    if (clothItem != null)
                    {
                        var clothModel = await ClothModels.Query(clothItem.ClothModelId);

                        if (clothModel != null)
                        {
                            var clothRecord = new ClothRecord
                            {
                                CharacterInventories = item,
                                ClothItems = clothItem,
                                ClothModels = clothModel
                            };

                            allClothRecords.Add(clothRecord);
                        }
                        else Logger.LogError("GetInventoryForCharacterAsync", $"ClothModel not found for ClothModelId: {clothItem.ClothModelId}");
                    }
                    else Logger.LogError("GetInventoryForCharacterAsync", $"ClothItem not found for ClothItemId: {item.ClothItemId}");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("GetInventoryForCharacterAsync", $"{ex.Message}");
            }

            return allClothRecords;
        }

    }

    public class ClothRecord
    {
        public CharacterInventories CharacterInventories { get; set; }
        public ClothItems ClothItems { get; set; }
        public ClothModels ClothModels { get; set; }
    }
}
