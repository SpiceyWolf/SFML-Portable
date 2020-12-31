/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// TGUI - Texus' Graphical User Interface
// Copyright (C) 2012-2020 Bruno Van de Velde (vdv_b@TGUI.eu)
//
// This software is provided 'as-is', without any express or implied warranty.
// In no event will the authors be held liable for any damages arising from the use of this software.
//
// Permission is granted to anyone to use this software for any purpose,
// including commercial applications, and to alter it and redistribute it freely,
// subject to the following restrictions:
//
// 1. The origin of this software must not be misrepresented;
//    you must not claim that you wrote the original software.
//    If you use this software in a product, an acknowledgment
//    in the product documentation would be appreciated but is not required.
//
// 2. Altered source versions must be plainly marked as such,
//    and must not be misrepresented as being the original software.
//
// 3. This notice may not be removed or altered from any source distribution.
//
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Text;
using System.Security;
using System.Runtime.InteropServices;
using SFML.System;

namespace TGUI
{
    public static class Util
    {
        public static string GetStringFromC_UTF32(IntPtr source)
        {
            // Find the length of the source string (find the terminating 0)
            uint length = 0;
            unsafe
            {
                for (uint* ptr = (uint*)source.ToPointer(); *ptr != 0; ++ptr)
                    length++;
            }
                
            // Copy it to a byte array
            byte[] sourceBytes = new byte[length * 4];
            Marshal.Copy(source, sourceBytes, 0, sourceBytes.Length);

            // Convert it to a C# string
            return Encoding.UTF32.GetString(sourceBytes);
        }

        public static string GetStringFromC_ASCII(IntPtr source)
        {
            return Marshal.PtrToStringAnsi(source);
        }

        public static IntPtr ConvertStringForC_UTF32(string source)
        {
            // Copy the string to a null-terminated UTF-32 byte array
            byte[] utf32 = Encoding.UTF32.GetBytes(source + '\0');

            // Pass it to the C API
            unsafe
            {
                fixed (byte* ptr = utf32)
                {
                    return (IntPtr)ptr;
                }
            }
        }

        public static IntPtr ConvertStringForC_ASCII(string source)
        {
            // Copy the string to a null-terminated ANSI byte array
            byte[] bytes = Encoding.ASCII.GetBytes(source + '\0');

            // Pass it to the C API
            unsafe
            {
                fixed (byte* ptr = bytes)
                {
                    return (IntPtr)ptr;
                }
            }
        }

        public static Widget GetWidgetFromC(IntPtr widgetCPointer, Gui parentGui)
        {
            if (widgetCPointer == IntPtr.Zero)
                return null;

            var type = Type.GetType("TGUI." + Util.GetStringFromC_ASCII(tguiWidget_getWidgetType(widgetCPointer)));
            var widget = (Widget)Activator.CreateInstance(type, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, null, new object[]{ widgetCPointer }, null);
            widget.ParentGui = parentGui;
            return widget;
        }


        #region Imports

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private IntPtr tguiWidget_getWidgetType(IntPtr cPointer);

        #endregion
    }
}
