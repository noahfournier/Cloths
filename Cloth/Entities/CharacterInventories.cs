using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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


        public static async Task<List<CharacterInventoryItem>> GetInventoryForCharacterAsync(int characterId)
        {
            var inventoryItems = new List<CharacterInventoryItem>();
            var connection = ModKit.ORM.Orm.GetOrmInstance().SqliteConnection;

            if (connection == null)
            {
                throw new Exception("SQLiteAsyncConnection is not initialized.");
            }

            string query = @"
            SELECT
                ci.Id AS InventoryId,
                ci.CharacterId,
                ci.IsEquipped,
                ci.ClothItemId,
                cli.IsDirty,
                clm.ClothId,
                clm.ClothType,
                clm.SexId,
                clm.Name,
                clm.ClothData,
                clm.Price
            FROM
                CharacterInventories ci
            JOIN
                ClothItems cli ON ci.ClothItemId = cli.ClothItemId
            JOIN
                ClothModels clm ON cli.ClothModelId = clm.ClothId
            WHERE
                ci.CharacterId = ?";

            try
            {
                var results = await connection.QueryAsync<CharacterInventoryItem>(query, characterId);
                inventoryItems.AddRange(results);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing query: {ex.Message}");
            }

            return inventoryItems;
        }

    }

    public class CharacterInventoryItem
    {
        public int InventoryId { get; set; }
        public int CharacterId { get; set; }
        public bool IsEquipped { get; set; }
        public int ClothItemId { get; set; }
        public bool IsDirty { get; set; }
        public int ClothId { get; set; }
        public string ClothType { get; set; }
        public int SexId { get; set; }
        public string Name { get; set; }
        public string ClothData { get; set; }
        public float Price { get; set; }
    }
}
