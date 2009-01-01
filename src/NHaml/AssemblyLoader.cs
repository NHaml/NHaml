using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Runtime.InteropServices;

namespace NHaml
{
    /// <summary>
    /// 
    /// </summary>
    ///<remarks>
    /// http://blog.domaindotnet.com/2008/09/20/fusion_c_sharp_wrapper_for_linq_to_gac_access/
    /// SharpDevelop
    /// </remarks>
    public static class AssemblyLoader
    {
        public static Assembly Load(string assemblyName)
        {
            Assembly assembly;
            try
            {
               assembly = Assembly.Load(assemblyName);
            }
            catch (FileNotFoundException )
            {
                if (!assemblyName.Contains(","))
                {
                    string assemblyWithLargestVersion = null;
                    Version largestVersion = null;
                    var list = GetAssemblyList(assemblyName);
                    foreach (var assemblyFullName in list)
                    {
                        var split = assemblyFullName.Split(',');
                        var version = new Version(split[1].Replace("Version=",string.Empty));
                        if ((largestVersion == null) || (version > largestVersion))
                        {
                            largestVersion = version;
                            assemblyWithLargestVersion = assemblyFullName;
                        }
                    }
                    if (assemblyWithLargestVersion != null)
                    {
                        return Assembly.Load(assemblyWithLargestVersion);
                    }
                }
                throw;
            }
            return assembly;
        }
        public static List<string> GetAssemblyList(string name)
        {
            IApplicationContext applicationContext;
            IAssemblyEnum assemblyEnum;
            IAssemblyName assemblyName;
            var nameAndComma = name + ",";
            var assemblyList = new List<string>();
            CreateAssemblyEnum(out assemblyEnum, null, null, 2, 0);
            while (assemblyEnum.GetNextAssembly(out applicationContext, out assemblyName, 0) == 0)
            {
                uint nChars = 0;
                assemblyName.GetDisplayName(null, ref nChars, 0);

                var stringBuilder = new StringBuilder((int)nChars);
                assemblyName.GetDisplayName(stringBuilder, ref nChars, 0);

                var item = stringBuilder.ToString();
                //TODO: case insensitive?
                if (item.StartsWith(nameAndComma))
                {
                    assemblyList.Add(item);
                }
            }
            return assemblyList;
        }
        public static List<string> GetAssemblyList()
        {
            IApplicationContext applicationContext;
            IAssemblyEnum assemblyEnum;
            IAssemblyName assemblyName;

            var l = new List<string>();
            CreateAssemblyEnum(out assemblyEnum, null, null, 2, 0);
            while (assemblyEnum.GetNextAssembly(out applicationContext, out assemblyName, 0) == 0)
            {
                uint nChars = 0;
                assemblyName.GetDisplayName(null, ref nChars, 0);

                var sb = new StringBuilder((int)nChars);
                assemblyName.GetDisplayName(sb, ref nChars, 0);

                var s = sb.ToString();
                l.Add(s);
            }
            return l;
        }
        [DllImport("fusion.dll", CharSet = CharSet.Auto)]
        internal static extern int CreateAssemblyEnum(out IAssemblyEnum ppEnum, IApplicationContext pAppCtx, IAssemblyName pName, uint dwFlags, int pvReserved);
    }


    [ComImport, Guid("CD193BC0-B4BC-11D2-9833-00C04FC31D2E"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IAssemblyName
    {
        [PreserveSig]
        int Set(uint PropertyId, IntPtr pvProperty, uint cbProperty);

        [PreserveSig]
        int Get(uint PropertyId, IntPtr pvProperty, ref uint pcbProperty);

        [PreserveSig]
        int Finalize();

        [PreserveSig]
        int GetDisplayName([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder szDisplayName, ref uint pccDisplayName, uint dwDisplayFlags);

        [PreserveSig]
        int BindToObject(object refIID, object pAsmBindSink, IApplicationContext pApplicationContext, [MarshalAs(UnmanagedType.LPWStr)] string szCodeBase, long llFlags, int pvReserved, uint cbReserved, out int ppv);

        [PreserveSig]
        int GetName(ref uint lpcwBuffer, [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwzName);

        [PreserveSig]
        int GetVersion(out uint pdwVersionHi, out uint pdwVersionLow);

        [PreserveSig]
        int IsEqual(IAssemblyName pName, uint dwCmpFlags);

        [PreserveSig]
        int Clone(out IAssemblyName pName);
    }

    [ComImport, Guid("21B8916C-F28E-11D2-A473-00C04F8EF448"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IAssemblyEnum
    {
        [PreserveSig]
        int GetNextAssembly(out IApplicationContext ppAppCtx, out IAssemblyName ppName, uint dwFlags);

        [PreserveSig]
        int Reset();

        [PreserveSig]
        int Clone(out IAssemblyEnum ppEnum);
    }
    [ComImport, Guid("7C23FF90-33AF-11D3-95DA-00A024A85B51"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IApplicationContext
    {
        void SetContextNameObject(IAssemblyName pName);

        void GetContextNameObject(out IAssemblyName ppName);

        void Set([MarshalAs(UnmanagedType.LPWStr)] string szName, int pvValue, uint cbValue, uint dwFlags);

        void Get([MarshalAs(UnmanagedType.LPWStr)] string szName, out int pvValue, ref uint pcbValue, uint dwFlags);

        void GetDynamicDirectory(out int wzDynamicDir, ref uint pdwSize);
    }
}
