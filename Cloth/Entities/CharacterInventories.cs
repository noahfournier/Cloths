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


        public static async Task<List<ClothRecord>> GetInventoryForCharacterAsync(int characterId)
        {
            var allClothRecords = new List<ClothRecord>();
            var connection = ModKit.ORM.Orm.GetOrmInstance().SqliteConnection;

            if (connection == null) throw new Exception("SQLiteAsyncConnection is not initialized.");
            
            try
            {
                var characterInventories = await Query(i => i.CharacterId == characterId);

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
                        else Console.WriteLine($"ClothModel not found for ClothModelId: {clothItem.ClothModelId}");                    
                    }
                    else Console.WriteLine($"ClothItem not found for ClothItemId: {item.ClothItemId}");
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
