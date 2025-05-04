using System.Collections.Generic;
using System.Threading.Tasks;
using Cloth.Cache;
using Cloth.Config;
using Cloth.Entities;
using Cloth.Panels;
using Life;
using Life.Network;
using ModKit.Helper;
using ModKit.Interfaces;
using ModKit.Utils;

namespace Cloth
{
    public class Cloth : ModKit.ModKit
    {
        public static CacheManager CacheManager { get; set; }
        MainPanel MainPanel { get; set; }
        
        public Cloth(IGameAPI api) : base(api)
        {
            PluginInformations = new PluginInformations(AssemblyHelper.GetName(), "1.0.0", "Noah");
            CacheManager = new CacheManager();
            MainPanel = new MainPanel(this);
        }

        public async override void OnPluginInit()
        {
            base.OnPluginInit();

            await InitConfiguration();
            await InitDatabase();
            await InitClothModels();
            await PopulateDb();       
            await CacheManager.InitializeCacheAsync();

            GenerateCommands();

            ModKit.Internal.Logger.LogSuccess($"{PluginInformations.SourceName} v{PluginInformations.Version}", "initialisé");
        }

        /// <summary>
        /// Initializes the configuration : https://github.com/Aarnow/NovaLife_ModKit-Releases/wiki/JsonHelper
        /// </summary>
        public async Task InitConfiguration()
        {
            var tasks = new List<Task>
            {
                Task.Run(() => ModKit.Helper.JsonHelper.JsonHelper.RegisterJson<ClothsConfig>()),
            };

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Initializes the database
        /// </summary>
        public async Task InitDatabase()
        {
            var tasks = new List<Task>
            {
                Task.Run(() => Orm.RegisterTable<ClothModels>()),
                Task.Run(() => Orm.RegisterTable<ClothItems>()),
                Task.Run(() => Orm.RegisterTable<CharacterInventories>()),
                Task.Run(() => Orm.RegisterTable<AreaInventories>())
            };

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Initializes the list of basic clothing items in the game from available data.
        /// </summary>
        private async Task InitClothModels()
        {
            var task = Task.Run(() =>
            {
                foreach (BuyableCloth c in Nova.server.buyableCloths)
                {
                    Utils.ClothUtils.BaseClothing.Add(
                        new Utils.ClothUtils.ClothModelStruct(c.clothId, c.clothType, c.sexId, c.name, c.price)
                    );
                }
            });

            await task;
        }

        /// <summary>
        /// Adds native clothing items to the database if the table is empty.
        /// </summary>
        private async Task PopulateDb()
        {
            var result = await ClothModels.QueryAll();

            if (result.Count == 0)
            {
                foreach (Utils.ClothUtils.ClothModelStruct c in Utils.ClothUtils.BaseClothing)
                {
                    ClothModels model = new ClothModels();
                    model.ClothId = c.ClothId;
                    model.ClothType = c.ClothType;
                    model.SexId = c.SexId;
                    model.Name = c.Name;
                    model.Price = c.Price;
                    model.CreatedAt = DateUtils.GetCurrentTime();
                    await model.Save();
                }
            }
        }

        /// <summary>
        /// Generates and registers the plugin commands.
        /// </summary>
        public void GenerateCommands()
        {
            new SChatCommand("/cloths", new string[] { "/c" }, "Permet d'ouvrir le panel du plugin \"Cloths\"", "/cloths", (player, arg) =>
            {
                MainPanel.MenuPanel(player);
            }).Register();
        }

    }
}