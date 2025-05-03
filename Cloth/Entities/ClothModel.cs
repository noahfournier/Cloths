using ModKit.ORM;
using SQLite;

namespace Cloth.Entities
{
    public class ClothModel : ModEntity<ClothModel>
    {
        [AutoIncrement][PrimaryKey] public int Id { get; set; }
        /// <summary>
        /// Identifier for the clothing item.
        /// </summary>
        public int ClothId { get; set; }

        /// <summary>
        /// Identifier for the type of clothing.
        /// </summary>
        public int ClothType { get; set; }

        /// <summary>
        /// Identifier for the gender associated with the clothing.
        /// </summary>
        public int SexId { get; set; }

        /// <summary>
        /// Name or label of the clothing item.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Default unit price for clothing items that reference this model.
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        /// JSON data for the ClothData, containing the URL of the texture to be applied.
        /// </summary>
        public string ClothData { get; set; }

        /// <summary>
        /// UNIX timestamp indicating the creation date.
        /// </summary>
        public long CreatedAt { get; set; }

        /// <summary>
        /// UNIX timestamp indicating the date of the last update.
        /// </summary>
        public long UpdatedAt { get; set; }

        public ClothModel()
        {
        }
    }
}
