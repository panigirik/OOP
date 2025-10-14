using System.Runtime.InteropServices;

namespace ConsoleWord;

public sealed class EditorSettings
{
    public static readonly Lazy<EditorSettings> _instance = new(() => new EditorSettings());
    public static EditorSettings Instance => _instance.Value;

    public int FontSize { get; set; } = 12;

    public EditorSettings() { }

    public void DisplaySettings()
    {
        Console.WriteLine($"Current Settings: Font Size: {FontSize}");
    }

    public void ConfigureEditorSettings()
    {
        var settings = EditorSettings.Instance;

        settings.DisplaySettings();

        Console.Write("Enter new font size (current: " + settings.FontSize + "): ");
        if (int.TryParse(Console.ReadLine(), out int fontSize) && fontSize > 0)
        {
            settings.FontSize = fontSize;
            Console.WriteLine("Font size updated!");
            SetConsoleFontSize(fontSize);
        }
        else
        {
            Console.WriteLine("Invalid input. Font size must be a positive integer.");
        }

        settings.DisplaySettings();
    }
    

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool SetCurrentConsoleFontEx(
        IntPtr consoleOutput,
        bool maximumWindow,
        ref CONSOLE_FONT_INFO_EX consoleCurrentFontEx);

    private const int STD_OUTPUT_HANDLE = -11;

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct CONSOLE_FONT_INFO_EX
    {
        public int cbSize;
        public int nFont;
        public COORD dwFontSize;
        public int FontFamily;
        public int FontWeight;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string FaceName;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct COORD
    {
        public short X;
        public short Y;

        public COORD(short x, short y)
        {
            X = x;
            Y = y;
        }
    }

    private void SetConsoleFontSize(int fontSize)
    {
        IntPtr hnd = GetStdHandle(STD_OUTPUT_HANDLE);

        var info = new CONSOLE_FONT_INFO_EX();
        info.cbSize = Marshal.SizeOf<CONSOLE_FONT_INFO_EX>();
        info.FaceName = "Consolas";
        info.dwFontSize = new COORD(0, (short)fontSize);
        info.FontFamily = 54; 
        info.FontWeight = 400;

        bool result = SetCurrentConsoleFontEx(hnd, false, ref info);

        if (!result)
        {
            Console.WriteLine("Unable to change console font size. Make sure you are running on Windows and not in Visual Studio's output window.");
        }
    }
}
