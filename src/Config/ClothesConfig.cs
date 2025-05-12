using Newtonsoft.Json;
using SQLite;
using System;
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
        /// Duration in seconds before a previewed clothing item is removed.
        /// </summary>
        public int PreviewDurationInSeconds = 6;

        /// <summary>
        /// List of IDs that are considered as wardrobe
        /// Items IDs: https://docs.google.com/spreadsheets/d/1kdvg91jcMozdSUnP6n0ng_0_lgwTXt30CSv6uXjRT98/edit?gid=0#gid=0
        /// </summary>
        public string WardrobeItemIds { get; set; } = "[1007,1008]";

        [JsonIgnore]
        public List<int> WardrobeItemIdsList
        {
            get => ListConverter.ReadJson(WardrobeItemIds);
            set => WardrobeItemIds = ListConverter.WriteJson(value);
        }

        /// <summary>
        /// Gets the preview duration as a TimeSpan.
        /// </summary>
        /// <returns>A TimeSpan representing the preview duration.</returns>
        public TimeSpan GetPreviewDurationTimeSpan()
        {
            return TimeSpan.FromSeconds(PreviewDurationInSeconds);
        }
    }
}
