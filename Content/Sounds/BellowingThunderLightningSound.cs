using Microsoft.Xna.Framework.Audio;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content.Sounds
{
    public class BellowingThunderLightningSound : ModSound
    {
        public override SoundEffectInstance PlaySound(ref SoundEffectInstance soundInstance, float volume, float pan)
        {
            int num = Main.rand.Next(SoundEngine.LegacySoundPlayer.SoundThunder.Length);

            for (int i = 0; i < SoundEngine.LegacySoundPlayer.SoundThunder.Length; i++)
            {
                if (SoundEngine.LegacySoundPlayer.SoundInstanceThunder[num] == null || SoundEngine.LegacySoundPlayer.SoundInstanceThunder[num].State > SoundState.Playing)
                {
                    break;
                }

                num = Main.rand.Next(SoundEngine.LegacySoundPlayer.SoundThunder.Length);
            }

            if (SoundEngine.LegacySoundPlayer.SoundInstanceThunder[num] != null)
            {
                SoundEngine.LegacySoundPlayer.SoundInstanceThunder[num].Stop();
            }

            SoundEngine.LegacySoundPlayer.SoundInstanceThunder[num] = SoundEngine.LegacySoundPlayer.SoundThunder[num].Value.CreateInstance();
            SoundEngine.LegacySoundPlayer.SoundInstanceThunder[num].Volume = volume;
            SoundEngine.LegacySoundPlayer.SoundInstanceThunder[num].Pan = pan;
            SoundEngine.LegacySoundPlayer.SoundInstanceThunder[num].Pitch = (float)Main.rand.Next(-10, 11) * 0.01f;
            soundInstance = SoundEngine.LegacySoundPlayer.SoundInstanceThunder[num];

            return soundInstance;
        }
    }
}
