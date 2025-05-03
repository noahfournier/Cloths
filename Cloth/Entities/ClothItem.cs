using ModKit.ORM;
using SQLite;

namespace Cloth.Entities
{
    public class ClothItem : ModEntity<ClothItem>
    {
        [AutoIncrement][PrimaryKey] public int Id { get; set; }

       public int ClothModelId { get; set; }

        /// <summary>
        /// Indicates whether the clothing item is dirty or not.
        /// </summary>
        public bool IsDirty { get; set; }

        /// <summary>
        /// UNIX timestamp indicating the creation date.
        /// </summary>
        public long CreatedAt { get; set; }

        /// <summary>
        /// UNIX timestamp indicating the date of the last update.
        /// </summary>
        public long UpdatedAt { get; set; }

        public ClothItem()
        { 
        }
    }
}
