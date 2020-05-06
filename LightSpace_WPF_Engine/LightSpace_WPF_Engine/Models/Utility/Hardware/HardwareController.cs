namespace LightSpace_WPF_Engine.Models.Utility.Hardware
{
    public abstract class HardwareController
    {
        public delegate void ReadEvent();
        public readonly ReadEvent Read;

        public delegate void WriteEvent();
        public readonly WriteEvent Write;

        protected HardwareController()
        {
            Read += ReadData;
            Write += WriteData;
        }

        protected abstract void WriteData();

        protected abstract void ReadData();
    }
}
