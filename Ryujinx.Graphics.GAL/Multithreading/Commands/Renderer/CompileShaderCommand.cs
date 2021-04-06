﻿using Ryujinx.Graphics.GAL.Multithreading.Model;
using Ryujinx.Graphics.GAL.Multithreading.Resources;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands.Renderer
{
    struct CompileShaderCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.CompileShader;
        private TableRef<ThreadedShader> _shader;

        public void Set(TableRef<ThreadedShader> shader)
        {
            _shader = shader;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            ThreadedShader shader = _shader.Get(threaded);
            shader.EnsureCreated();
        }
    }
}
