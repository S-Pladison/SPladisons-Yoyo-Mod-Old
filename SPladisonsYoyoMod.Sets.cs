using Terraria.ID;

namespace SPladisonsYoyoMod
{
    public partial class SPladisonsYoyoMod
    {
        public class Sets
        {
            public static float?[] ItemCustomInventoryScale = ItemID.Sets.Factory.CreateCustomSet<float?>(null);

            public static void Unload()
            {
                ItemCustomInventoryScale = null;
            }
        }
    }
}