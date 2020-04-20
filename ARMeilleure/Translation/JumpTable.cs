﻿using ARMeilleure.Memory;
using ARMeilleure.Translation.PTC;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace ARMeilleure.Translation
{
    class JumpTable
    {
        // The jump table is a block of (guestAddress, hostAddress) function mappings.
        // Each entry corresponds to one branch in a JIT compiled function. The entries are
        // reserved specifically for each call.
        // The _dependants dictionary can be used to update the hostAddress for any functions that change.

        public const int JumpTableStride = 16; // 8 byte guest address, 8 byte host address.

        private const int JumpTableSize = 1048576;
        private const int JumpTableByteSize = JumpTableSize * JumpTableStride;

        // The dynamic table is also a block of (guestAddress, hostAddress) function mappings.
        // The main difference is that indirect calls and jumps reserve _multiple_ entries on the table.
        // These start out as all 0. When an indirect call is made, it tries to find the guest address on the table.

        // If we get to an empty address, the guestAddress is set to the call that we want.

        // If we get to a guestAddress that matches our own (or we just claimed it), the hostAddress is read.
        // If it is non-zero, we immediately branch or call the host function.
        // If it is 0, NativeInterface is called to find the rejited address of the call.
        // If none is found, the hostAddress entry stays at 0. Otherwise, the new address is placed in the entry.

        // If the table size is exhausted and we didn't find our desired address, we fall back to requesting
        // the function from the JIT.

        public const int DynamicTableElems = 1;

        public const int DynamicTableStride = DynamicTableElems * JumpTableStride;

        private const int DynamicTableSize = 1048576;
        private const int DynamicTableByteSize = DynamicTableSize * DynamicTableStride;

        private readonly ReservedRegion _jumpRegion;
        private readonly ReservedRegion _dynamicRegion;

        private int _tableEnd    = 0;
        private int _dynTableEnd = 0;

        private readonly ConcurrentDictionary<ulong, TranslatedFunction> _targets;
        private readonly ConcurrentDictionary<ulong, LinkedList<int>>    _dependants; // TODO: Attach to TranslatedFunction or a wrapper class.

        public IntPtr JumpPointer    => _jumpRegion.Pointer;
        public IntPtr DynamicPointer => _dynamicRegion.Pointer;

        public int TableEnd    => _tableEnd;
        public int DynTableEnd => _dynTableEnd;

        public ConcurrentDictionary<ulong, TranslatedFunction> Targets => _targets;
        public ConcurrentDictionary<ulong, LinkedList<int>> Dependants => _dependants;

        public JumpTable()
        {
            _jumpRegion    = new ReservedRegion(JumpTableByteSize);
            _dynamicRegion = new ReservedRegion(DynamicTableByteSize);

            _targets    = new ConcurrentDictionary<ulong, TranslatedFunction>();
            _dependants = new ConcurrentDictionary<ulong, LinkedList<int>>();
        }

        public void Initialize(PtcJumpTable ptcJumpTable, ConcurrentDictionary<ulong, TranslatedFunction> funcsHighCq)
        {
            _tableEnd    = ptcJumpTable.TableEnd;
            _dynTableEnd = ptcJumpTable.DynTableEnd;

            foreach (ulong guestAddress in ptcJumpTable.Targets)
            {
                if (funcsHighCq.TryGetValue(guestAddress, out TranslatedFunction func))
                {
                    _targets.TryAdd(guestAddress, func);
                }
                else
                {
                    throw new KeyNotFoundException($"({nameof(guestAddress)} = 0x{guestAddress:X16})");
                }
            }

            foreach (var item in ptcJumpTable.Dependants)
            {
                _dependants.TryAdd(item.Key, new LinkedList<int>(item.Value));
            }
        }

        public void RegisterFunction(ulong address, TranslatedFunction func)
        {
            address &= ~3UL;
            _targets.AddOrUpdate(address, func, (key, oldFunc) => func);
            long funcPtr = func.FuncPtr.ToInt64();

            // Update all jump table entries that target this address.
            if (_dependants.TryGetValue(address, out LinkedList<int> myDependants))
            {
                lock (myDependants)
                {
                    foreach (int entry in myDependants)
                    {
                        IntPtr addr = GetEntryAddressJumpTable(entry);

                        Marshal.WriteInt64(addr, 8, funcPtr);
                    }
                }
            }
        }

        public int ReserveTableEntry(long ownerAddress, long address, bool isJump)
        {
            int entry = Interlocked.Increment(ref _tableEnd);

            ExpandIfNeededJumpTable(entry);

            // Is the address we have already registered? If so, put the function address in the jump table.
            // If not, it will point to the direct call stub.
            long value = DirectCallStubs.DirectCallStub(isJump).ToInt64();
            if (_targets.TryGetValue((ulong)address, out TranslatedFunction func))
            {
                value = func.FuncPtr.ToInt64();
            }

            // Make sure changes to the function at the target address update this jump table entry.
            LinkedList<int> targetDependants = _dependants.GetOrAdd((ulong)address, (addr) => new LinkedList<int>());
            lock (targetDependants)
            {
                targetDependants.AddLast(entry);
            }

            IntPtr addr = GetEntryAddressJumpTable(entry);

            Marshal.WriteInt64(addr, 0, address);
            Marshal.WriteInt64(addr, 8, value);

            return entry;
        }

        public int ReserveDynamicEntry(bool isJump)
        {
            int entry = Interlocked.Increment(ref _dynTableEnd);

            ExpandIfNeededDynamicTable(entry);

            // Initialize all host function pointers to the indirect call stub.
            IntPtr addr = GetEntryAddressDynamicTable(entry);
            long stubPtr = DirectCallStubs.IndirectCallStub(isJump).ToInt64();

            for (int i = 0; i < DynamicTableElems; i++)
            {
                Marshal.WriteInt64(addr, i * JumpTableStride + 8, stubPtr);
            }

            return entry;
        }

        public void ExpandIfNeededJumpTable(int entries)
        {
            Debug.Assert(entries > 0);

            if (entries < JumpTableSize)
            {
                _jumpRegion.ExpandIfNeeded((ulong)((entries + 1) * JumpTableStride));
            }
            else
            {
                throw new OutOfMemoryException("JIT Direct Jump Table exhausted.");
            }
        }

        public void ExpandIfNeededDynamicTable(int entries)
        {
            Debug.Assert(entries >= 1);

            if (entries < DynamicTableSize)
            {
                _dynamicRegion.ExpandIfNeeded((ulong)((entries + 1) * DynamicTableStride));
            }
            else
            {
                throw new OutOfMemoryException("JIT Dynamic Jump Table exhausted.");
            }
        }

        public IntPtr GetEntryAddressJumpTable(int entry)
        {
            Debug.Assert(entry >= 1 && entry <= _tableEnd);

            return _jumpRegion.Pointer + entry * JumpTableStride;
        }

        public IntPtr GetEntryAddressDynamicTable(int entry)
        {
            Debug.Assert(entry >= 1 && entry <= _dynTableEnd);

            return _dynamicRegion.Pointer + entry * DynamicTableStride;
        }
    }
}
