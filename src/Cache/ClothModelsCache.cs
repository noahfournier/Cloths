using System.Collections.Generic;
using System.Threading.Tasks;
using Clothes.Entities;

namespace Clothes.Cache
{
    public class ClothModelsCache
    {
        public Dictionary<int, ClothModels> Cache = new Dictionary<int, ClothModels>();

        public ClothModelsCache(){
        }

        /// <summary>
        /// Refreshes the cache of clothing models asynchronously.
        /// </summary>
        public async Task RefreshClothModelsCache()
        {
            List<ClothModels> clothModels = await ClothModels.QueryAll();
            foreach (var model in clothModels)
            {
                Cache[model.Id] = model;
            }
        }

        /// <summary>
        /// Adds or updates a clothing model in the cache.
        /// </summary>
        /// <param name="model">The clothing model to add or update.</param>
        public void AddOrUpdateClothModel(ClothModels model)
        {
            Cache[model.Id] = model;
        }
    }
}
