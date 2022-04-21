namespace SPladisonsYoyoMod
{
    public partial class SPladisonsYoyoMod
    {
        public delegate void PostUpdateCameraPositionDelegate();
        public static event PostUpdateCameraPositionDelegate PostUpdateCameraPositionEvent;
        public static void PostUpdateCameraPosition()
        {
            PostUpdateCameraPositionEvent?.Invoke();
        }

        // ...

        private static void UnloadEvents()
        {
            PostUpdateCameraPositionEvent = null;
        }
    }
}