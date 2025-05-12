using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Clothes.Cache;
using Clothes.Config;
using Clothes.Entities;
using Clothes.Events;
using Clothes.Panels;
using Clothes.Points;
using Life;
using Life.Network;
using ModKit.Helper;
using ModKit.Interfaces;
using ModKit.Internal;
using ModKit.Utils;

namespace Clothes
{
    public class Clothes : ModKit.ModKit
    {
        private EventManager EventManager { get; }
        public static CacheManager CacheManager { get; set; }
        
        MainPanel MainPanel { get; }
        
        public Clothes(IGameAPI api) : base(api)
        {
            PluginInformations = new PluginInformations(AssemblyHelper.GetName(), "1.0.0", "MODNL");
            EventManager = new EventManager();
            CacheManager = new CacheManager();
            MainPanel = new MainPanel(this);
        }

        public async override void OnPluginInit()
        {
            base.OnPluginInit();

            await InitConfiguration();
            if(await InitDatabase())
            {
                await InitClothModels();
                await PopulateDb();
            }                   
            await CacheManager.InitializeCacheAsync();

            GenerateCommands();

            Logger.LogSuccess($"{PluginInformations.SourceName} v{PluginInformations.Version}", "initialisé");
        }

        /// <summary>
        /// Initializes the configuration : https://github.com/Aarnow/NovaLife_ModKit-Releases/wiki/JsonHelper
        /// </summary>
        public async Task InitConfiguration()
        {
            var tasks = new List<Task>
            {
                Task.Run(() => ModKit.Helper.JsonHelper.JsonHelper.RegisterJson<ClothesConfig>()),
            };

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Initializes the database by registering all necessary tables.
        /// </summary>
        /// <returns>A boolean indicating whether the database initialization was successful.</returns>
        public async Task<bool> InitDatabase()
        {
            try
            {
                var tasks = new List<Task>
                {
                    Task.Run(() => Orm.RegisterTable<ClothModels>()),
                    Task.Run(() => Orm.RegisterTable<ClothItems>()),
                    Task.Run(() => Orm.RegisterTable<CharacterInventories>()),
                    Task.Run(() => Orm.RegisterTable<AreaInventories>()),
                    Task.Run(() => Orm.RegisterTable<ClothShopPoint>())
                };

                MCheckpointHelper.RegisterType(typeof(ClothShopPoint));

                await Task.WhenAll(tasks);
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError("InitDatabase", $"Erreur lors de l'initialisation de la base de données: {ex.Message}");
                return false;
            }
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