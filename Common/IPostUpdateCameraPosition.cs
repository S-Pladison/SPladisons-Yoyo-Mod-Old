namespace SPladisonsYoyoMod.Common
{
    public interface IPostUpdateCameraPosition
    {
        /// <summary>
        /// Useful for things like additional drawing of additive/pixelated things or primitives
        /// </summary>
        void PostUpdateCameraPosition();
    }
}
