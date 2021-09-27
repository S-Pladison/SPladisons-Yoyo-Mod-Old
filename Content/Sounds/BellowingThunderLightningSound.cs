using Microsoft.Xna.Framework.Audio;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content.Sounds
{
    public class BellowingThunderLightningSound : ModSound
    {
        public override SoundEffectInstance PlaySound(ref SoundEffectInstance soundInstance, float volume, float pan, Terraria.ModLoader.SoundType type)
        {
            int num18 = Main.rand.Next(SoundEngine.LegacySoundPlayer.SoundThunder.Length);

            for (int i = 0; i < SoundEngine.LegacySoundPlayer.SoundThunder.Length; i++)
            {
                bool flag67 = SoundEngine.LegacySoundPlayer.SoundInstanceThunder[num18] == null;
                if (flag67)
                {
                    break;
                }
                bool flag68 = SoundEngine.LegacySoundPlayer.SoundInstanceThunder[num18].State > SoundState.Playing;
                if (flag68)
                {
                    break;
                }
                num18 = Main.rand.Next(SoundEngine.LegacySoundPlayer.SoundThunder.Length);
            }

            if (SoundEngine.LegacySoundPlayer.SoundInstanceThunder[num18] != null)
            {
                SoundEngine.LegacySoundPlayer.SoundInstanceThunder[num18].Stop();
            }

            SoundEngine.LegacySoundPlayer.SoundInstanceThunder[num18] = SoundEngine.LegacySoundPlayer.SoundThunder[num18].Value.CreateInstance();
            SoundEngine.LegacySoundPlayer.SoundInstanceThunder[num18].Volume = volume;
            SoundEngine.LegacySoundPlayer.SoundInstanceThunder[num18].Pan = pan;
            SoundEngine.LegacySoundPlayer.SoundInstanceThunder[num18].Pitch = (float)Main.rand.Next(-10, 11) * 0.01f;
            soundInstance = SoundEngine.LegacySoundPlayer.SoundInstanceThunder[num18];

            return soundInstance;
        }
    }
}
