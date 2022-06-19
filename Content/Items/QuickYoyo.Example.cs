using Terraria.ID;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content.Items
{
    /*
     
        | Pre-Hardmode
        -----------------------------------------------------------------------------------------------
        • Wooden Yoyo        | gamepadExtraRange: 4  | lifeTime: 3f  | maxRange: 130f | topSpeed: 9f
        • Rally              | gamepadExtraRange: 6  | lifeTime: 5f  | maxRange: 170f | topSpeed: 11f
        • Malaise            | gamepadExtraRange: 8  | lifeTime: 7f  | maxRange: 195f | topSpeed: 12.5f
        • Artery             | gamepadExtraRange: 8  | lifeTime: 6f  | maxRange: 207f | topSpeed: 12f
        • Amazon             | gamepadExtraRange: 9  | lifeTime: 8f  | maxRange: 215f | topSpeed: 13f
        • Code 1             | gamepadExtraRange: 10 | lifeTime: 9f  | maxRange: 220f | topSpeed: 13f
        • Valor              | gamepadExtraRange: 10 | lifeTime: 11f | maxRange: 225f | topSpeed: 14f
        • Cascade            | gamepadExtraRange: 10 | lifeTime: 13f | maxRange: 235f | topSpeed: 14f
        
        | Hardmode
        -----------------------------------------------------------------------------------------------
        • Chik               | gamepadExtraRange: 12 | lifeTime: 16f | maxRange: 275f | topSpeed: 17f
        • Format:C           | gamepadExtraRange: 10 | lifeTime: 8f  | maxRange: 235f | topSpeed: 15f
        • Hel-Fire           | gamepadExtraRange: 13 | lifeTime: 12f | maxRange: 275f | topSpeed: 15f
        • Amarok             | gamepadExtraRange: 11 | lifeTime: 15f | maxRange: 270f | topSpeed: 14f
        • Gradient           | gamepadExtraRange: 11 | lifeTime: 10f | maxRange: 250f | topSpeed: 12f
        • Code 2             | gamepadExtraRange: 13 | lifeTime: -   | maxRange: 280f | topSpeed: 17f
        • Yelets             | gamepadExtraRange: 13 | lifeTime: 14f | maxRange: 290f | topSpeed: 16f
        • Red's Throw        | gamepadExtraRange: 18 | lifeTime: -   | maxRange: 370f | topSpeed: 16f
        • Valkyrie Yoyo      | gamepadExtraRange: 18 | lifeTime: -   | maxRange: 370f | topSpeed: 16f
        • Kraken             | gamepadExtraRange: 17 | lifeTime: -   | maxRange: 340f | topSpeed: 16f
        • The Eye of Cthulhu | gamepadExtraRange: 18 | lifeTime: -   | maxRange: 360f | topSpeed: 16.5f
        • Terrarian          | gamepadExtraRange: 21 | lifeTime: -   | maxRange: 400f | topSpeed: 17.5f

    */

    [Autoload(false)]
    public class ExampleYoyo : YoyoItem
    {
        public ExampleYoyo() : base(gamepadExtraRange: 15) { }

        public override void YoyoSetDefaults()
        {
            Item.damage = 43;
            Item.knockBack = 2.5f;

            Item.shoot = ModContent.ProjectileType<ExampleYoyoProjectile>();

            Item.rare = ItemRarityID.Lime;
            Item.value = Terraria.Item.sellPrice(platinum: 0, gold: 1, silver: 50, copper: 0);
        }
    }

    [Autoload(false)]
    public class ExampleYoyoProjectile : YoyoProjectile
    {
        public ExampleYoyoProjectile() : base(lifeTime: -1f, maxRange: 300f, topSpeed: 13f) { }
    }
}