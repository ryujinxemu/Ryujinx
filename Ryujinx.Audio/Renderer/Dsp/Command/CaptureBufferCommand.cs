﻿//
// Copyright (c) 2019-2021 Ryujinx
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.
//

using Ryujinx.Audio.Renderer.Dsp.State;
using Ryujinx.Audio.Renderer.Parameter;
using Ryujinx.Memory;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static Ryujinx.Audio.Renderer.Dsp.State.AuxiliaryBufferHeader;
using CpuAddress = System.UInt64;

namespace Ryujinx.Audio.Renderer.Dsp.Command
{
    public class CaptureBufferCommand : ICommand
    {
        public bool Enabled { get; set; }

        public int NodeId { get; }

        public CommandType CommandType => CommandType.CaptureBuffer;

        public ulong EstimatedProcessingTime { get; set; }

        public uint InputBufferIndex { get; }

        public ulong CpuBufferInfo { get; }
        public ulong DspBufferInfo { get; }

        public CpuAddress OutputBuffer { get; }
        public uint CountMax { get; }
        public uint UpdateCount { get; }
        public uint WriteOffset { get; }

        public bool IsEffectEnabled { get; }

        public CaptureBufferCommand(uint bufferOffset, byte inputBufferOffset, ulong sendBufferInfo, bool isEnabled,
                                    uint countMax, CpuAddress outputBuffer, uint updateCount, uint writeOffset, int nodeId)
        {
            Enabled = true;
            NodeId = nodeId;
            InputBufferIndex = bufferOffset + inputBufferOffset;
            CpuBufferInfo = sendBufferInfo;
            DspBufferInfo = sendBufferInfo + (ulong)Unsafe.SizeOf<AuxiliaryBufferHeader>();
            OutputBuffer = outputBuffer;
            CountMax = countMax;
            UpdateCount = updateCount;
            WriteOffset = writeOffset;
            IsEffectEnabled = isEnabled;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private uint Write(IVirtualMemoryManager memoryManager, ulong outBufferAddress, uint countMax, ReadOnlySpan<int> buffer, uint count, uint writeOffset, uint updateCount)
        {
            if (countMax == 0 || outBufferAddress == 0)
            {
                return 0;
            }

            uint targetWriteOffset = writeOffset + AuxiliaryBufferInfo.GetWriteOffset(memoryManager, DspBufferInfo);

            if (targetWriteOffset > countMax)
            {
                return 0;
            }

            uint remaining = count;

            uint inBufferOffset = 0;

            while (remaining != 0)
            {
                uint countToWrite = Math.Min(countMax - targetWriteOffset, remaining);

                memoryManager.Write(outBufferAddress + targetWriteOffset * sizeof(int), MemoryMarshal.Cast<int, byte>(buffer.Slice((int)inBufferOffset, (int)countToWrite)));

                targetWriteOffset = (targetWriteOffset + countToWrite) % countMax;
                remaining -= countToWrite;
                inBufferOffset += countToWrite;
            }

            if (updateCount != 0)
            {
                uint dspTotalSampleCount = AuxiliaryBufferInfo.GetTotalSampleCount(memoryManager, DspBufferInfo);
                uint cpuTotalSampleCount = AuxiliaryBufferInfo.GetTotalSampleCount(memoryManager, CpuBufferInfo);

                uint totalSampleCountDiff = dspTotalSampleCount - cpuTotalSampleCount;

                if (totalSampleCountDiff >= countMax)
                {
                    uint dspLostSampleCount = AuxiliaryBufferInfo.GetLostSampleCount(memoryManager, DspBufferInfo);
                    uint cpuLostSampleCount = AuxiliaryBufferInfo.GetLostSampleCount(memoryManager, CpuBufferInfo);

                    uint lostSampleCountDiff = dspLostSampleCount - cpuLostSampleCount;
                    uint newLostSampleCount = lostSampleCountDiff + updateCount;

                    if (lostSampleCountDiff > newLostSampleCount)
                    {
                        newLostSampleCount = cpuLostSampleCount - 1;
                    }

                    AuxiliaryBufferInfo.SetLostSampleCount(memoryManager, DspBufferInfo, newLostSampleCount);
                }

                uint newWriteOffset = (AuxiliaryBufferInfo.GetWriteOffset(memoryManager, DspBufferInfo) + updateCount) % countMax;

                AuxiliaryBufferInfo.SetWriteOffset(memoryManager, DspBufferInfo, newWriteOffset);

                uint newTotalSampleCount = totalSampleCountDiff + newWriteOffset;

                AuxiliaryBufferInfo.SetTotalSampleCount(memoryManager, DspBufferInfo, newTotalSampleCount);
            }

            return count;
        }

        public void Process(CommandList context)
        {
            Span<float> inputBuffer = context.GetBuffer((int)InputBufferIndex);

            if (IsEffectEnabled)
            {
                Span<int> inputBufferInt = MemoryMarshal.Cast<float, int>(inputBuffer);

                // Convert input data to the target format for user (int)
                DataSourceHelper.ToInt(inputBufferInt, inputBuffer, inputBuffer.Length);

                // Send the input to the user
                Write(context.MemoryManager, OutputBuffer, CountMax, inputBufferInt, context.SampleCount, WriteOffset, UpdateCount);

                // Convert back to float
                DataSourceHelper.ToFloat(inputBuffer, inputBufferInt, inputBuffer.Length);
            }
            else
            {
                AuxiliaryBufferInfo.Reset(context.MemoryManager, DspBufferInfo);
            }
        }
    }
}
