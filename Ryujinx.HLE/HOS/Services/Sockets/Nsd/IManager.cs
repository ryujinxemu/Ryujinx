﻿using Ryujinx.Common.Logging;
using Ryujinx.Cpu;
using Ryujinx.HLE.Exceptions;
using Ryujinx.HLE.HOS.Services.Settings;
using Ryujinx.HLE.HOS.Services.Sockets.Nsd.Manager;
using System.Runtime.InteropServices;
using System.Text;

namespace Ryujinx.HLE.HOS.Services.Sockets.Nsd
{
    [Service("nsd:a")] // Max sessions: 5
    [Service("nsd:u")] // Max sessions: 20
    class IManager : IpcService
    {
        private readonly NsdSettings  _nsdSettings;
        private readonly FqdnResolver _fqdnResolver;

        private bool _isInitialized = false;

        public IManager(ServiceCtx context)
        {
            // TODO: Load nsd settings through the savedata 0x80000000000000B0 (nsdsave:/).

            if (!NxSettings.Settings.TryGetValue("nsd!test_mode", out object testMode))
            {
                // return ResultCode.InvalidSettingsValue;
            }

            if (!NxSettings.Settings.TryGetValue("nsd!environment_identifier", out object environmentIdentifier))
            {
                // return ResultCode.InvalidSettingsValue;
            }

            _nsdSettings = new NsdSettings
            {
                Initialized = true,
                TestMode    = (bool)testMode,
                Environment = (string)environmentIdentifier
            };

            _fqdnResolver = new FqdnResolver(_nsdSettings);

            _isInitialized = true;
        }

        [CommandHipc(5)] // 11.0.0+
        // GetSettingUrl() -> buffer<unknown<0x100>, 0x16>
        public ResultCode GetSettingUrl(ServiceCtx context)
        {
            throw new ServiceNotImplementedException(this, context, false);
        }

        [CommandHipc(10)]
        // GetSettingName() -> buffer<unknown<0x100>, 0x16>
        public ResultCode GetSettingName(ServiceCtx context)
        {
            throw new ServiceNotImplementedException(this, context, false);
        }

        [CommandHipc(11)]
        // GetEnvironmentIdentifier() -> buffer<byte<8> environment_identifier, 0x16>
        public ResultCode GetEnvironmentIdentifier(ServiceCtx context)
        {
            (ulong outputPosition, ulong outputSize) = context.Request.GetBufferType0x22();

            MemoryHelper.FillWithZeros(context.Memory, outputPosition, (int)outputSize);

            ResultCode result = _fqdnResolver.GetEnvironmentIdentifier(out string identifier);

            if (result == ResultCode.Success)
            {
                byte[] identifierBuffer = Encoding.UTF8.GetBytes(identifier);

                context.Memory.Write(outputPosition, identifierBuffer);
            }

            return result;
        }

        [CommandHipc(12)]
        // GetDeviceId() -> bytes<0x10, 1>
        public ResultCode GetDeviceId(ServiceCtx context)
        {
            // NOTE: Stubbed in system module.

            return ResultCode.Success;
        }

        [CommandHipc(13)]
        // DeleteSettings(u32)
        public ResultCode DeleteSettings(ServiceCtx context)
        {
            uint unknown = context.RequestData.ReadUInt32();

            if (!_isInitialized)
            {
                return ResultCode.ServiceNotInitialized;
            }

            if (unknown > 1)
            {
                return ResultCode.InvalidArgument;
            }

            if (unknown == 1)
            {
                NxSettings.Settings.TryGetValue("nsd!environment_identifier", out object environmentIdentifier);

                if ((string)environmentIdentifier == _nsdSettings.Environment)
                {
                    // TODO: Call nn::fs::DeleteSystemFile() to delete the savedata file and return ResultCode.
                }
                else
                {
                    // TODO: Mount the savedata file and return ResultCode.
                }
            }
            else
            {
                // TODO: Call nn::fs::DeleteSystemFile() to delete the savedata file and return ResultCode.
            }

            return ResultCode.Success;
        }

        [CommandHipc(14)]
        // ImportSettings(u32, buffer<unknown, 5>) -> buffer<unknown, 6>
        public ResultCode ImportSettings(ServiceCtx context)
        {
            throw new ServiceNotImplementedException(this, context, false);
        }

