using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;

class Program
{
    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;
    private const int WM_SYSKEYDOWN = 0x0104;

    private static LowLevelKeyboardProc _proc = HookCallback;
    private static IntPtr _hookID = IntPtr.Zero;

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    static void Main(string[] args)
    {
        // 1. Start the keyboard hook loop in a background STA thread
        System.Threading.Thread hookThread = new System.Threading.Thread(() =>
        {
            _hookID = SetHook(_proc);
            Application.Run();
        });
        hookThread.SetApartmentState(System.Threading.ApartmentState.STA);
        hookThread.IsBackground = true;
        hookThread.Start();

        // 2. Start the real agy_real.exe process
        int exitCode = 0;
        try
        {
            string realAgyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "agy_real.exe");
            
            // Re-construct the arguments string
            string arguments = string.Join(" ", args.Select(a => a.Contains(" ") || a.Contains("\"") ? "\"" + a.Replace("\"", "\\\"") + "\"" : a));

            ProcessStartInfo agyInfo = new ProcessStartInfo
            {
                FileName = realAgyPath,
                Arguments = arguments,
                UseShellExecute = false // Inherits terminal streams
            };

            using (Process agy = Process.Start(agyInfo))
            {
                agy.WaitForExit();
                exitCode = agy.ExitCode;
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("Error running agy_real: " + ex.Message);
            exitCode = 1;
        }
        finally
        {
            // 3. Cleanup the keyboard hook on exit
            if (_hookID != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_hookID);
            }

            // Cleanup any temporary JPEG files
            try
            {
                string[] files = Directory.GetFiles("C:\\Users\\Admin", "clipboard_temp_*.jpg");
                foreach (string file in files)
                {
                    File.Delete(file);
                }
            }
            catch {}
        }

        Environment.Exit(exitCode);
    }

    private static IntPtr SetHook(LowLevelKeyboardProc proc)
    {
        using (Process curProcess = Process.GetCurrentProcess())
        using (ProcessModule curModule = curProcess.MainModule)
        {
            return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
        }
    }

    private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
        {
            int vkCode = Marshal.ReadInt32(lParam);
            Keys key = (Keys)vkCode;

            // Detect Alt+V
            bool altPressed = (Control.ModifierKeys & Keys.Alt) != 0 || 
                              (wParam == (IntPtr)WM_SYSKEYDOWN && key == Keys.V);

            if (key == Keys.V && altPressed)
            {
                // Check if our console window is currently active/foreground
                IntPtr activeWindow = GetForegroundWindow();
                uint activeProcessId;
                GetWindowThreadProcessId(activeWindow, out activeProcessId);

                if (activeProcessId == Process.GetCurrentProcess().Id)
                {
                    // Run clipboard capture in STA thread
                    System.Threading.Thread pasteThread = new System.Threading.Thread(HandleClipboardPaste);
                    pasteThread.SetApartmentState(System.Threading.ApartmentState.STA);
                    pasteThread.Start();
                    pasteThread.Join();

                    return (IntPtr)1; // Consume keypress
                }
            }
        }
        return CallNextHookEx(_hookID, nCode, wParam, lParam);
    }

    private static void HandleClipboardPaste()
    {
        try
        {
            int slot = 1;
            string filepath = "";
            while (slot <= 5)
            {
                filepath = $"C:\\Users\\Admin\\clipboard_temp_{slot}.jpg";
                if (!File.Exists(filepath))
                {
                    break;
                }
                slot++;
            }
            if (slot > 5)
            {
                slot = 1;
                filepath = "C:\\Users\\Admin\\clipboard_temp_1.jpg";
            }

            IDataObject clipboardData = Clipboard.GetDataObject();
            if (clipboardData != null && clipboardData.GetDataPresent(DataFormats.Bitmap))
            {
                using (Image img = (Image)clipboardData.GetData(DataFormats.Bitmap))
                {
                    // Find Jpeg codec
                    ImageCodecInfo jpgCodec = ImageCodecInfo.GetImageDecoders()
                        .FirstOrDefault(codec => codec.FormatID == ImageFormat.Jpeg.Guid);
                    
                    if (jpgCodec != null)
                    {
                        using (EncoderParameters encoderParams = new EncoderParameters(1))
                        using (EncoderParameter qualityParam = new EncoderParameter(Encoder.Quality, 75L))
                        {
                            encoderParams.Param[0] = qualityParam;
                            img.Save(filepath, jpgCodec, encoderParams);
                        }
                    }
                    else
                    {
                        img.Save(filepath, ImageFormat.Jpeg);
                    }
                }

                // Simulate key typing directly
                SendKeys.SendWait("[#image-" + slot + "] ");
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("\nError pasting clipboard image: " + ex.Message);
        }
    }
}
