using System.Threading.Tasks;
using Life.Network;
using ModKit.Helper.CheckpointHelper;
using ModKit.ORM;
using SQLite;
using System.Collections.Generic;
using System.Collections;
using ModKit.Helper;
using Life.UI;

namespace Clothes.Points
{
    public class ClothShopPoint : ModEntity<ClothShopPoint>, IMCheckpoint
    {
        [AutoIncrement][PrimaryKey] public int Id { get; set; }

        public string ClothModelList { get; set; } = "[]";

        [Ignore]
        public List<int> ClothModelIds
        {
            get => ListConverter.ReadJson(ClothModelList);
            set => ClothModelList = ListConverter.WriteJson(value);
        }

        #region NE PAS TOUCHER
        public string Name { get; set; } = "Default";

        public string MCheckpointIdList { get; set; } = "[]";

        [Ignore]
        public List<int> MCheckpointIds
        {
            get => ListConverter.ReadJson(MCheckpointIdList);
            set => MCheckpointIdList = ListConverter.WriteJson(value);
        }

        [Ignore] public ModKit.ModKit Context { get; set; }

        public ClothShopPoint() { }
        public ClothShopPoint(ModKit.ModKit context)
        {
            Context = context;
        }
        #endregion

        private void OnPlayerTrigger(Player player)
        {
            ClothShopMenuPanel(player);
        }

        public void ClothShopMenuPanel(Player player)
        {
            Panel panel = Context.PanelHelper.Create($"{Name}", UIPanel.PanelType.Tab, player, () => ClothShopMenuPanel(player));


            panel.NextButton("Sélectionner", () => panel.SelectTab());
            if (player.IsAdmin && player.serviceAdmin) panel.NextButton("Configurer", () => panel.SelectTab());
            panel.CloseButton();

            panel.Display();
        }

        #region NE PAS TOUCHER
        public async Task<bool> Create(Player player)
        {
            return await MCheckpoint.Create(player, OnPlayerTrigger, this);
        }

        public void Generate(Player player, List<MCheckpoint> mCheckpoints)
        {
            MCheckpoint.Generate(player, OnPlayerTrigger, mCheckpoints, this);
        }


        public async Task<IList> QueryAllHandler()
        {
            List<ClothShopPoint> result = await QueryAll();
            return result;
        }
        #endregion
    }
}
