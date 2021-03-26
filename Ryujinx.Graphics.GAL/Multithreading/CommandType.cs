﻿namespace Ryujinx.Graphics.GAL.Multithreading
{
    enum CommandType : byte
    {
        Action = 0x00,
        CompileShader = 0x01,
        CreateBuffer = 0x02,
        CreateProgram = 0x03,
        CreateSampler = 0x04,
        CreateSync = 0x05,
        CreateTexture = 0x06,
        GetCapabilities = 0x07,
        LoadProgramBinary = 0x08,
        PreFrame = 0x09,
        ReportCounter = 0x0a,
        ResetCounter = 0x0b,
        UpdateCounters = 0x0c,

        BufferDispose = 0x10,
        BufferGetData = 0x11,
        BufferSetData = 0x12,

        CounterEventDispose = 0x20,
        CounterEventFlush = 0x21,

        ProgramDispose = 0x30,
        ProgramGetBinary = 0x31,

        SamplerDispose = 0x40,

        ShaderDispose = 0x50,

        TextureCopyTo = 0x60,
        TextureCopyToScaled = 0x61,
        TextureCopyToSlice = 0x62,
        TextureCreateView = 0x63,
        TextureGetData = 0x64,
        TextureRelease = 0x65,
        TextureSetData = 0x66,
        TextureSetDataSlice = 0x67,
        TextureSetStorage = 0x68,

        WindowPresent = 0x70,

        Barrier = 0x80,
        BeginTransformFeedback = 0x81,
        ClearBuffer = 0x82,
        ClearRenderTargetColor = 0x83,
        ClearRenderTargetDepthStencil = 0x84,
        CopyBuffer = 0x85,
        DispatchCompute = 0x86,
        Draw = 0x87,
        DrawIndexed = 0x88,
        EndHostConditionalRendering = 0x89,
        EndTransformFeedback = 0x8a,
        SetAlphaTest = 0x8b,
        SetBlendState = 0x8c,
        SetDepthBias = 0x8d,
        SetDepthClamp = 0x8e,
        SetDepthMode = 0x8f,
        SetDepthTest = 0x90,
        SetFaceCulling = 0x91,
        SetFrontFace = 0x92,
        SetGenericBuffers = 0x93,
        SetImage = 0x94,
        SetIndexBuffer = 0x95,
        SetLogicOpState = 0x96,
        SetPointParameters = 0x97,
        SetPrimitiveRestart = 0x98,
        SetPrimitiveTopology = 0x99,
        SetProgram = 0x9a,
        SetRasterizerDiscard = 0x9b,
        SetRenderTargetColorMasks = 0x9c,
        SetRenderTargetScale = 0x9d,
        SetRenderTargets = 0x9e,
        SetSampler = 0x9f,
        SetScissor = 0xa0,
        SetStencilTest = 0xa1,
        SetTexture = 0xa2,
        SetUserClipDistance = 0xa3,
        SetVertexAttribs = 0xa4,
        SetVertexBuffers = 0xa5,
        SetViewports = 0xa6,
        TextureBarrier = 0xa7,
        TextureBarrierTiled = 0xa8,
        TryHostConditionalRendering = 0xa9,
        TryHostConditionalRenderingFlush = 0xaa,
        UpdateRenderScale = 0xab
    }
}
