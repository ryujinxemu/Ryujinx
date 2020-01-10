﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ryujinx.Tests.Cpu
{
    [Category("Bfm")]
    public sealed class CpuTestBf32 : CpuTest32
    {
        private const int RndCnt = 10;
        private const int RndCntImmr = 10;
        private const int RndCntImms = 10;

        [Test, Pairwise, Description("BFC <Rd>, #<lsb>, #<width>")]
        public void Bfc([Values(0u, 0xdu)] uint rd,
                        [Values(0x00000000u, 0x7FFFFFFFu,
                                0x80000000u, 0xFFFFFFFFu)] [Random(RndCnt)] uint wd,
                        [Values(0u, 15u, 16u, 31u)] [Random(0u, 31u, RndCntImmr)] uint lsb,
                        [Values(0u, 15u, 16u, 31u)] [Random(0u, 31u, RndCntImms)] uint msb)
        {
            msb = Math.Max(lsb, msb); // don't test unpredictable for now
            uint opcode = 0xe7c0001f; // BFC R0, #0, #1
            opcode |= ((rd & 0xf) << 12);
            opcode |= ((msb & 31) << 16) | ((lsb & 31) << 7);

            uint sp = TestContext.CurrentContext.Random.NextUInt();

            SingleOpcode(opcode, r0: wd, sp: sp);

            CompareAgainstUnicorn();
        }

        [Test, Pairwise, Description("BFI <Rd>, <Rn>, #<lsb>, #<width>")]
        public void Bfi([Values(0u, 0xdu)] uint rd,
                        [Values(1u, 0xdu)] uint rn,
                        [Random(RndCnt)] uint wd,
                        [Values(0x00000000u, 0x7FFFFFFFu,
                                0x80000000u, 0xFFFFFFFFu)] [Random(RndCnt)] uint wn,
                        [Values(0u, 15u, 16u, 31u)] [Random(0u, 31u, RndCntImmr)] uint lsb,
                        [Values(0u, 15u, 16u, 31u)] [Random(0u, 31u, RndCntImms)] uint msb)
        {
            msb = Math.Max(lsb, msb); // don't test unpredictable for now
            uint opcode = 0xe7c00010; // BFI r0, r0, #0, #1
            opcode |= ((rd & 0xf) << 12);
            opcode |= ((rn & 0xf) << 0);
            opcode |= ((msb & 31) << 16) | ((lsb & 31) << 7);

            uint sp = TestContext.CurrentContext.Random.NextUInt();

            SingleOpcode(opcode, r0: wd, r1: wn, sp: sp);

            CompareAgainstUnicorn();
        }

        [Test, Pairwise, Description("UBFX <Rd>, <Rn>, #<lsb>, #<width>")]
        public void Ubfx([Values(0u, 0xdu)] uint rd,
        [Values(1u, 0xdu)] uint rn,
        [Random(RndCnt)] uint wd,
        [Values(0x00000000u, 0x7FFFFFFFu,
                                0x80000000u, 0xFFFFFFFFu)] [Random(RndCnt)] uint wn,
        [Values(0u, 15u, 16u, 31u)] [Random(0u, 31u, RndCntImmr)] uint lsb,
        [Values(0u, 15u, 16u, 31u)] [Random(0u, 31u, RndCntImms)] uint widthm1)
        {
            if (lsb + widthm1 > 31)
            {
                widthm1 -= (lsb + widthm1) - 31;
            }
            uint opcode = 0xe7e00050; // UBFX r0, r0, #0, #1
            opcode |= ((rd & 0xf) << 12);
            opcode |= ((rn & 0xf) << 0);
            opcode |= ((widthm1 & 31) << 16) | ((lsb & 31) << 7);

            uint sp = TestContext.CurrentContext.Random.NextUInt();

            SingleOpcode(opcode, r0: wd, r1: wn, sp: sp);

            CompareAgainstUnicorn();
        }

        [Test, Pairwise, Description("SBFX <Rd>, <Rn>, #<lsb>, #<width>")]
        public void Sbfx([Values(0u, 0xdu)] uint rd,
                [Values(1u, 0xdu)] uint rn,
                [Random(RndCnt)] uint wd,
                [Values(0x00000000u, 0x7FFFFFFFu,
                                0x80000000u, 0xFFFFFFFFu)] [Random(RndCnt)] uint wn,
                [Values(0u, 15u, 16u, 31u)] [Random(0u, 31u, RndCntImmr)] uint lsb,
                [Values(0u, 15u, 16u, 31u)] [Random(0u, 31u, RndCntImms)] uint widthm1)
        {
            if (lsb + widthm1 > 31)
            {
                widthm1 -= (lsb + widthm1) - 31;
            }
            uint opcode = 0xe7a00050; // SBFX r0, r0, #0, #1
            opcode |= ((rd & 0xf) << 12);
            opcode |= ((rn & 0xf) << 0);
            opcode |= ((widthm1 & 31) << 16) | ((lsb & 31) << 7);

            uint sp = TestContext.CurrentContext.Random.NextUInt();

            SingleOpcode(opcode, r0: wd, r1: wn, sp: sp);

            CompareAgainstUnicorn();
        }
    }
}
