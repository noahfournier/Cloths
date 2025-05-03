using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cloth.Entities;
using ModKit.Internal;

namespace Cloth.Cache
{
    public class CacheManager
    {
        public Dictionary<int, ClothModels> ClothModelsCache = new Dictionary<int, ClothModels>();


        public CacheManager()
        {
        }

        public async Task InitializeCacheAsync()
        {
            try
            {
                await RefreshClothModelsCache();
            } catch (Exception ex)
            {
                Logger.LogError("InitializeCacheAsync", ex.Message);
            }
        }


        public async Task RefreshClothModelsCache()
        {
            List<ClothModels> clothModels = await ClothModels.QueryAll();
            foreach (var model in clothModels)
            {
                ClothModelsCache[model.Id] = model;
            }
        }

        public void AddOrUpdateClothModel(ClothModels model)
        {
            Console.WriteLine(model.Id);
            ClothModelsCache[model.Id] = model;
        }
    }
}
