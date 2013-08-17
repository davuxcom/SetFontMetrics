using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SetFontMetrics
{
    public class Program
    {
        [StructLayout(LayoutKind.Sequential)]
        public class NONCLIENTMETRICS
        {
           public int cbSize;
           public int iBorderWidth;
           public int iScrollWidth;
           public int iScrollHeight;
           public int iCaptionWidth;
           public int iCaptionHeight;
           public LOGFONT   lfntCaptionFont;
           public int iSMCaptionWidth;
           public int iSMCaptionHeight;
           public LOGFONT lfntSMCaptionFont;
           public int iMenuWidth;
           public int iMenuHeight;
           public LOGFONT lfntMenuFont;
           public LOGFONT lfntStatusFont;
           public LOGFONT lfntMessageFont;
 
           public NONCLIENTMETRICS()
           {
              cbSize = Marshal.SizeOf(this);
           }
        }

        [StructLayout(LayoutKind.Sequential)]
        public class ICONMETRICS
        {
           public int cbSize;
           public int     iHorzSpacing;
           public int     iVertSpacing;
           public int     iTitleWrap;
           public LOGFONT lfFont;
 
           public ICONMETRICS()
           {
              cbSize = Marshal.SizeOf(this);
           }
        }
 
        [StructLayout(LayoutKind.Sequential)]
        public struct LOGFONT
        {
           public int lfHeight;
           public int lfWidth;
           public int lfEscapement;
           public int lfOrientation;
           public int lfWeight;
           public byte lfItalic;
           public byte lfUnderline;
           public byte lfStrikeOut;
           public byte lfCharSet;
           public byte lfOutPrecision;
           public byte lfClipPrecision;
           public byte lfQuality;
           public byte lfPitchAndFamily;
           [MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst=32)]
           public string lfFaceName;
 
           public static LOGFONT Empty
           {
              get
              ;set;
           }
        }

        const int SPI_GETNONCLIENTMETRICS = 41;
        const int SPI_SETNONCLIENTMETRICS = 42;

        const int SPI_GETICONMETRICS = 0x002D;
        const int SPI_SETICONMETRICS = 0x002E;

        public const int SPI_ICONVERTICALSPACING = 24;


        const int SPIF_SENDCHANGE = 0x2;
        const int SPIF_UPDATEINIFILE = 0x1;

        [DllImport("User32.dll")]
        public static extern bool SystemParametersInfo(int Action,
                                             int Param,
                                             IntPtr lpParam,
                                             int WinIni);

        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, int procedureName);

        [DllImport("kernel32.dll")]
        public static extern bool FreeLibrary(IntPtr hModule);



        public NONCLIENTMETRICS NCMetrics
        {
            get
            {
                NONCLIENTMETRICS ncm = new NONCLIENTMETRICS();

                int size = ncm.cbSize = Marshal.SizeOf(typeof(NONCLIENTMETRICS));
                IntPtr pncmetrics = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(ncm, pncmetrics, true);
                bool b = SystemParametersInfo(SPI_GETNONCLIENTMETRICS, size, pncmetrics, 0);
                Marshal.PtrToStructure(pncmetrics, ncm);
                Marshal.FreeHGlobal(pncmetrics);

                return ncm;
            }
            set
            {
                var ncm = value;
                int size = ncm.cbSize = Marshal.SizeOf(typeof(NONCLIENTMETRICS));
                IntPtr pncmetrics = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(ncm, pncmetrics, true);
                bool b = SystemParametersInfo(SPI_SETNONCLIENTMETRICS, size, pncmetrics, (SPIF_SENDCHANGE | SPIF_UPDATEINIFILE));
                Marshal.PtrToStructure(pncmetrics, ncm);
                Marshal.FreeHGlobal(pncmetrics);
            }
        }

        public ICONMETRICS IconMetrics
        {
            get
            {
                ICONMETRICS ncm = new ICONMETRICS();

                int size = ncm.cbSize = Marshal.SizeOf(typeof(ICONMETRICS));
                IntPtr pncmetrics = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(ncm, pncmetrics, true);
                bool b = SystemParametersInfo(SPI_GETICONMETRICS, size, pncmetrics, 0);
                Marshal.PtrToStructure(pncmetrics, ncm);
                Marshal.FreeHGlobal(pncmetrics);

                return ncm;
            }
            set
            {
                var ncm = value;
                int size = ncm.cbSize = Marshal.SizeOf(typeof(ICONMETRICS));
                IntPtr pncmetrics = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(ncm, pncmetrics, true);
                bool b = SystemParametersInfo(SPI_SETICONMETRICS, size, pncmetrics, (SPIF_SENDCHANGE | SPIF_UPDATEINIFILE));
                Marshal.PtrToStructure(pncmetrics, ncm);
                Marshal.FreeHGlobal(pncmetrics);
            }
        }

        public class AllMetrics
        {
            public ICONMETRICS IconMetrics;
            public NONCLIENTMETRICS NCMetrics;
        }


        public Program(string[] args)
        {
            /*
            // value sets, nothing seems to happen??
            Int32 ncm = new Int32();

            int size = Marshal.SizeOf(typeof(Int32));
            IntPtr pncmetrics = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(ncm, pncmetrics, true);
            bool b = SystemParametersInfo(SPI_ICONVERTICALSPACING, size, pncmetrics, 0);
            //Marshal.PtrToStructure(pncmetrics, ncm);
            ncm = Marshal.ReadInt32(pncmetrics);
            Marshal.FreeHGlobal(pncmetrics);

            b = SystemParametersInfo(SPI_ICONVERTICALSPACING, 94, IntPtr.Zero, 0);

            Console.WriteLine(ncm);

            SetVisualStyle();
            */
            
            try
            {
                if (args.Length == 0)
                {
                    Console.WriteLine(new AllMetrics { NCMetrics = this.NCMetrics, IconMetrics = this.IconMetrics }.ToXml().Remove(0,1));
                }
                else
                {
                    var am = File.ReadAllText(args[0]).FromXML<AllMetrics>();

                    IconMetrics = am.IconMetrics;
                    NCMetrics = am.NCMetrics;

                    if (args.Length == 2)
                    {
                        SetVisualStyle();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        static void Main(string[] args)
        {
            new Program(args);
        }

        void SetVisualStyle()
        {
            /*
            IntPtr pHandle = LoadLibrary(@"c:\windows\System32\uxtheme.dll");
            if (pHandle == IntPtr.Zero)
            {
                Console.WriteLine("Can't load UXTheme");
                return;
            }

            IntPtr pAddr = GetProcAddress(pHandle, 65); // (LPCSTR)MAKEINTRESOURCE(65)
            if (pAddr == IntPtr.Zero)
            {
                Console.WriteLine("Can't find SetSystemVisualStyle");
                return;
            }
            var SetSystemVisualStylePtr = (SetSystemVisualStyle)
                Marshal.GetDelegateForFunctionPointer(pAddr,
                typeof(SetSystemVisualStyle));
            */
            var aero = Environment.ExpandEnvironmentVariables(@"%SystemRoot%\Resources\Themes\Aero\Aero.msstyles");

            var x = SetSystemVisualStyle(aero, "NormalColor", "NormalSize", 0);

            Console.WriteLine("Theme Set: " + x);

            //FreeLibrary(pHandle);
        }

        [DllImport("UxTheme.Dll", EntryPoint = "#65", CharSet = CharSet.Unicode)]
        public static extern int SetSystemVisualStyle(string pszFilename, string pszColor, string pszSize, int dwReserved);
        /*
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int SetSystemVisualStyle(string a, string b, string c, int n);
        */
    }
}
