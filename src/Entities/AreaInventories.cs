using ModKit.ORM;
using SQLite;

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
    }
}
