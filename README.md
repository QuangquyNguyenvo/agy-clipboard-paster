<!-- Animated SVG Header -->
<p align="center">
  <img src="assets/header.svg" alt="agy-clipboard-paster Banner" width="100%" />
</p>

<!-- Tightly grouped flat-square badges -->
<p align="center">
  <a href="https://www.npmjs.com/package/agy-clipboard-paster"><img src="https://img.shields.io/npm/v/agy-clipboard-paster.svg?style=flat-square&color=FF007A" alt="npm version"></a>&nbsp;
  <a href="https://www.npmjs.com/package/agy-clipboard-paster"><img src="https://img.shields.io/npm/dm/agy-clipboard-paster.svg?style=flat-square&color=7928CA" alt="npm downloads"></a>&nbsp;
  <a href="https://github.com/QuangquyNguyenvo/agy-clipboard-paster"><img src="https://img.shields.io/badge/platform-windows-blue.svg?style=flat-square&logo=windows" alt="platform"></a>&nbsp;
  <a href="LICENSE"><img src="https://img.shields.io/github/license/QuangquyNguyenvo/agy-clipboard-paster.svg?style=flat-square&color=00DFD8" alt="license"></a>
</p>

<!-- SVG Divider -->
<p align="center">
  <svg width="100%" height="10" viewBox="0 0 1200 10" fill="none" xmlns="http://www.w3.org/2000/svg">
    <path d="M0 5 H1200" stroke="url(#paint0_gradient)" stroke-width="2" stroke-dasharray="6 3" />
    <defs>
      <linearGradient id="paint0_gradient" x1="0" y1="0" x2="1200" y2="0" gradientUnits="userSpaceOnUse">
        <stop stop-color="#FF007A" />
        <stop offset="0.5" stop-color="#7928CA" />
        <stop offset="1" stop-color="#00DFD8" />
      </linearGradient>
    </defs>
  </svg>
</p>

## <img src="data:image/svg+xml;utf8,<svg xmlns='http://www.w3.org/2000/svg' width='24' height='24' viewBox='0 0 24 24' fill='none' stroke='%2300FFF0' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'><polygon points='13 2 3 14 12 14 11 22 21 10 12 10 13 2'></polygon></svg>" width="20" /> Features

* **⚡ Alt + V Paste**: Insert clipboard images directly into the active `agy` chat prompt.
* **🚀 Turbo Uploads**: Auto-compresses images into JPEGs (75% quality, <150KB) for instant API upload.
* **🔒 Sandbox Safe**: Hook is only active when the terminal window is in focus. Zero risk of keylogger flags.
* **🔄 Zero Dependencies**: 100% native C# wrapper compiled locally on Windows. No Python, Java, or Node runtime dependencies.

---

## <img src="data:image/svg+xml;utf8,<svg xmlns='http://www.w3.org/2000/svg' width='24' height='24' viewBox='0 0 24 24' fill='none' stroke='%237928CA' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'><path d='M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4'></path><polyline points='7 10 12 15 17 10'></polyline><line x1='12' y1='15' x2='12' y2='3'></line></svg>" width="20" /> Installation

Choose either **PowerShell** (no dependencies) or **NPM**:

### Method 1: PowerShell (Recommended)
```powershell
irm https://raw.githubusercontent.com/QuangquyNguyenvo/agy-clipboard-paster/main/install.ps1 | iex
```

### Method 2: NPM
```bash
npm install -g agy-clipboard-paster
```

---

## <img src="data:image/svg+xml;utf8,<svg xmlns='http://www.w3.org/2000/svg' width='24' height='24' viewBox='0 0 24 24' fill='none' stroke='%23FF007A' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'><polygon points='5 3 19 12 5 21 5 3'></polygon></svg>" width="20" /> How to Use

1. **Copy** any image or take a screenshot (`Win + Shift + S`).
2. **Focus** the `agy` terminal and press **`Alt + V`**.
3. **Send**: Type your request next to the **`[#image-1]`** tag and press **Enter**.

---

## <img src="data:image/svg+xml;utf8,<svg xmlns='http://www.w3.org/2000/svg' width='24' height='24' viewBox='0 0 24 24' fill='none' stroke='%23FF0000' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'><polyline points='3 6 5 6 21 6'></polyline><path d='M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2'></path><line x1='10' y1='11' x2='10' y2='17'></line><line x1='14' y1='11' x2='14' y2='17'></line></svg>" width="20" /> Uninstall

* **PowerShell**:
  ```powershell
  irm https://raw.githubusercontent.com/QuangquyNguyenvo/agy-clipboard-paster/main/uninstall.ps1 | iex
  ```
* **NPM**:
  ```bash
  npm uninstall -g agy-clipboard-paster
  ```

<!-- Animated SVG Footer -->
<p align="center">
  <img src="assets/footer.svg" alt="agy-clipboard-paster Footer" width="100%" />
</p>
