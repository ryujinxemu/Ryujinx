using Ryujinx.Graphics.OpenGL;
using Silk.NET.OpenGL.Legacy;
using SPB.Graphics;
using SPB.Graphics.OpenGL;
using SPB.Platform;
using SPB.Windowing;

namespace Ryujinx.UI
{
    class SPBOpenGLContext : IOpenGLContext
    {
        public readonly GL Api;
        private readonly OpenGLContextBase _context;
        private readonly NativeWindowBase _window;

        private SPBOpenGLContext(GL api, OpenGLContextBase context, NativeWindowBase window)
        {
            Api = api;
            _context = context;
            _window = window;
        }

        public void Dispose()
        {
            _context.Dispose();
            _window.Dispose();
        }

        public void MakeCurrent()
        {
            _context.MakeCurrent(_window);
        }

        public bool HasContext() => _context.IsCurrent;

        public static SPBOpenGLContext CreateBackgroundContext(OpenGLContextBase sharedContext)
        {
            OpenGLContextBase context = PlatformHelper.CreateOpenGLContext(FramebufferFormat.Default, 3, 3, OpenGLContextFlags.Compat, true, sharedContext);
            NativeWindowBase window = PlatformHelper.CreateOpenGLWindow(FramebufferFormat.Default, 0, 0, 100, 100);

            context.Initialize(window);
            context.MakeCurrent(window);

            GL api = GL.GetApi(context.GetProcAddress);

            context.MakeCurrent(null);

            return new SPBOpenGLContext(api, context, window);
        }
    }
}
