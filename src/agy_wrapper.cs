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
    private static readonly string _tempFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

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

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetConsoleWindow();

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CloseHandle(IntPtr hObject);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr CreateToolhelp32Snapshot(uint dwFlags, uint th32ProcessID);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    private static extern bool Process32First(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    private static extern bool Process32Next(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private struct PROCESSENTRY32
    {
        public uint dwSize;
        public uint cntUsage;
        public uint th32ProcessID;
        public IntPtr th32DefaultHeapID;
        public uint th32ModuleID;
        public uint cntThreads;
        public uint th32ParentProcessID;
        public int pcPriClassBase;
        public uint dwFlags;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szExeFile;
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

    [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    private static extern short GetKeyState(int virtualKeyCode);

    [StructLayout(LayoutKind.Sequential)]
    private struct INPUT
    {
        public uint type;
        public INPUT_UNION union;
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct INPUT_UNION
    {
        [FieldOffset(0)]
        public MOUSEINPUT mi;
        [FieldOffset(0)]
        public KEYBDINPUT ki;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MOUSEINPUT
    {
        public int dx;
        public int dy;
        public uint mouseData;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct KEYBDINPUT
    {
        public ushort wVk;
        public ushort wScan;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    private const int INPUT_KEYBOARD = 1;
    private const uint KEYEVENTF_UNICODE = 0x0004;
    private const uint KEYEVENTF_KEYUP = 0x0002;

    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    static void Main(string[] args)
    {
        Log("agy_wrapper started. Args: " + string.Join(" ", args));

        // Cleanup any temporary JPEG/PNG files from previous runs
        try
        {
            string[] files = Directory.GetFiles(_tempFolder, "clipboard_temp_*.*");
            foreach (string file in files)
            {
                string ext = Path.GetExtension(file).ToLower();
                if (ext == ".jpg" || ext == ".jpeg" || ext == ".png")
                {
                    File.Delete(file);
                }
            }
        }
        catch {}

        // 1. Start the keyboard hook loop in a background STA thread
        System.Threading.Thread hookThread = new System.Threading.Thread(() =>
        {
            _hookID = SetHook(_proc);
            Log("SetHook completed. Hook ID: " + _hookID + ", LastError: " + Marshal.GetLastWin32Error());
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
                string[] files = Directory.GetFiles(_tempFolder, "clipboard_temp_*.jpg");
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

            // Read flags from lParam to check for Alt down (offset 8 bytes)
            int flags = Marshal.ReadInt32(lParam, 8);
            bool altPressed = (flags & 0x20) != 0 || wParam == (IntPtr)WM_SYSKEYDOWN;

            if (key == Keys.V && altPressed)
            {
                bool isConsoleActive = IsConsoleActive();
                Log("Alt+V detected. IsConsoleActive=" + isConsoleActive);
                if (isConsoleActive)
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

    private static bool IsConsoleActive()
    {
        IntPtr activeWindow = GetForegroundWindow();
        if (activeWindow == IntPtr.Zero)
        {
            Log("IsConsoleActive: activeWindow is Zero");
            return false;
        }

        uint activeProcessId;
        GetWindowThreadProcessId(activeWindow, out activeProcessId);

        uint currentProcessId = (uint)Process.GetCurrentProcess().Id;
        Log("IsConsoleActive check: activeWindow=" + activeWindow + ", activeProcessId=" + activeProcessId + ", currentProcessId=" + currentProcessId);

        if (activeProcessId == currentProcessId)
        {
            Log("IsConsoleActive: Match activeProcessId == currentProcessId");
            return true;
        }

        // Check if the active process is our child (e.g., agy_real.exe)
        uint activeParentPid = GetParentProcessId(activeProcessId);
        Log("IsConsoleActive check: activeParentPid=" + activeParentPid);
        if (activeParentPid == currentProcessId)
        {
            Log("IsConsoleActive: Match activeParentPid == currentProcessId");
            return true;
        }

        // Trace ancestors
        uint checkPid = currentProcessId;
        int depth = 0;
        while (checkPid != 0 && depth < 10)
        {
            uint parentPid = GetParentProcessId(checkPid);
            Log("IsConsoleActive check: checkPid=" + checkPid + " parentPid=" + parentPid);
            if (parentPid == 0) break;
            if (parentPid == activeProcessId)
            {
                Log("IsConsoleActive: Match parentPid == activeProcessId (" + parentPid + ")");
                return true;
            }
            checkPid = parentPid;
            depth++;
        }

        // Fallback: Check if the foreground window is our console window handle
        IntPtr consoleWindow = GetConsoleWindow();
        Log("IsConsoleActive check: consoleWindow=" + consoleWindow);
        if (consoleWindow != IntPtr.Zero && consoleWindow == activeWindow)
        {
            Log("IsConsoleActive: Match consoleWindow == activeWindow");
            return true;
        }

        Log("IsConsoleActive: No match found, returning false");
        return false;
    }

    private static uint GetParentProcessId(uint processId)
    {
        uint parentPid = 0;
        IntPtr handle = CreateToolhelp32Snapshot(2, 0); // TH32CS_SNAPPROCESS = 2
        if (handle != IntPtr.Zero)
        {
            PROCESSENTRY32 pe = new PROCESSENTRY32();
            pe.dwSize = (uint)Marshal.SizeOf(typeof(PROCESSENTRY32));
            if (Process32First(handle, ref pe))
            {
                do
                {
                    if (pe.th32ProcessID == processId)
                    {
                        parentPid = pe.th32ParentProcessID;
                        break;
                    }
                } while (Process32Next(handle, ref pe));
            }
            CloseHandle(handle);
        }
        return parentPid;
    }

    private static void HandleClipboardPaste()
    {
        Log("HandleClipboardPaste: Called");
        try
        {
            int slot = 1;
            string filepath = "";
            while (slot <= 5)
            {
                filepath = Path.Combine(_tempFolder, "clipboard_temp_" + slot + ".jpg");
                if (!File.Exists(filepath))
                {
                    break;
                }
                slot++;
            }
            if (slot > 5)
            {
                // All 5 slots are occupied. Overwrite the oldest one based on LastWriteTime.
                DateTime oldestTime = DateTime.MaxValue;
                int oldestSlot = 1;
                for (int i = 1; i <= 5; i++)
                {
                    string path = Path.Combine(_tempFolder, "clipboard_temp_" + i + ".jpg");
                    if (File.Exists(path))
                    {
                        DateTime writeTime = File.GetLastWriteTime(path);
                        if (writeTime < oldestTime)
                        {
                            oldestTime = writeTime;
                            oldestSlot = i;
                        }
                    }
                }
                slot = oldestSlot;
                filepath = Path.Combine(_tempFolder, "clipboard_temp_" + slot + ".jpg");
            }
            Log("HandleClipboardPaste: Slot=" + slot + ", Path=" + filepath);

            IDataObject clipboardData = null;
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    clipboardData = Clipboard.GetDataObject();
                    break;
                }
                catch (System.Runtime.InteropServices.ExternalException ex)
                {
                    Log("HandleClipboardPaste: Clipboard retry " + i + " failed: " + ex.Message);
                    System.Threading.Thread.Sleep(50);
                }
            }
            if (clipboardData != null)
            {
                Image imgToSave = null;
                bool shouldDisposeImage = false;

                if (clipboardData.GetDataPresent(DataFormats.Bitmap))
                {
                    imgToSave = (Image)clipboardData.GetData(DataFormats.Bitmap);
                }
                else if (clipboardData.GetDataPresent(DataFormats.FileDrop))
                {
                    string[] filepaths = (string[])clipboardData.GetData(DataFormats.FileDrop);
                    if (filepaths != null && filepaths.Length > 0)
                    {
                        string firstFile = filepaths[0];
                        string ext = Path.GetExtension(firstFile).ToLower();
                        if (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".gif" || ext == ".bmp" || ext == ".tiff")
                        {
                            try
                            {
                                imgToSave = Image.FromFile(firstFile);
                                shouldDisposeImage = true;
                            }
                            catch {}
                        }
                    }
                }

                if (imgToSave != null)
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
                            imgToSave.Save(filepath, jpgCodec, encoderParams);
                        }
                    }
                    else
                    {
                        imgToSave.Save(filepath, ImageFormat.Jpeg);
                    }

                    if (shouldDisposeImage)
                    {
                        imgToSave.Dispose();
                    }

                    Log("HandleClipboardPaste: Saved image to " + filepath + ". Sending unicode text...");
                    // Simulate key typing directly
                    SendUnicodeText("[#image-" + slot + "] ");
                }
                else
                {
                    Log("HandleClipboardPaste: No image or file drop found in clipboard. Formats: " + string.Join(", ", clipboardData.GetFormats()));
                }
            }
            else
            {
                Log("HandleClipboardPaste: Clipboard data is null");
            }
        }
        catch (Exception ex)
        {
            Log("Error pasting clipboard image: " + ex.Message + "\nStack trace: " + ex.StackTrace);
            Console.Error.WriteLine("\nError pasting clipboard image: " + ex.Message);
        }
    }

    private static void SendUnicodeText(string text)
    {
        const ushort VK_SHIFT = 0x10;
        const ushort VK_CONTROL = 0x11;
        const ushort VK_MENU = 0x12; // Alt

        // Check if modifiers are pressed so we can release them
        bool isShiftDown = (GetKeyState(VK_SHIFT) & 0x8000) != 0;
        bool isCtrlDown = (GetKeyState(VK_CONTROL) & 0x8000) != 0;
        bool isAltDown = (GetKeyState(VK_MENU) & 0x8000) != 0;

        int releaseCount = 0;
        if (isShiftDown) releaseCount++;
        if (isCtrlDown) releaseCount++;
        if (isAltDown) releaseCount++;

        int textCount = text.Length * 2;
        INPUT[] inputs = new INPUT[releaseCount + textCount + releaseCount];
        int idx = 0;

        // 1. Release modifiers
        if (isShiftDown)
        {
            inputs[idx].type = (uint)INPUT_KEYBOARD;
            inputs[idx].union.ki.wVk = VK_SHIFT;
            inputs[idx].union.ki.wScan = 0;
            inputs[idx].union.ki.dwFlags = KEYEVENTF_KEYUP;
            idx++;
        }
        if (isCtrlDown)
        {
            inputs[idx].type = (uint)INPUT_KEYBOARD;
            inputs[idx].union.ki.wVk = VK_CONTROL;
            inputs[idx].union.ki.wScan = 0;
            inputs[idx].union.ki.dwFlags = KEYEVENTF_KEYUP;
            idx++;
        }
        if (isAltDown)
        {
            inputs[idx].type = (uint)INPUT_KEYBOARD;
            inputs[idx].union.ki.wVk = VK_MENU;
            inputs[idx].union.ki.wScan = 0;
            inputs[idx].union.ki.dwFlags = KEYEVENTF_KEYUP;
            idx++;
        }

        // 2. Send the unicode text characters
        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];
            
            // Key down
            inputs[idx].type = (uint)INPUT_KEYBOARD;
            inputs[idx].union.ki.wVk = 0;
            inputs[idx].union.ki.wScan = (ushort)c;
            inputs[idx].union.ki.dwFlags = KEYEVENTF_UNICODE;
            idx++;
            
            // Key up
            inputs[idx].type = (uint)INPUT_KEYBOARD;
            inputs[idx].union.ki.wVk = 0;
            inputs[idx].union.ki.wScan = (ushort)c;
            inputs[idx].union.ki.dwFlags = KEYEVENTF_UNICODE | KEYEVENTF_KEYUP;
            idx++;
        }

        // 3. Restore modifiers
        if (isAltDown)
        {
            inputs[idx].type = (uint)INPUT_KEYBOARD;
            inputs[idx].union.ki.wVk = VK_MENU;
            inputs[idx].union.ki.wScan = 0;
            inputs[idx].union.ki.dwFlags = 0; // Key down
            idx++;
        }
        if (isCtrlDown)
        {
            inputs[idx].type = (uint)INPUT_KEYBOARD;
            inputs[idx].union.ki.wVk = VK_CONTROL;
            inputs[idx].union.ki.wScan = 0;
            inputs[idx].union.ki.dwFlags = 0; // Key down
            idx++;
        }
        if (isShiftDown)
        {
            inputs[idx].type = (uint)INPUT_KEYBOARD;
            inputs[idx].union.ki.wVk = VK_SHIFT;
            inputs[idx].union.ki.wScan = 0;
            inputs[idx].union.ki.dwFlags = 0; // Key down
            idx++;
        }

        uint sent = SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
        if (sent != inputs.Length)
        {
            Log("SendUnicodeText failed: sent " + sent + " of " + inputs.Length + " inputs. LastError: " + Marshal.GetLastWin32Error());
        }
        else
        {
            Log("SendUnicodeText succeeded: sent " + sent + " inputs.");
        }
    }

    private static void Log(string message)
    {
        try
        {
            string logPath = Path.Combine(_tempFolder, "agy_wrapper_debug.log");
            File.AppendAllText(logPath, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " - " + message + Environment.NewLine);
        }
        catch {}
    }
}
