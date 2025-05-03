using System.Threading.Tasks;
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
        AdminPanels AdminPanels { get; set; }
        public Cloth(IGameAPI api) : base(api)
        {
            PluginInformations = new PluginInformations(AssemblyHelper.GetName(), "1.0.0", "Noah");
            AdminPanels = new AdminPanels(this);
        }

        public override void OnPluginInit()
        {
            base.OnPluginInit();

            PopulateDb();
            GenerateCommands();

            ModKit.Internal.Logger.LogSuccess($"{PluginInformations.SourceName} v{PluginInformations.Version}", "initialisé");
        }


        /// <summary>
        /// Initializes the database
        /// </summary>
        public async Task InitDatabase()
        {
            Orm.RegisterTable<ClothModel>();
            Orm.RegisterTable<ClothItem>();

            await Task.Delay(500);
        }

        /// <summary>
        /// Initializes the list of basic clothing items in the game from available data.
        /// </summary>
        private async Task InitClothModels()
        {
            foreach (BuyableCloth c in Nova.server.buyableCloths)
            {
                Utils.ClothUtils.BaseClothing.Add(
                    new Utils.ClothUtils.ClothModelStruct(c.clothId, c.clothType, c.sexId, c.name, c.price)
                );
            }

            await Task.CompletedTask;
        }

        /// <summary>
        /// Adds native clothing items to the database if the table is empty.
        /// </summary>
        private async void PopulateDb()
        {
            await InitDatabase();
            await InitClothModels();

            var result = await ClothModel.QueryAll();

            if (result.Count == 0)
            {
                foreach (Utils.ClothUtils.ClothModelStruct c in Utils.ClothUtils.BaseClothing)
                {
                    ClothModel model = new ClothModel();
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
            new SChatCommand("/cloth", new string[] { "/cloth" }, "Permet d'ouvrir le panel du plugin \"Cloth\"", "/cloth", (player, arg) =>
            {
                if (player.IsAdmin) AdminPanels.AdminMenuPanel(player);
                else player.Notify("Cloth", "Vous n'avez pas la permission requise.", NotificationManager.Type.Warning);
            }).Register();
        }

    }
}