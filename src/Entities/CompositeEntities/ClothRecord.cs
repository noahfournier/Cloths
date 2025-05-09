namespace Clothes.Entities.CompositeEntities
{
    public class ClothRecord
    {
        public CharacterInventories CharacterInventories { get; set; }
        public AreaInventories AreaInventories { get; set; }
        public ClothItems ClothItems { get; set; }
        public ClothModels ClothModels { get; set; }
    }
}
