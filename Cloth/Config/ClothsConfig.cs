using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloth.Config
{
    /// <summary>
    /// Config data : https://github.com/Aarnow/NovaLife_ModKit-Releases/wiki/JsonHelper
    /// </summary>
    public class ClothsConfig : ModKit.Helper.JsonHelper.JsonEntity<ClothsConfig>
    {
        public string Nom = "Test";
    }
}
