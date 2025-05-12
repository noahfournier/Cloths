using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ModKit.ORM;
using SQLite;
using Clothes.Entities.CompositeEntities;
using ModKit.Internal;

namespace Clothes.Entities
{
    public class AreaInventories : ModEntity<AreaInventories>
    {
        [AutoIncrement][PrimaryKey] public int Id { get; set; }

        /// <summary>
        /// Area identifier
        /// </summary>
        public int AreaId { get; set; }

        /// <summary>
        /// Reference to ClothItem
        /// </summary>
        public int ClothItemId { get; set; }

        public AreaInventories()
        {
        }

        public AreaInventories(int areaId, int clothItemId)
        {
            AreaId = areaId;
            ClothItemId = clothItemId;
        }

        /// <summary>
        /// Retrieves the clothing inventory for a specific area.
        /// </summary>
        /// <param name="areaId">The identifier of the area.</param>
        /// <returns>A list of <see cref="ClothRecord"/> representing the area's clothing inventory.</returns>
        public static async Task<List<ClothRecord>> GetInventoryForAreaAsync(int areaId)
        {
            var allClothRecords = new List<ClothRecord>();

            try
            {
                var areaInventories = await Query(i => i.AreaId == areaId);

                foreach (var item in areaInventories)
                {
                    var clothItem = await ClothItems.Query(item.ClothItemId);

                    if (clothItem != null)
                    {
                        var clothModel = await ClothModels.Query(clothItem.ClothModelId);

                        if (clothModel != null)
                        {
                            var clothRecord = new ClothRecord
                            {
                                AreaInventories = item,
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
                    else Logger.LogError("GetInventoryForAreaAsync", $"ClothItem not found for ClothItemId: {item.ClothItemId}");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("GetInventoryForAreaAsync", $"{ex.Message}");
            }

            return allClothRecords;
        }
    }
}
