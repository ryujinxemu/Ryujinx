﻿namespace Ryujinx.HLE.HOS.Services.Set
{
    enum ResultCode
    {
        ModuleId       = 105,
        ErrorCodeShift = 9,

        Success = 0,

        LanguageOutOfRange = (625 << ErrorCodeShift) | ModuleId
    }
}