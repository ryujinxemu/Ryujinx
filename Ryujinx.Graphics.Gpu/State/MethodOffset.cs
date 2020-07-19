namespace Ryujinx.Graphics.Gpu.State
{
    /// <summary>
    /// GPU method offset.
    /// </summary>
    /// <remarks>
    /// This is indexed in 32 bits word.
    /// </remarks>
    enum MethodOffset
    {
        BindChannel                     = 0x0,
        I2mParams                       = 0x60,
        LaunchDma                       = 0x6c,
        LoadInlineData                  = 0x6d,
        CopyDstTexture                  = 0x80,
        CopySrcTexture                  = 0x8c,
        DispatchParamsAddress           = 0xad,
        Dispatch                        = 0xaf,
        SyncpointAction                 = 0xb2,
        CopyBuffer                      = 0xc0,
        RasterizeEnable                 = 0xdf,
        TfBufferState                   = 0xe0,
        CopyBufferParams                = 0x100,
        TfState                         = 0x1c0,
        CopyBufferSwizzle               = 0x1c2,
        CopyBufferDstTexture            = 0x1c3,
        CopyBufferSrcTexture            = 0x1ca,
        TfEnable                        = 0x1d1,
        RtColorState                    = 0x200,
        CopyTextureControl              = 0x223,
        CopyRegion                      = 0x22c,
        CopyTexture                     = 0x237,
        ViewportTransform               = 0x280,
        ViewportExtents                 = 0x300,
        VertexBufferDrawState           = 0x35d,
        DepthMode                       = 0x35f,
        ClearColors                     = 0x360,
        ClearDepthValue                 = 0x364,
        ClearStencilValue               = 0x368,
        DepthBiasState                  = 0x370,
        TextureBarrier                  = 0x378,
        ScissorState                    = 0x380,
        StencilBackMasks                = 0x3d5,
        InvalidateTextures              = 0x3dd,
        TextureBarrierTiled             = 0x3df,
        RtColorMaskShared               = 0x3e4,
        RtDepthStencilState             = 0x3f8,
        VertexAttribState               = 0x458,
        RtControl                       = 0x487,
        RtDepthStencilSize              = 0x48a,
        SamplerIndex                    = 0x48d,
        DepthTestEnable                 = 0x4b3,
        BlendIndependent                = 0x4b9,
        DepthWriteEnable                = 0x4ba,
        VbElementU8                     = 0x4c1,
        DepthTestFunc                   = 0x4c3,
        BlendConstant                   = 0x4c7,
        BlendStateCommon                = 0x4cf,
        BlendEnableCommon               = 0x4d7,
        BlendEnable                     = 0x4d8,
        StencilTestState                = 0x4e0,
        YControl                        = 0x4eb,
        FirstVertex                     = 0x50d,
        FirstInstance                   = 0x50e,
        ClipDistanceEnable              = 0x544,
        PointSize                       = 0x546,
        ResetCounter                    = 0x54c,
        RtDepthStencilEnable            = 0x54e,
        ConditionState                  = 0x554,
        SamplerPoolState                = 0x557,
        DepthBiasFactor                 = 0x55b,
        TexturePoolState                = 0x55d,
        StencilBackTestState            = 0x565,
        DepthBiasUnits                  = 0x56f,
        RtMsaaMode                      = 0x574,
        VbElementU32                    = 0x57a,
        VbElementU16                    = 0x57c,
        ShaderBaseAddress               = 0x582,
        DrawEnd                         = 0x585,
        DrawBegin                       = 0x586,
        PrimitiveRestartState           = 0x591,
        IndexBufferState                = 0x5f2,
        IndexBufferCount                = 0x5f8,
        DepthBiasClamp                  = 0x61f,
        VertexBufferInstanced           = 0x620,
        FaceState                       = 0x646,
        ViewportTransformEnable         = 0x64b,
        ViewVolumeClipControl           = 0x64f,
        LogicOpState                    = 0x671,
        Clear                           = 0x674,
        RtColorMask                     = 0x680,
        ReportState                     = 0x6c0,
        Report                          = 0x6c3,
        VertexBufferState               = 0x700,
        BlendState                      = 0x780,
        VertexBufferEndAddress          = 0x7c0,
        ShaderState                     = 0x800,
        FirmwareCall0                   = 0x8c0,
        FirmwareCall1                   = 0x8c1,
        FirmwareCall2                   = 0x8c2,
        FirmwareCall3                   = 0x8c3,
        FirmwareCall4                   = 0x8c4,
        FirmwareCall5                   = 0x8c5,
        FirmwareCall6                   = 0x8c6,
        FirmwareCall7                   = 0x8c7,
        UniformBufferState              = 0x8e0,
        UniformBufferUpdateData         = 0x8e4,
        UniformBufferBindVertex         = 0x904,
        UniformBufferBindTessControl    = 0x90c,
        UniformBufferBindTessEvaluation = 0x914,
        UniformBufferBindGeometry       = 0x91c,
        UniformBufferBindFragment       = 0x924,
        TextureBufferIndex              = 0x982,
        TfVaryingLocations              = 0xa00
    }
}