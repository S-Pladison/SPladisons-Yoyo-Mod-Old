using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;

namespace SPladisonsYoyoMod.Common.Primitives.Trails
{
    public class PrimitiveTrail
    {
        public static PrimitiveTrail Create(Entity entity, PrimitiveDrawLayer layer = PrimitiveDrawLayer.SolidTiles)
        {
            PrimitiveTrail trail = new(entity, layer);
            return AddTrailToList(trail);
        }

        public static PrimitiveTrail Create(Entity entity, Action<PrimitiveTrail> action, PrimitiveDrawLayer layer = PrimitiveDrawLayer.SolidTiles)
        {
            PrimitiveTrail trail = new(entity, layer);
            action?.Invoke(trail);
            return AddTrailToList(trail);
        }

        private static PrimitiveTrail AddTrailToList(PrimitiveTrail trail)
        {
            PrimitiveSystem.Instance.AddTrail(trail);
            return trail;
        }

        // ...

        public Entity Target { get; protected set; }
        public Asset<Effect> Effect { get; protected set; }
        public PrimitiveDrawLayer PrimitiveDrawLayer { get; protected set; }
        public List<Vector2> Positions { get; protected set; }
        public float DissolveProgress { get; protected set; }

        protected readonly List<VertexPositionColorTexture> vertices;

        protected IPrimitiveTrailColor color;
        protected IPrimitiveTrailTip tip;
        protected IPrimitiveTrailUpdate update;
        protected IPrimitiveTrailWidth width;

        protected bool dissolving = false;
        protected float dissolveSpeed = 0.2f;

        private PrimitiveTrail(Entity entity, PrimitiveDrawLayer layer)
        {
            Target = entity;
            PrimitiveDrawLayer = layer;
            Positions = new();
            DissolveProgress = 1f;

            vertices = new();

            color = new DefaultTrailColor(Color.White);
            tip = new WithoutTrailTip();
            update = new DefaultTrailUpdate(25);
            width = new DefaultTrailWidth(0);

            Effect = ModAssets.GetEffect("Primitive", AssetRequestMode.ImmediateLoad);
            Effect.Value.Parameters["texture0"].SetValue(TextureAssets.MagicPixel.Value);
        }

        // ...

        public PrimitiveTrail SetDrawLayer(PrimitiveDrawLayer layer)
        {
            this.PrimitiveDrawLayer = layer;
            return this;
        }

        public PrimitiveTrail SetColor(IPrimitiveTrailColor color)
        {
            if (color == null) return this;

            this.color = color;
            return this;
        }

        public PrimitiveTrail SetColor(CustomTrailColor.ColorDelegate color)
        {
            if (color == null) return this;

            this.color = new CustomTrailColor(color);
            return this;
        }

        public PrimitiveTrail SetTip(IPrimitiveTrailTip tip)
        {
            if (tip == null) return this;

            this.tip = tip;
            return this;
        }

        public PrimitiveTrail SetUpdate(IPrimitiveTrailUpdate update)
        {
            if (update == null) return this;

            this.update = update;
            return this;
        }

        public PrimitiveTrail SetWidth(IPrimitiveTrailWidth width)
        {
            if (width == null) return this;

            this.width = width;
            return this;
        }

        public PrimitiveTrail SetWidth(CustomTrailWidth.WidthDelegate width)
        {
            if (width == null) return this;

            this.width = new CustomTrailWidth(width);
            return this;
        }

        public PrimitiveTrail SetEffect(Asset<Effect> effect)
        {
            if (effect == null || effect.Value == null) return this;

            this.Effect = effect;
            return this;
        }

        public PrimitiveTrail SetEffectTexture(Texture2D texture, int index = 0)
        {
            if (texture == null) return this;

            Effect.Value.Parameters["texture" + index].SetValue(texture);
            return this;
        }

        public PrimitiveTrail SetDissolveSpeed(float speed = 0.2f)
        {
            dissolveSpeed = MathF.Max(0.001f, speed);
            return this;
        }

        // ...

        public void StartDissolving()
        {
            dissolving = true;
        }

        public void Update()
        {
            if (!dissolving && (Target == null || !Target.active))
            {
                StartDissolving();
            }

            if (dissolving)
            {
                DissolveProgress -= dissolveSpeed;

                if (DissolveProgress <= 0)
                {
                    Kill();
                    return;
                }
            }

            update.UpdatePoints(this);
        }

        public void Draw()
        {
            if (Positions.Count < 2) return;

            CreateMesh();
            PrimitiveSystem.DrawPrimitives(new PrimitiveDrawData(PrimitiveDrawLayer, PrimitiveType.TriangleList, (Positions.Count - 1) * 2 + tip.ExtraTriangles, vertices.ToArray(), Effect));
            vertices.Clear();
        }

        public void Kill()
        {
            OnKill();
            PrimitiveSystem.Instance.RemoveTrail(this);
        }

        public void AddVertex(Vector2 position, Color color, Vector2 uv)
        {
            Vector3 pos = new(position.X - Main.screenPosition.X, position.Y - Main.screenPosition.Y, 0);
            vertices.Add(new VertexPositionColorTexture(pos, color, uv));
        }

        // ...

        protected (Color, Vector2) GetProgressVariables(float progress, int index = 1)
        {
            float width = this.width.GetWidth(progress, 1f);
            Color color = this.color.GetColor(progress) * DissolveProgress;
            Vector2 normal = (Positions[index] - Positions[index - 1]).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2) * width / 2f;

            return (color, normal);
        }

        protected virtual void CreateMesh()
        {
            float length = 0;
            for (int i = 1; i < Positions.Count; i++)
            {
                length += Vector2.Distance(Positions[i - 1], Positions[i]);
            }

            float progress = 0f;
            (Color color, Vector2 normal) = GetProgressVariables(progress);

            tip.CreateTipMesh(this, Positions[0], normal, color);

            for (int i = 1; i < Positions.Count; i++)
            {
                float nextProgress = progress + Vector2.Distance(Positions[i], Positions[i - 1]) / length;
                (Color nextColor, Vector2 nextNormal) = GetProgressVariables(nextProgress, i);

                AddVertex(Positions[i - 1] - normal, color, new Vector2(progress, 0));
                AddVertex(Positions[i] - nextNormal, nextColor, new Vector2(nextProgress, 0));
                AddVertex(Positions[i] + nextNormal, nextColor, new Vector2(nextProgress, 1));

                AddVertex(Positions[i - 1] - normal, color, new Vector2(progress, 0));
                AddVertex(Positions[i] + nextNormal, nextColor, new Vector2(nextProgress, 1));
                AddVertex(Positions[i - 1] + normal, color, new Vector2(progress, 1));

                progress = nextProgress;
                color = nextColor;
                normal = nextNormal;
            }
        }

        protected virtual void OnKill() { }
    }
}
