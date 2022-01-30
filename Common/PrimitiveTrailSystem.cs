using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Common
{
    public class PrimitiveTrailSystem : ModSystem
    {
        internal static readonly List<Trail> AdditiveTrails = new List<Trail>();
        internal static readonly List<Trail> AlphaBlendTrails = new List<Trail>();

        public static PrimitiveTrailSystem Instance { get => ModContent.GetInstance<PrimitiveTrailSystem>(); }

        public Asset<Effect> BasicPrimitiveEffect { get; private set; }
        public Matrix TransformMatrix { get; private set; }

        // ...

        public override void Load()
        {
            if (Main.dedServ) return;

            BasicPrimitiveEffect = ModAssets.GetEffect("Primitive");
            UpdateTransformMatrix();
        }

        public override void Unload()
        {
            BasicPrimitiveEffect = null;
        }

        public override void PostUpdateEverything()
        {
            foreach (var trail in AdditiveTrails.ToArray()) trail.Update();
            foreach (var trail in AlphaBlendTrails.ToArray()) trail.Update();
        }

        public void UpdateTransformMatrix()
        {
            Matrix m1 = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up) * Main.GameViewMatrix.EffectMatrix * Matrix.CreateTranslation(Main.screenWidth / 2, -Main.screenHeight / 2, 0) * Matrix.CreateRotationZ(MathHelper.Pi);
            Matrix m3 = Matrix.CreateOrthographic(Main.screenWidth, Main.screenHeight, 0, 1000);
            TransformMatrix = m1 * Matrix.CreateScale(Main.GameViewMatrix.Zoom.X, Main.GameViewMatrix.Zoom.Y, 1) * m3;
        }

        public void DrawTrails(List<Trail> trails)
        {
            foreach (var trail in trails)
            {
                try
                {
                    if (!trail.CustomDraw) trail.Draw(Main.spriteBatch, TransformMatrix);
                }
                catch (Exception e)
                {
                    TimeLogger.DrawException(e);
                    trail.Active = false;
                }
            }
        }

        public static void NewTrail(Trail trail)
        {
            if (Main.dedServ) return;

            if (trail.Additive) AdditiveTrails.Add(trail);
            else AlphaBlendTrails.Add(trail);

            trail.OnSpawn();
        }

        // ...

        public abstract class Trail
        {
            public bool Active { get; set; } = true;
            public bool CustomDraw { get; set; } = false;
            public bool Additive { get; protected set; }
            public float Length { get; protected set; }

            protected readonly int _maxLength;
            protected readonly List<Vector2> _points = new();
            protected readonly List<VertexPositionColorTexture> _vertices = new();

            protected bool _dissolving = false;
            protected float _dissolveSpeed = 0.1f;
            protected float _dissolveProgress = 1f;

            protected int _maxPoints = 25;
            protected Entity _target = null;
            protected Func<Entity, Vector2> _customPositionMethod = null;

            private readonly Asset<Effect> _effect;

            // ...

            public Trail(Entity target, int length, Asset<Effect> effect = null, bool additive = false)
            {
                _target = target;
                _maxLength = length;

                _effect = effect ?? PrimitiveTrailSystem.Instance.BasicPrimitiveEffect;
                _effect.Value.Parameters["texture0"].SetValue(ModAssets.GetExtraTexture(6).Value);

                this.Additive = additive;
            }

            public void Update()
            {
                if (_target == null || !_target.active || !Active)
                {
                    StartDissolving();
                    _target = null;
                }

                int length = _maxLength;

                if (_dissolving)
                {
                    _dissolveProgress -= _dissolveSpeed;
                    length = (int)(length * _dissolveProgress);

                    if (_dissolveProgress <= 0) Kill();
                }

                UpdateLength(maxLength: length);
            }

            public void Draw(SpriteBatch spriteBatch, Matrix matrix)
            {
                if (_points.Count <= 1) return;

                CreateMesh();
                ApplyGlobalEffectParameters(effect: _effect.Value, matrix: matrix);
                ApplyEffectParameters(effect: _effect.Value);

                var graphics = spriteBatch.GraphicsDevice;
                foreach (var pass in _effect.Value.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    graphics.DrawUserPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleList, _vertices.ToArray(), 0, (_points.Count - 1) * 2 + ExtraTrianglesCount);
                }

                _vertices.Clear();
            }

            public void Kill()
            {
                if (PreKill()) Active = false;

                if (Additive) AdditiveTrails.Remove(this);
                else AlphaBlendTrails.Remove(this);
            }

            public void StartDissolving() => _dissolving = true;
            public void SetEffectTexture(Texture2D texture, int index = 0) => _effect.Value.Parameters["texture" + index].SetValue(texture);
            public void SetDissolveSpeed(float speed = 0.1f) => _dissolveSpeed = speed;
            public void SetMaxPoints(int value = 25) => _maxPoints = Math.Max(value, 2);
            public void SetCustomPositionMethod(Func<Entity, Vector2> method) => _customPositionMethod = method;
            public virtual void OnSpawn() { }

            // ...

            protected void UpdateLength(int maxLength)
            {
                Length = 0;

                if (!_dissolving) _points.Insert(0, _customPositionMethod?.Invoke(_target) ?? _target.Center);
                if (_points.Count <= 1) return;
                if (_points.Count > _maxPoints) _points.Remove(_points.Last());

                int lastIndex = -1;
                for (int i = 1; i < _points.Count; i++)
                {
                    float dist = Vector2.Distance(_points[i], _points[i - 1]);

                    Length += dist;
                    if (Length > maxLength)
                    {
                        lastIndex = i;
                        Length -= dist;
                        break;
                    }
                }
                if (lastIndex < 0) return;

                var vector = Vector2.Normalize(_points[lastIndex] - _points[lastIndex - 1]) * (maxLength - this.Length);
                _points.RemoveRange(lastIndex, _points.Count - lastIndex);
                _points.Add(_points[lastIndex - 1] + vector);

                Length = maxLength;
            }

            protected void AddVertex(Vector2 position, Color color, Vector2 uv)
            {
                Vector3 pos = new Vector3(position.X - Main.screenPosition.X, position.Y - Main.screenPosition.Y, 0);
                _vertices.Add(new VertexPositionColorTexture(pos, color, uv));
            }

            protected virtual int ExtraTrianglesCount => 0;
            protected virtual bool PreKill() => true;
            protected virtual void CreateMesh() { }
            protected virtual void ApplyEffectParameters(Effect effect) { }

            // ...

            private static void ApplyGlobalEffectParameters(Effect effect, Matrix matrix)
            {
                effect.Parameters["transformMatrix"].SetValue(matrix);
                foreach (var param in effect.Parameters)
                {
                    if (param.Name == "time") effect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly);
                }
            }

            public delegate float WidthDelegate(float progress);
            public delegate Color ColorDelegate(float progress);
        }
    }
}
