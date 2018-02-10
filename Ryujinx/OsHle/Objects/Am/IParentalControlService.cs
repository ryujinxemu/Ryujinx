using Ryujinx.OsHle.Ipc;
using System.Collections.Generic;

namespace Ryujinx.OsHle.Objects.Am
{
    class IParentalControlService : IIpcInterface
    {
        private Dictionary<int, ServiceProcessRequest> m_Commands;

        public IReadOnlyDictionary<int, ServiceProcessRequest> Commands => m_Commands;

        public IParentalControlService()
        {
            m_Commands = new Dictionary<int, ServiceProcessRequest>()
            {
                //...
            };
        }
    }
}