        [CommandHipc(15)] // 4.0.0+
        // SetChangeEnvironmentIdentifierDisabled(bytes<1>)
        public ResultCode SetChangeEnvironmentIdentifierDisabled(ServiceCtx context)
        {
            byte disabled = context.RequestData.ReadByte();

            // TODO: When sys:set service calls will be implemented
            /*
                if (((nn::settings::detail::GetServiceDiscoveryControlSettings() ^ disabled) & 1) != 0 )
                {
                    nn::settings::detail::SetServiceDiscoveryControlSettings(disabled & 1);
                }
            */

            Logger.Stub?.PrintStub(LogClass.ServiceNsd, new { disabled });

            return ResultCode.Success;
        }

        [CommandHipc(20)]
        // Resolve(buffer<unknown<0x100>, 0x15>) -> buffer<unknown<0x100>, 0x16>
        public ResultCode Resolve(ServiceCtx context)
        {
            ulong outputPosition = context.Request.ReceiveBuff[0].Position;
            ulong outputSize     = context.Request.ReceiveBuff[0].Size;

            ResultCode result = _fqdnResolver.ResolveEx(context, out _, out string resolvedAddress);

            if ((ulong)resolvedAddress.Length > outputSize)
            {
                return ResultCode.InvalidArgument;
            }

            byte[] resolvedAddressBuffer = Encoding.UTF8.GetBytes(resolvedAddress);

            MemoryHelper.FillWithZeros(context.Memory, outputPosition, (int)outputSize);

            context.Memory.Write(outputPosition, resolvedAddressBuffer);

            return result;
        }

        [CommandHipc(21)]
        // ResolveEx(buffer<unknown<0x100>, 0x15>) -> (u32, buffer<unknown<0x100>, 0x16>)
        public ResultCode ResolveEx(ServiceCtx context)
        {
            ulong outputPosition = context.Request.ReceiveBuff[0].Position;
            ulong outputSize     = context.Request.ReceiveBuff[0].Size;

            ResultCode result = _fqdnResolver.ResolveEx(context, out ResultCode errorCode, out string resolvedAddress);

            if ((ulong)resolvedAddress.Length > outputSize)
            {
                return ResultCode.InvalidArgument;
            }

            byte[] resolvedAddressBuffer = Encoding.UTF8.GetBytes(resolvedAddress);

            MemoryHelper.FillWithZeros(context.Memory, outputPosition, (int)outputSize);

            context.Memory.Write(outputPosition, resolvedAddressBuffer);

            context.ResponseData.Write((int)errorCode);

            return result;
        }

        [CommandHipc(30)]
        // GetNasServiceSetting(buffer<unknown<0x10>, 0x15>) -> buffer<unknown<0x108>, 0x16>
        public ResultCode GetNasServiceSetting(ServiceCtx context)
        {
            throw new ServiceNotImplementedException(this, context, false);
        }

        [CommandHipc(31)]
        // GetNasServiceSettingEx(buffer<unknown<0x10>, 0x15>) -> (u32, buffer<unknown<0x108>, 0x16>)
        public ResultCode GetNasServiceSettingEx(ServiceCtx context)
        {
            throw new ServiceNotImplementedException(this, context, false);
        }

        [CommandHipc(40)]
        // GetNasRequestFqdn() -> buffer<unknown<0x100>, 0x16>
        public ResultCode GetNasRequestFqdn(ServiceCtx context)
        {
            throw new ServiceNotImplementedException(this, context, false);
        }

        [CommandHipc(41)]
        // GetNasRequestFqdnEx() -> (u32, buffer<unknown<0x100>, 0x16>)
        public ResultCode GetNasRequestFqdnEx(ServiceCtx context)
        {
            throw new ServiceNotImplementedException(this, context, false);
        }

        [CommandHipc(42)]
        // GetNasApiFqdn() -> buffer<unknown<0x100>, 0x16>
        public ResultCode GetNasApiFqdn(ServiceCtx context)
        {
            throw new ServiceNotImplementedException(this, context, false);
        }

        [CommandHipc(43)]
        // GetNasApiFqdnEx() -> (u32, buffer<unknown<0x100>, 0x16>)
        public ResultCode GetNasApiFqdnEx(ServiceCtx context)
        {
            throw new ServiceNotImplementedException(this, context, false);
        }

        [CommandHipc(50)]
        // GetCurrentSetting() -> buffer<unknown<0x12bf0>, 0x16>
        public ResultCode GetCurrentSetting(ServiceCtx context)
        {
            throw new ServiceNotImplementedException(this, context, false);
        }

        [CommandHipc(51)] // 9.0.0+
        // WriteTestParameter(buffer<?>)
        public ResultCode WriteTestParameter(ServiceCtx context)
        {
            // TODO: Write test parameter through the savedata 0x80000000000000B0 (nsdsave:/test_parameter).

            throw new ServiceNotImplementedException(this, context, false);
        }

