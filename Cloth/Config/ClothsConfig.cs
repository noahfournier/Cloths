namespace Cloth.Config
{
    public class ClothsConfig : ModKit.Helper.JsonHelper.JsonEntity<ClothsConfig>
    {
        /// <summary>
        /// Maximum quantity of clothes in a backpack
        /// </summary>
        public int MaxBackpackSlots = 10;
    }
}
