using System;

namespace Ryujinx.Core.OsHle.Handles
{
    class KProcessHandleTable : IDisposable
    {
        private IdDictionary Handles;

        public KProcessHandleTable()
        {
            Handles = new IdDictionary();
        }

        public int OpenHandle(object Obj)
        {
            int h = Handles.Add(Obj);

            /*if (h == 0x1d)
            {
                throw new System.Exception("bad handle");
            }*/

            return h;
        }

        public T GetData<T>(int Handle)
        {
            return Handles.GetData<T>(Handle);
        }

        public bool CloseHandle(int Handle)
        {
            object Data = Handles.GetData(Handle);

            if (Data is HTransferMem TMem)
            {
                TMem.Memory.Manager.Reprotect(
                    TMem.Position,
                    TMem.Size,
                    TMem.Perm);
            }

            return Handles.Delete(Handle);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool Disposing)
        {
            if (Disposing)
            {
                foreach (object Obj in Handles)
                {
                    if (Obj is IDisposable DisposableObj)
                    {
                        DisposableObj.Dispose();
                    }
                }
            }
        }
    }
}