using Newtonsoft.Json;
using SQLite;
using System.Collections.Generic;

namespace Clothes.Config
{
    public class ClothesConfig : ModKit.Helper.JsonHelper.JsonEntity<ClothesConfig>
    {
        /// <summary>
        /// Maximum quantity of clothes in a backpack
        /// </summary>
        public int MaxBackpackSlots = 10;

        /// <summary>
        /// Maximum quantity of clothes in a wardrobe
        /// 0 = unlimited.
        /// </summary>
        public int MaxWardrobeSlots = 0;

        /// <summary>
        /// List of IDs that are considered as wardrobe
        /// </summary>
        public string WardrobeItemIds { get; set; } = "[1007,1008]";

        [JsonIgnore]
        public List<int> WardrobeItemIdsList
        {
            get => ListConverter.ReadJson(WardrobeItemIds);
            set => WardrobeItemIds = ListConverter.WriteJson(value);
        }
    }
}
