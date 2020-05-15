namespace LightSpace_WPF_Engine.Models.Utility.Hardware
{
    public abstract class BaseHardwareController
    {
        public bool NoHardwareMode;
        public TileManager TileManager;

        public delegate void ReadEvent();
        public readonly ReadEvent Read;

        public delegate void WriteEvent();
        public readonly WriteEvent Write;

        protected BaseHardwareController(TileManager tileManager)
        {
            TileManager = tileManager;
            Read += ReadData;
            Write += WriteData;
        }

        protected abstract void WriteData();

        protected abstract void ReadData();
    }
}
