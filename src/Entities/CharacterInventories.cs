using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Clothes.Entities.CompositeEntities;
using ModKit;
using ModKit.Internal;
using ModKit.ORM;
using SQLite;

namespace Clothes.Entities
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

        public CharacterInventories(int characterId, int clothItemId)
        {
            CharacterId = characterId;
            ClothItemId = clothItemId;
            IsEquipped = false;
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
                        else
                        {
                            /// If ClothModel does not exist, then we remove the data related to it.
                            await item.Delete();
                            await clothItem.Delete();
                        }
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

        /// <summary>
        /// Retrieves the equipped clothing record for a specific character and cloth type.
        /// </summary>
        /// <param name="characterId">The identifier of the character.</param>
        /// <param name="clothType">The type of the clothing item.</param>
        /// <returns>A <see cref="ClothRecord"/> representing the equipped clothing item, or null if not found.</returns>
        public static async Task<ClothRecord> GetEquippedClothRecordByClothTypeAsync(int characterId, int clothType)
        {
            ClothRecord clothRecord = null;

            try
            {
                var characterInventories = await Query(i => i.CharacterId == characterId && i.IsEquipped);

                foreach (var item in characterInventories)
                {
                    var clothItem = await ClothItems.Query(item.ClothItemId);

                    if (clothItem != null)
                    {
                        var clothModel = await ClothModels.Query(clothItem.ClothModelId);

                        if (clothModel != null && clothModel.ClothType == clothType)
                        {
                            return new ClothRecord
                            {
                                CharacterInventories = item,
                                ClothItems = clothItem,
                                ClothModels = clothModel
                            };
                        }
                    }
                    else Logger.LogError("GetEquippedClothRecordByTypeAsync", $"ClothItem not found for ClothItemId: {item.ClothItemId}");                   
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("GetEquippedClothRecordByTypeAsync", $"{ex.Message}");
            }

            return clothRecord;
        }
    }
}
