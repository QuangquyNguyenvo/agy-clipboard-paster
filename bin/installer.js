const fs = require('fs');
const path = require('path');
const { execSync } = require('child_process');

function install() {
    console.log('Installing agy-clipboard-paster native C# wrapper...');
    try {
        let agyDir = '';
        try {
            const output = execSync('where.exe agy', { encoding: 'utf8' }).trim();
            const lines = output.split(/\r?\n/);
            for (const line of lines) {
                if (line.endsWith('.exe')) {
                    agyDir = path.dirname(line);
                    break;
                }
            }
        } catch (e) {
            const defaultPath = path.join(process.env.USERPROFILE || 'C:\\Users\\Admin', 'AppData', 'Local', 'agy', 'bin');
            if (fs.existsSync(defaultPath)) {
                agyDir = defaultPath;
            }
        }

        if (!agyDir) {
            console.error('ERROR: Could not find agy.exe installation directory.');
            process.exit(1);
        }

        const realAgyPath = path.join(agyDir, 'agy_real.exe');
        const agyPath = path.join(agyDir, 'agy.exe');

        // 1. Backup original agy.exe to agy_real.exe
        if (!fs.existsSync(realAgyPath)) {
            if (fs.existsSync(agyPath)) {
                fs.renameSync(agyPath, realAgyPath);
                console.log(`Backed up original agy.exe to agy_real.exe`);
            } else {
                console.error(`ERROR: agy.exe not found in ${agyDir}`);
                process.exit(1);
            }
        }

        // 2. Locate C# wrapper source code
        const wrapperSourcePath = path.join(__dirname, '..', 'src', 'agy_wrapper.cs');

        // 3. Compile wrapper to agy.exe using csc.exe referencing Windows Forms & Drawing
        const cscCompiler = 'C:\\Windows\\Microsoft.NET\\Framework64\\v4.0.30319\\csc.exe';
        if (!fs.existsSync(cscCompiler)) {
            console.error('ERROR: C# compiler csc.exe not found at default location.');
            process.exit(1);
        }

        // Rename running agy.exe to a unique temporary name so we can overwrite it
        const oldAgyPath = path.join(agyDir, `agy_old_${Date.now()}.exe`);
        if (fs.existsSync(agyPath)) {
            try {
                fs.renameSync(agyPath, oldAgyPath);
                console.log(`Renamed running agy.exe to ${path.basename(oldAgyPath)}`);
            } catch (e) {
                console.warn('Warning: Could not rename running agy.exe. Attempting direct overwrite...');
            }
        }

        // Cleanup any older unlocked agy_old_*.exe files
        try {
            const files = fs.readdirSync(agyDir);
            for (const file of files) {
                if (file.startsWith('agy_old_') && file.endsWith('.exe')) {
                    try {
                        fs.unlinkSync(path.join(agyDir, file));
                    } catch (e) {
                        // File is locked (currently running), skip it
                    }
                }
            }
        } catch (e) {}

        const compileCmd = `"${cscCompiler}" /r:System.Windows.Forms.dll /r:System.Drawing.dll /out:"${agyPath}" "${wrapperSourcePath}"`;
        execSync(compileCmd);
        console.log(`Successfully compiled and installed native wrapper at ${agyPath}`);

    } catch (err) {
        console.error('Installation failed:', err.message);
        process.exit(1);
    }
}

function uninstall() {
    console.log('Uninstalling agy-clipboard-paster wrapper...');
    try {
        let agyDir = '';
        const defaultPath = path.join(process.env.USERPROFILE || 'C:\\Users\\Admin', 'AppData', 'Local', 'agy', 'bin');
        if (fs.existsSync(defaultPath)) {
            agyDir = defaultPath;
        }

        if (!agyDir) return;

        const realAgyPath = path.join(agyDir, 'agy_real.exe');
        const agyPath = path.join(agyDir, 'agy.exe');

        // Restore original agy.exe
        if (fs.existsSync(realAgyPath)) {
            if (fs.existsSync(agyPath)) {
                fs.unlinkSync(agyPath);
            }
            fs.renameSync(realAgyPath, agyPath);
            console.log('Restored original agy.exe successfully.');
        }
    } catch (err) {
        console.error('Uninstallation failed:', err.message);
    }
}

const action = process.argv[2];
if (action === 'install') {
    install();
} else if (action === 'uninstall') {
    uninstall();
} else {
    console.log('Usage: node installer.js [install|uninstall]');
}
