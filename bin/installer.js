const fs = require('fs');
const path = require('path');
const { execSync } = require('child_process');

function install() {
    console.log('Installing agy-clipboard-paster wrapper...');
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

        // 2. Prepare C# wrapper source code with absolute index.js path
        const wrapperTemplatePath = path.join(__dirname, '..', 'src', 'agy_wrapper.cs');
        let wrapperCode = fs.readFileSync(wrapperTemplatePath, 'utf8');
        
        const indexJsPath = path.join(__dirname, 'index.js').replace(/\\/g, '\\\\');
        wrapperCode = wrapperCode.replace('{{INDEX_JS_PATH}}', indexJsPath);

        const tempCsPath = path.join(__dirname, 'agy_wrapper_temp.cs');
        fs.writeFileSync(tempCsPath, wrapperCode, 'utf8');

        // 3. Compile wrapper to agy.exe using csc.exe
        const cscCompiler = 'C:\\Windows\\Microsoft.NET\\Framework64\\v4.0.30319\\csc.exe';
        if (!fs.existsSync(cscCompiler)) {
            console.error('ERROR: C# compiler csc.exe not found at default location.');
            process.exit(1);
        }

        const compileCmd = `"${cscCompiler}" /out:"${agyPath}" "${tempCsPath}"`;
        execSync(compileCmd);
        console.log(`Successfully compiled and installed wrapper at ${agyPath}`);

        // Cleanup temp file
        if (fs.existsSync(tempCsPath)) {
            fs.unlinkSync(tempCsPath);
        }
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
