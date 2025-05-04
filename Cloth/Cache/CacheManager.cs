using System;
using System.Threading.Tasks;
using ModKit.Internal;

namespace Cloth.Cache
{
    public class CacheManager
    {
        public ClothModelsCache ClothModelsCache { get; set; }


        public CacheManager()
        {
            ClothModelsCache = new ClothModelsCache();
        }

        public async Task InitializeCacheAsync()
        {
            try
            {
                await ClothModelsCache.RefreshClothModelsCache();
            } catch (Exception ex)
            {
                Logger.LogError("InitializeCacheAsync", ex.Message);
            }
        } 
    }
}
