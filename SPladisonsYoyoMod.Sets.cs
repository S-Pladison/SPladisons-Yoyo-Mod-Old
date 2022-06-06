using Terraria.ID;

namespace SPladisonsYoyoMod
{
    public partial class SPladisonsYoyoMod
    {
        public class Sets
        {
            // Items
            public static float?[] ItemCustomInventoryScale = ItemID.Sets.Factory.CreateCustomSet<float?>(null);

            // Projectiles
            public static bool[] IsSoloYoyoProjectile = ProjectileID.Sets.Factory.CreateBoolSet();

            // ...
            public static void Unload()
            {
                ItemCustomInventoryScale = null;

                IsSoloYoyoProjectile = null;
            }
        }
    }
}