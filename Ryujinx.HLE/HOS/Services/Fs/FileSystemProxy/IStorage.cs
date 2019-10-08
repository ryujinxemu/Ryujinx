using LibHac;
using Ryujinx.HLE.HOS.Ipc;

namespace Ryujinx.HLE.HOS.Services.Fs.FileSystemProxy
{
    class IStorage : IpcService
    {
        private LibHac.Fs.IStorage _baseStorage;

        public IStorage(LibHac.Fs.IStorage baseStorage)
        {
            _baseStorage = baseStorage;
        }

        [Command(0)]
        // Read(u64 offset, u64 length) -> buffer<u8, 0x46, 0> buffer
        public ResultCode Read(ServiceCtx context)
        {
            long offset = context.RequestData.ReadInt64();
            long size   = context.RequestData.ReadInt64();

            if (context.Request.ReceiveBuff.Count > 0)
            {
                IpcBuffDesc buffDesc = context.Request.ReceiveBuff[0];

                // Use smaller length to avoid overflows.
                if (size > buffDesc.Size)
                {
                    size = buffDesc.Size;
                }

                byte[] data = new byte[size];

                Result rc = _baseStorage.Read(offset, data);

                context.Memory.WriteBytes(buffDesc.Position, data);

                return (ResultCode)rc.Value;
            }

            return ResultCode.Success;
        }

        [Command(4)]
        // GetSize() -> u64 size
        public ResultCode GetSize(ServiceCtx context)
        {
            Result rc = _baseStorage.GetSize(out long size);

            context.ResponseData.Write(size);

            return (ResultCode)rc.Value;
        }
    }
}