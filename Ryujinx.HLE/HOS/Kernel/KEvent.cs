namespace Ryujinx.HLE.HOS.Kernel
{
    class KEvent
    {
        public KReadableEvent ReadableEvent { get; private set; }
        public KWritableEvent WritableEvent { get; private set; }

        public KEvent(Horizon system)
        {
            ReadableEvent = new KReadableEvent(system, this);
            WritableEvent = new KWritableEvent(this);
        }
    }
}