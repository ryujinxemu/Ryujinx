namespace Ryujinx.Graphics.Gal.Shader
{
    enum ShaderIrInst
    {
        Invalid,

        B_Start,
        Band,
        Bnot,
        Bor,
        Bxor,
        B_End,

        F_Start,
        Ceil,

        Fabs,
        Fadd,
        Fceq,
        Fcequ,
        Fcge,
        Fcgeu,
        Fcgt,
        Fcgtu,
        Fclamp,
        Fcle,
        Fcleu,
        Fclt,
        Fcltu,
        Fcnan,
        Fcne,
        Fcneu,
        Fcnum,
        Fcos,
        Fex2,
        Ffma,
        Flg2,
        Floor,
        Fmax,
        Fmin,
        Fmul,
        Fneg,
        Frcp,
        Frsq,
        Fsin,
        Fsqrt,
        Ftos,
        Ftou,
        Ipa,
        Texs,
        Trunc,
        F_End,

        I_Start,
        Abs,
        Add,
        And,
        Asr,
        Ceq,
        Cge,
        Cgt,
        Clamps,
        Clampu,
        Cle,
        Clt,
        Cne,
        Lsl,
        Lsr,
        Max,
        Min,
        Mul,
        Neg,
        Not,
        Or,
        Stof,
        Sub,
        Texq,
        Txlf,
        Utof,
        Xor,
        I_End,

        Bra,
        Exit,
        Kil
    }
}