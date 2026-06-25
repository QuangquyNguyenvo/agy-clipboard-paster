<!-- Animated SVG Header Banner -->
<p align="center">
  <img src="assets/header.svg" alt="agy-clipboard-paster Banner" width="100%" />
</p>

<!-- Badges (Single line to prevent wrapping issues) -->
<p align="center"><a href="https://www.npmjs.com/package/agy-clipboard-paster"><img src="https://img.shields.io/npm/v/agy-clipboard-paster.svg?style=flat-square&color=FF007A" alt="npm version"></a>&nbsp;<a href="https://www.npmjs.com/package/agy-clipboard-paster"><img src="https://img.shields.io/npm/dm/agy-clipboard-paster.svg?style=flat-square&color=7928CA" alt="npm downloads"></a>&nbsp;<a href="https://github.com/QuangquyNguyenvo/agy-clipboard-paster"><img src="https://img.shields.io/badge/platform-windows-blue.svg?style=flat-square&logo=windows" alt="platform"></a>&nbsp;<a href="LICENSE"><img src="https://img.shields.io/github/license/QuangquyNguyenvo/agy-clipboard-paster.svg?style=flat-square&color=00DFD8" alt="license"></a></p>

<!-- SVG Divider -->
<p align="center">
  <svg width="100%" height="20" viewBox="0 0 1200 20" fill="none" xmlns="http://www.w3.org/2000/svg">
    <path d="M0 10 H1200" stroke="url(#paint0_gradient)" stroke-width="2" stroke-dasharray="8 4" />
    <defs>
      <linearGradient id="paint0_gradient" x1="0" y1="0" x2="1200" y2="0" gradientUnits="userSpaceOnUse">
        <stop stop-color="#FF007A" />
        <stop offset="0.5" stop-color="#7928CA" />
        <stop offset="1" stop-color="#00DFD8" />
      </linearGradient>
    </defs>
  </svg>
</p>

## 📌 Overview
**`agy-clipboard-paster`** is a native Windows global shortcut utility that brings **instant Alt+V image pasting** to Google's **Antigravity CLI** (`agy`), similar to Claude Code.

---

## ✨ Features
* **⚡ Native `Alt + V`**: Paste screenshots/images directly into the active terminal chat prompt.
* **🚀 Zero-Lag Uploads**: Automatically compresses screenshots into lightweight JPEGs (75% quality, <150KB) for instant API upload.
* **👻 100% Silent**: Runs hidden under the hood. No command prompt windows flashing on screen.
* **🔄 Zero-Dependency Node.js Installation**: No Python, C++, or external build tools required.

---

## 🚀 Installation

Install globally via npm:

```bash
npm install -g agy-clipboard-paster
```
*The installer automatically handles backing up `agy.exe` and compiling the wrapper.*

---

## 🎮 How to Use

1. Copy any image or take a screenshot (`Win + Shift + S`).
2. Focus the `agy` terminal and press **`Alt + V`**.
3. A tag like **`[#image-1]`** will be typed into your input. 
4. Write your request next to it and press **Enter**.

---

## 🛠️ Uninstallation

Restore the original `agy.exe` binary:

```bash
npm uninstall -g agy-clipboard-paster
```

<!-- SVG Divider -->
<p align="center">
  <svg width="100%" height="20" viewBox="0 0 1200 20" fill="none" xmlns="http://www.w3.org/2000/svg">
    <path d="M0 10 H1200" stroke="url(#paint1_gradient)" stroke-width="2" stroke-dasharray="8 4" />
    <defs>
      <linearGradient id="paint1_gradient" x1="0" y1="0" x2="1200" y2="0" gradientUnits="userSpaceOnUse">
        <stop stop-color="#FF007A" />
        <stop offset="0.5" stop-color="#7928CA" />
        <stop offset="1" stop-color="#00DFD8" />
      </linearGradient>
    </defs>
  </svg>
</p>

## 📄 License
This project is licensed under the MIT License.

---
<!-- Animated SVG Footer Banner -->
<p align="center">
  <img src="assets/footer.svg" alt="agy-clipboard-paster Footer" width="100%" />
</p>
