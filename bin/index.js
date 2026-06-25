const { GlobalKeyboardListener } = require("node-global-key-listener");
const { exec } = require("child_process");
const fs = require("fs");

const v = new GlobalKeyboardListener();

v.addListener(function (e, down) {
    // Detect Alt+V (LEFT ALT or RIGHT ALT + V)
    if (e.state === "DOWN" && e.name === "V" && (down["LEFT ALT"] || down["RIGHT ALT"])) {
        handleHotkey();
    }
});

function handleHotkey() {
    let slot = 1;
    let filepath = "";
    while (slot <= 5) {
        filepath = `C:\\Users\\Admin\\clipboard_temp_${slot}.jpg`;
        if (!fs.existsSync(filepath)) {
            break;
        }
        slot++;
    }
    if (slot > 5) {
        slot = 1;
        filepath = "C:\\Users\\Admin\\clipboard_temp_1.jpg";
    }

    // PowerShell script: saves clipboard image to JPEG at 75% quality, then types the tag
    const psCommand = `
Add-Type -AssemblyName System.Windows.Forms
Add-Type -AssemblyName System.Drawing
$img = [System.Windows.Forms::Clipboard]::GetImage()
if ($img -ne $null) {
    $encoder = [System.Drawing.Imaging.Encoder]::Quality
    $encoderParams = New-Object System.Drawing.Imaging.EncoderParameters(1)
    $encoderParams.Param[0] = New-Object System.Drawing.Imaging.EncoderParameter($encoder, 75)
    $codec = [System.Drawing.Imaging.ImageCodecInfo]::GetImageDecoders() | Where-Object { $_.FormatID -eq [System.Drawing.Imaging.ImageFormat]::Jpeg.Guid }
    $img.Save('${filepath.replace(/\\/g, '/')}', $codec, $encoderParams)
    $img.Dispose()
    [System.Windows.Forms::SendKeys]::SendWait("[#image-${slot}] ")
}
    `.trim();

    // Execute silently
    exec(`powershell -Command "${psCommand.replace(/\n/g, ' ')}"`, { windowsHide: true }, (err) => {
        if (err) {
            console.error("Error executing clipboard paste:", err);
        }
    });
}