        [CommandHipc(52)] // 9.0.0+
        // ReadTestParameter() -> buffer<?>
        public ResultCode ReadTestParameter(ServiceCtx context)
        {
            // TODO: Read test parameter through the savedata 0x80000000000000B0 (nsdsave:/test_parameter).

            throw new ServiceNotImplementedException(this, context, false);
        }

        [CommandHipc(60)]
        // ReadSaveDataFromFsForTest() -> buffer<unknown<0x12bf0>, 0x16>
        public ResultCode ReadSaveDataFromFsForTest(ServiceCtx context)
        {
            if (!_isInitialized)
            {
                return ResultCode.ServiceNotInitialized;
            }

            // TODO: Read the savedata 0x80000000000000B0 (nsdsave:/file) and write it inside the buffer.

            Logger.Stub?.PrintStub(LogClass.ServiceNsd);

            return ResultCode.Success;
        }

        [CommandHipc(61)]
        // WriteSaveDataToFsForTest(buffer<unknown<0x12bf0>, 0x15>)
        public ResultCode WriteSaveDataToFsForTest(ServiceCtx context)
        {
            if (!_isInitialized)
            {
                return ResultCode.ServiceNotInitialized;
            }

            // TODO: When sys:set service calls will be implemented
            /*
                if (nn::settings::detail::GetSettingsItemValueSize("nsd", "test_mode") != 1)
                {
                    return ResultCode.InvalidSettingsValue;
                }
            */

            if (!_nsdSettings.TestMode)
            {
                return ResultCode.InvalidSettingsValue;
            }

            // TODO: Write the buffer inside the savedata 0x80000000000000B0 (nsdsave:/file).

            Logger.Stub?.PrintStub(LogClass.ServiceNsd);

            return ResultCode.Success;
        }

        [CommandHipc(62)]
        // DeleteSaveDataOfFsForTest()
        public ResultCode DeleteSaveDataOfFsForTest(ServiceCtx context)
        {
            if (!_isInitialized)
            {
                return ResultCode.ServiceNotInitialized;
            }

            // TODO: When sys:set service calls will be implemented
            /*
                if (nn::settings::detail::GetSettingsItemValueSize("nsd", "test_mode") != 1)
                {
                    return ResultCode.InvalidSettingsValue;
                }
            */

            if (!_nsdSettings.TestMode)
            {
                return ResultCode.InvalidSettingsValue;
            }

            // TODO: Delete the savedata 0x80000000000000B0.

            Logger.Stub?.PrintStub(LogClass.ServiceNsd);

            return ResultCode.Success;
        }

        [CommandHipc(63)] // 4.0.0+
        // IsChangeEnvironmentIdentifierDisabled() -> bytes<1>
        public ResultCode IsChangeEnvironmentIdentifierDisabled(ServiceCtx context)
        {
            // TODO: When sys:set service calls will be implemented use nn::settings::detail::GetServiceDiscoveryControlSettings()

            bool disabled = false;

            context.ResponseData.Write(disabled);

            Logger.Stub?.PrintStub(LogClass.ServiceNsd, new { disabled });

            return ResultCode.Success;
        }

        [CommandHipc(100)] // 10.0.0+
        // GetApplicationServerEnvironmentType() -> bytes<1>
        public ResultCode GetApplicationServerEnvironmentType(ServiceCtx context)
        {
            // TODO: Mount the savedata 0x80000000000000B0 (nsdsave:/test_parameter) and returns the environment type stored inside if the mount succeed.
            //       Returns ResultCode.NullOutputObject if failed.

            ResultCode result = _fqdnResolver.GetEnvironmentIdentifier(out string identifier);

            if (result != ResultCode.Success)
            {
                return result;
            }

            byte environmentType = identifier.Substring(0, 2) switch
            {
                "lp" => 1,
                "sd" => 2,
                "sp" => 3,
                "dp" => 4,
                _    => 0
            };

            context.ResponseData.Write(environmentType);

            return ResultCode.Success;
        }

        [CommandHipc(101)] // 10.0.0+
        // SetApplicationServerEnvironmentType(bytes<1>)
        public ResultCode SetApplicationServerEnvironmentType(ServiceCtx context)
        {
            throw new ServiceNotImplementedException(this, context, false);
        }

        [CommandHipc(102)] // 10.0.0+
        // DeleteApplicationServerEnvironmentType()
        public ResultCode DeleteApplicationServerEnvironmentType(ServiceCtx context)
        {
            throw new ServiceNotImplementedException(this, context, false);
        }
    }
}