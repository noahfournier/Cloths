using ModKit.ORM;
using SQLite;

namespace Cloth.Entities
{
    public class ClothItems : ModEntity<ClothItems>
    {
        [AutoIncrement][PrimaryKey] public int Id { get; set; }

        /// <summary>
        /// Reference ClothModel
        /// </summary>
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

        public ClothItems()
        { 
        }
    }
}
