using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        Process listener = null;
        try
        {
            // Start the node background listener silently
            ProcessStartInfo listenerInfo = new ProcessStartInfo
            {
                FileName = "node",
                Arguments = "\"{{INDEX_JS_PATH}}\"",
                UseShellExecute = false,
                CreateNoWindow = true
            };
            listener = Process.Start(listenerInfo);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("Failed to start clipboard listener: " + ex.Message);
        }

        int exitCode = 0;
        try
        {
            string realAgyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "agy_real.exe");
            string arguments = string.Join(" ", args.Select(a => a.Contains(" ") || a.Contains("\"") ? "\"" + a.Replace("\"", "\\\"") + "\"" : a));

            ProcessStartInfo agyInfo = new ProcessStartInfo
            {
                FileName = realAgyPath,
                Arguments = arguments,
                UseShellExecute = false
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
            if (listener != null && !listener.HasExited)
            {
                try
                {
                    listener.Kill();
                }
                catch {}
            }

            // Cleanup any stray listener instances
            try
            {
                ProcessStartInfo killInfo = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = "-Command \"Get-CimInstance Win32_Process -Filter \\\"Name = 'node.exe'\\\" | Where-Object { $_.CommandLine -like '*agy-clipboard-paster*' } | ForEach-Object { Stop-Process -Id $_.ProcessId -Force }\"",
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                using (Process killProc = Process.Start(killInfo))
                {
                    killProc.WaitForExit();
                }
            }
            catch {}
        }

        Environment.Exit(exitCode);
    }
}
