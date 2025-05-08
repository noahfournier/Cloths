namespace Clothes.Config
{
    public class ClothesConfig : ModKit.Helper.JsonHelper.JsonEntity<ClothesConfig>
    {
        /// <summary>
        /// Maximum quantity of clothes in a backpack
        /// </summary>
        public int MaxBackpackSlots = 10;
    }
}
