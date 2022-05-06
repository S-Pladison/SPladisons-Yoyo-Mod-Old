using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content.Items.Weapons
{
    public class PrototypeF34 : YoyoItem
    {
        public PrototypeF34() : base(gamepadExtraRange: 15) { }

        public override void YoyoSetDefaults()
        {
            Item.damage = 43;
            Item.knockBack = 2.5f;

            Item.shoot = ModContent.ProjectileType<PrototypeF34Projectile>();

            Item.rare = ItemRarityID.Green;
            Item.value = Terraria.Item.sellPrice(platinum: 0, gold: 1, silver: 50, copper: 0);
        }
    }

    public class PrototypeF34Projectile : YoyoProjectile
    {
        public PrototypeF34Projectile() : base(lifeTime: -1f, maxRange: 300f, topSpeed: 13f) { }

        public override void YoyoSetDefaults()
        {
            Projectile.tileCollide = false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // Отрисовываем йо-йо
            // Далее поверх него типа свет. маску с шейдером,
            // в который передаются две текстуры Main.instance.tilemap и еще одна
            // Вродь как можно матрицу для отельной текстуры перекинуть прям в шейдер,
            // поэтому мы не рендерим текстуру, и пихаем ее, а юзаем с матрицами сразу эти две
            return false;
        }
    }

    public class PrototypeF34DrawSystem : ModSystem
    {
        public void DrawOverProjectiles()
        {

        }
    }
}