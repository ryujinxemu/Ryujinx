﻿#define SimdMem32

using ARMeilleure.State;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Ryujinx.Tests.Cpu
{
    [Category("SimdMemory32")]
    public sealed class CpuTestSimdMemory32 : CpuTest32
    {
#if SimdMem32
        private const int RndCntImm = 2;

        private uint[] LDSTModes =
            {
                //LD1
                0b0111,
                0b1010,
                0b0110,
                0b0010,

                //LD2
                0b1000,
                0b1001,
                0b0011,

                //LD3
                0b0100,
                0b0101,

                //LD4
                0b0000,
                0b0001
            };

        [Test, Pairwise, Description("VLDn.<size> <list>, [<Rn> {:<align>}]{ /!/, <Rm>} (single n element structure)")]
        public void Vldn_Single([Values(0u, 1u, 2u)] uint size,
                        [Values(0u, 13u)] uint rn,
                        [Values(1u, 13u, 15u)] uint rm,
                        [Values(0u, 1u, 2u, 3u, 4u, 5u, 6u, 7u)] uint vd,
                        [Range(0u, 7u)] uint index,
                        [Range(0u, 3u)] uint n,
                        [Values(0x0u)] [Random(0u, 0xffu, RndCntImm)] uint offset)
        {
            var data = GenerateVectorSequence(0x1000);
            SetWorkingMemory(data);

            uint opcode = 0xf4a00000; // vld1.8 {d0[0]}, [r0], r0

            opcode |= ((size & 3) << 10) | ((rn & 15) << 16) | (rm & 15);

            uint index_align = (index << (int)(1 + size)) & 15;

            opcode |= (index_align) << 4;

            opcode |= ((vd & 0x10) << 18);
            opcode |= ((vd & 0xf) << 12);

            opcode |= (n & 3) << 8; //LD1 is 0, LD2 is 1 etc

            SingleOpcode(opcode, r0: 0x2500, r1: offset, sp: 0x2500);

            CompareAgainstUnicorn();
        }

        [Test, Combinatorial, Description("VLDn.<size> <list>, [<Rn> {:<align>}]{ /!/, <Rm>} (all lanes)")]
        public void Vldn_All([Values(0u, 13u)] uint rn,
                [Values(1u, 13u, 15u)] uint rm,
                [Values(0u, 1u, 2u, 3u, 4u, 5u, 6u, 7u)] uint vd,
                [Range(0u, 3u)] uint n,
                [Range(0u, 2u)] uint size,
                [Values] bool t,
                [Values(0x0u)] [Random(0u, 0xffu, RndCntImm)] uint offset)
        {
            var data = GenerateVectorSequence(0x1000);
            SetWorkingMemory(data);

            uint opcode = 0xf4a00c00; // vld1.8 {d0[0]}, [r0], r0

            opcode |= ((size & 3) << 6) | ((rn & 15) << 16) | (rm & 15);

            opcode |= ((vd & 0x10) << 18);
            opcode |= ((vd & 0xf) << 12);

            opcode |= (n & 3) << 8; //LD1 is 0, LD2 is 1 etc
            if (t) opcode |= 1 << 5; //LD1 is 0, LD2 is 1 etc

            SingleOpcode(opcode, r0: 0x2500, r1: offset, sp: 0x2500);

            CompareAgainstUnicorn();
        }

        [Test, Combinatorial, Description("VLDn.<size> <list>, [<Rn> {:<align>}]{ /!/, <Rm>} (multiple n element structures)")]
        public void Vldn_Pair([Values(0u, 1u, 2u, 3u)] uint size,
                [Values(0u, 13u)] uint rn,
                [Values(1u, 13u, 15u)] uint rm,
                [Values(0u, 1u, 2u, 3u, 4u, 5u, 6u, 7u)] uint vd,
                [Range(0u, 3u)] uint mode,
                [Values(0x0u)] [Random(0u, 0xffu, RndCntImm)] uint offset)
        {
            var data = GenerateVectorSequence(0x1000);
            SetWorkingMemory(data);

            uint opcode = 0xf4200000; // vld4.8 {d0, d1, d2, d3}, [r0], r0

            opcode |= ((size & 3) << 6) | ((rn & 15) << 16) | (rm & 15) | (LDSTModes[mode] << 8);

            opcode |= ((vd & 0x10) << 18);
            opcode |= ((vd & 0xf) << 12);

            SingleOpcode(opcode, r0: 0x2500, r1: offset, sp: 0x2500);

            CompareAgainstUnicorn();
        }

        [Test, Pairwise, Description("VSTn.<size> <list>, [<Rn> {:<align>}]{ /!/, <Rm>} (single n element structure)")]
        public void Vstn_Single([Values(0u, 1u, 2u)] uint size,
                [Values(0u, 13u)] uint rn,
                [Values(1u, 13u, 15u)] uint rm,
                [Values(0u, 1u, 2u, 3u, 4u, 5u, 6u, 7u)] uint vd,
                [Range(0u, 7u)] uint index,
                [Range(0u, 3u)] uint n,
                [Values(0x0u)] [Random(0u, 0xffu, RndCntImm)] uint offset)
        {
            var data = GenerateVectorSequence(0x1000);
            SetWorkingMemory(data);

            (V128 vec1, V128 vec2, V128 vec3, V128 vec4) = GenerateTestVectors();

            uint opcode = 0xf4800000; // vst1.8 {d0[0]}, [r0], r0

            opcode |= ((size & 3) << 10) | ((rn & 15) << 16) | (rm & 15);

            uint index_align = (index << (int)(1 + size)) & 15;

            opcode |= (index_align) << 4;

            opcode |= ((vd & 0x10) << 18);
            opcode |= ((vd & 0xf) << 12);

            opcode |= (n & 3) << 8; //ST1 is 0, ST2 is 1 etc

            SingleOpcode(opcode, r0: 0x2500, r1: offset, v1: vec1, v2: vec2, v3: vec3, v4: vec4, sp: 0x2500);

            CompareAgainstUnicorn();
        }

        [Test, Combinatorial, Description("VSTn.<size> <list>, [<Rn> {:<align>}]{ /!/, <Rm>} (multiple n element structures)")]
        public void Vstn_Pair([Values(0u, 1u, 2u, 3u)] uint size,
                [Values(0u, 13u)] uint rn,
                [Values(1u, 13u, 15u)] uint rm,
                [Values(0u, 1u, 2u, 3u, 4u, 5u, 6u, 7u)] uint vd,
                [Range(0u, 3u)] uint mode,
                [Values(0x0u)] [Random(0u, 0xffu, RndCntImm)] uint offset)
        {
            var data = GenerateVectorSequence(0x1000);
            SetWorkingMemory(data);

            (V128 vec1, V128 vec2, V128 vec3, V128 vec4) = GenerateTestVectors();

            uint opcode = 0xf4000000; // vst4.8 {d0, d1, d2, d3}, [r0], r0

            opcode |= ((size & 3) << 6) | ((rn & 15) << 16) | (rm & 15) | (LDSTModes[mode] << 8);

            opcode |= ((vd & 0x10) << 18);
            opcode |= ((vd & 0xf) << 12);

            SingleOpcode(opcode, r0: 0x2500, r1: offset, v1: vec1, v2: vec2, v3: vec3, v4: vec4, sp: 0x2500);

            CompareAgainstUnicorn();
        }

        [Test, Combinatorial, Description("VLDM.<size> <Rn>{!}, <d/sreglist>")]
        public void Vldm([Values(0u, 13u)] uint rn,
        [Values(0u, 1u, 2u, 3u, 4u, 5u, 6u, 7u)] uint vd,
        [Range(0u, 2u)] uint mode,
        [Values(0x1u, 0x32u)] [Random(2u, 31u, RndCntImm)] uint regs,
        [Values] bool single)
        {
            var data = GenerateVectorSequence(0x1000);
            SetWorkingMemory(data);

            uint opcode = 0xec100a00; // vst4.8 {d0, d1, d2, d3}, [r0], r0

            uint[] vldmModes = {
                //note: 3rd 0 leaves a space for "D"
                0b0100, // increment after
                0b0101, // increment after !
                0b1001  // decrement before !
            };

            opcode |= ((vldmModes[mode] & 15) << 21);
            opcode |= ((rn & 15) << 16);

            opcode |= ((vd & 0x10) << 18);
            opcode |= ((vd & 0xf) << 12);

            opcode |= ((uint)(single ? 0 : 1) << 8);

            if (!single) regs = (regs << 1); //low bit must be 0 - must be even number of registers.
            uint regSize = single ? 1u : 2u;

            if (vd + (regs / regSize) > 32) //can't address further than s31 or d31
            {
                regs -= (vd + (regs / regSize)) - 32;
            }

            if (regs / regSize > 16) //can't do more than 16 registers at a time
            {
                regs = 16 * regSize;
            }

            opcode |= regs & 0xff;

            SingleOpcode(opcode, r0: 0x2500, sp: 0x2500);

            CompareAgainstUnicorn();
        }

        [Test, Pairwise, Description("VLDR.<size> <Sd>, [<Rn> {, #{+/-}<imm>}]")]
        public void Vldr([Values(2u, 3u)] uint size, //fp16 is not supported for now
                        [Values(0u)] uint rn,
                        [Values(0u, 1u, 2u, 3u, 4u, 5u, 6u, 7u)] uint sd,
                        [Values(0x0u)] [Random(0u, 0xffu, RndCntImm)] uint imm,
                        [Values] bool sub)
        {
            var data = GenerateVectorSequence(0x1000);
            SetWorkingMemory(data);

            uint opcode = 0xed900a00; // VLDR.32 S0, [R0, #0]
            opcode |= ((size & 3) << 8) | ((rn & 15) << 16);

            if (sub)
            {
                opcode &= ~(uint)(1 << 23);
            }

            if (size == 2)
            {
                opcode |= ((sd & 0x1) << 22);
                opcode |= ((sd & 0x1e) << 11);
            } 
            else
            {
                opcode |= ((sd & 0x10) << 18);
                opcode |= ((sd & 0xf) << 12);
            }
            opcode |= (uint)imm & 0xff;

            SingleOpcode(opcode, r0: 0x2500); //correct

            CompareAgainstUnicorn();
        }

        [Test, Pairwise, Description("VSTR.<size> <Sd>, [<Rn> {, #{+/-}<imm>}]")]
        public void Vstr([Values(2u, 3u)] uint size, //fp16 is not supported for now
                [Values(0u)] uint rn,
                [Values(0u, 1u, 2u, 3u, 4u, 5u, 6u, 7u)] uint sd,
                [Values(0x0u)] [Random(0u, 0xffu, RndCntImm)] uint imm,
                [Values] bool sub)
        {
            var data = GenerateVectorSequence(0x1000);
            SetWorkingMemory(data);

            uint opcode = 0xed800a00; // VSTR.32 S0, [R0, #0]
            opcode |= ((size & 3) << 8) | ((rn & 15) << 16);

            if (sub)
            {
                opcode &= ~(uint)(1 << 23);
            }

            if (size == 2)
            {
                opcode |= ((sd & 0x1) << 22);
                opcode |= ((sd & 0x1e) << 11);
            }
            else
            {
                opcode |= ((sd & 0x10) << 18);
                opcode |= ((sd & 0xf) << 12);
            }
            opcode |= (uint)imm & 0xff;

            (V128 vec1, V128 vec2, _, _) = GenerateTestVectors();

            SingleOpcode(opcode, r0: 0x2500, v0: vec1, v1: vec2); //correct

            CompareAgainstUnicorn();
        }

        private (V128, V128, V128, V128) GenerateTestVectors()
        {
            return (
                new V128(-12.43f, 1872.23f, 4456.23f, -5622.2f),
                new V128(0.0f, float.NaN, float.PositiveInfinity, float.NegativeInfinity),
                new V128(1.23e10f, -0.0f, -0.123f, 0.123f),
                new V128(float.Epsilon, 3.5f, 925.23f, -104.9f)
                );
        }

        private byte[] GenerateVectorSequence(int length)
        {
            int floatLength = length >> 2;
            float[] data = new float[floatLength];

            for (int i=0; i<floatLength; i++)
            {
                data[i] = i + (i / 9f);
            }

            var result = new byte[length];
            Buffer.BlockCopy(data, 0, result, 0, result.Length);
            return result;
        }
#endif
    }
}
