using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Common.Misc
{
    public class SoulFilledFlameEffect : ILoadable
    {
        public static SoulFilledFlameEffect Instance => ModContent.GetInstance<SoulFilledFlameEffect>();
        public static Mod Mod { get; private set; }

        private Asset<Texture2D> _particleTexture;
        private List<Particle> _particles;
        private List<ISoulFilledFlame> _elements;

        public void Load(Mod mod)
        {
            Mod = mod;

            _particles = new List<Particle>();
            _elements = new List<ISoulFilledFlame>();

            _particleTexture = ModContent.Request<Texture2D>("SPladisonsYoyoMod/Assets/Textures/Dusts/SoulFilledFlameDust");
        }

        public void Unload()
        {
            Mod = null;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.DepthRead, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            foreach (var p in _particles)
            {
                Vector2 pos = p.position - Main.screenPosition;
                spriteBatch.Draw(_particleTexture.Value, pos, null, p.color, p.rotation, _particleTexture.Size() * 0.5f, p.scale, SpriteEffects.None, p.layerDepth);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            foreach (var elem in _elements) elem.DrawSoulFilledFlame(spriteBatch);

            spriteBatch.End();
        }

        public void Render(SpriteBatch spriteBatch)
        {
            if (_elements.Count > 0)
            {

            }
        }

        public void AddElement(ISoulFilledFlame element)
        {
            if (!_elements.Contains(element)) _elements.Add(element);
        }

        public void RemoveElement(ISoulFilledFlame element)
        {
            if (_elements.Contains(element)) _elements.Remove(element);
        }

        public Particle CreateParticle(Vector2 position, float scale, float timeLeft, Color color, Action<Particle> updateAction = null, float layerDepth = 0)
        {
            Particle particle = new Particle(position, scale, timeLeft, color, updateAction, layerDepth);
            _particles.Add(particle);
            return particle;
        }

        public void RemoveParticle(Particle particle)
        {
            if (_particles.Contains(particle)) _particles.Remove(particle);
        }

        public void UpdateParticles()
        {
            foreach (var p in _particles.ToList()) p.Update();
        }
    }
}
