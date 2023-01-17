using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;

namespace Ryujinx.Ui.Common.Helper
{
    [SupportedOSPlatform("macos")]
    public static partial class NativeMacOS
    {
        private const string ObjCRuntime = "/usr/lib/libobjc.A.dylib";

        [LibraryImport(ObjCRuntime, StringMarshalling = StringMarshalling.Utf8)]
        private static unsafe partial IntPtr sel_getUid(string name);

        [LibraryImport(ObjCRuntime, StringMarshalling = StringMarshalling.Utf8)]
        public static partial IntPtr objc_getClass(string name);

        [LibraryImport(ObjCRuntime)]
        public static partial void objc_msgSend(IntPtr receiver, Selector selector);

        [LibraryImport(ObjCRuntime)]
        public static partial void objc_msgSend(IntPtr receiver, Selector selector, byte value);

        [LibraryImport(ObjCRuntime)]
        public static partial void objc_msgSend(IntPtr receiver, Selector selector, IntPtr value);

        [LibraryImport(ObjCRuntime)]
        public static partial void objc_msgSend(IntPtr receiver, Selector selector, NSRect point);

        [LibraryImport(ObjCRuntime)]
        public static partial void objc_msgSend(IntPtr receiver, Selector selector, double value);

        [LibraryImport(ObjCRuntime, EntryPoint = "objc_msgSend")]
        public static partial IntPtr IntPtr_objc_msgSend(IntPtr receiver, Selector selector);

        [LibraryImport(ObjCRuntime, EntryPoint = "objc_msgSend")]
        public static partial IntPtr IntPtr_objc_msgSend(IntPtr receiver, Selector selector, IntPtr param);

        [LibraryImport(ObjCRuntime, EntryPoint = "objc_msgSend", StringMarshalling = StringMarshalling.Utf8)]
        public static partial IntPtr IntPtr_objc_msgSend(IntPtr receiver, Selector selector, string param);

        [LibraryImport(ObjCRuntime, EntryPoint = "objc_msgSend")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool bool_objc_msgSend(IntPtr receiver, Selector selector, IntPtr param);

        public struct Selector
        {
            public readonly IntPtr SelPtr;

            public unsafe Selector(string name)
            {
                SelPtr = sel_getUid(name);
            }

            public static implicit operator Selector(string value) => new(value);
        }

        public struct NSURL
        {
            public readonly IntPtr URLPtr;

            public NSURL(string path)
            {
                NSString nsStringPath = new(path);
                IntPtr nsUrl = objc_getClass("NSURL");
                if (File.Exists(path))
                {
                    URLPtr = IntPtr_objc_msgSend(nsUrl, "fileURLWithPath:", nsStringPath);
                }
                else
                {
                    URLPtr = IntPtr_objc_msgSend(nsUrl, "URLWithString:", nsStringPath);
                }
            }

            public void OpenURL()
            {
                IntPtr nsWorkspace = objc_getClass("NSWorkspace");
                IntPtr sharedWorkspace = IntPtr_objc_msgSend(nsWorkspace, "sharedWorkspace");

                bool_objc_msgSend(sharedWorkspace, "openURL:", URLPtr);
            }

            public void ActivateFileViewerSelectingURL()
            {
                IntPtr nsArray = objc_getClass("NSArray");
                IntPtr urlArray = IntPtr_objc_msgSend(nsArray, "arrayWithObject:", URLPtr);

                IntPtr nsWorkspace = objc_getClass("NSWorkspace");
                IntPtr sharedWorkspace = IntPtr_objc_msgSend(nsWorkspace, "sharedWorkspace");

                objc_msgSend(sharedWorkspace, "activateFileViewerSelectingURLs:", urlArray);
            }

            public static implicit operator IntPtr(NSURL nsUrl) => nsUrl.URLPtr;
        }

        public struct NSString
        {
            public readonly IntPtr StrPtr;

            public NSString(string aString)
            {
                IntPtr nsString = objc_getClass("NSString");
                StrPtr = IntPtr_objc_msgSend(nsString, "stringWithUTF8String:", aString);
            }

            public static implicit operator IntPtr(NSString nsString) => nsString.StrPtr;
        }

        public struct NSPoint
        {
            public double X;
            public double Y;

            public NSPoint(double x, double y)
            {
                X = x;
                Y = y;
            }
        }

        public struct NSRect
        {
            public NSPoint Pos;
            public NSPoint Size;

            public NSRect(double x, double y, double width, double height)
            {
                Pos = new NSPoint(x, y);
                Size = new NSPoint(width, height);
            }
        }
    }
}