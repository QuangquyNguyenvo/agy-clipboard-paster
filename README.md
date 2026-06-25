<!-- SVG Header Banner -->
<p align="center">
  <svg width="100%" height="120" viewBox="0 0 800 120" fill="none" xmlns="http://www.w3.org/2000/svg">
    <rect width="100%" height="120" rx="10" fill="#0D1117" />
    <rect width="100%" height="120" rx="10" stroke="url(#border_grad)" stroke-width="2" />
    <text x="50%" y="48%" dominant-baseline="middle" text-anchor="middle" font-family="'Fira Code', monospace, sans-serif" font-size="34" font-weight="bold" fill="url(#title_grad)" filter="url(#glow_filter)">
      agy-clipboard-paster
    </text>
    <text x="50%" y="78%" dominant-baseline="middle" text-anchor="middle" font-family="'Inter', sans-serif" font-size="15" fill="#8B949E">
      ⚡ Native Alt+V clipboard image pasting for Google Antigravity CLI
    </text>
    <defs>
      <filter id="glow_filter" x="-20%" y="-20%" width="140%" height="140%">
        <feGaussianBlur stdDeviation="3" result="blur" />
        <feMerge>
          <feMergeNode in="blur" />
          <feMergeNode in="SourceGraphic" />
        </feMerge>
      </filter>
      <linearGradient id="border_grad" x1="0" y1="0" x2="800" y2="120" gradientUnits="userSpaceOnUse">
        <stop stop-color="#FF007A" />
        <stop offset="0.5" stop-color="#7928CA" />
        <stop offset="1" stop-color="#00DFD8" />
      </linearGradient>
      <linearGradient id="title_grad" x1="200" y1="0" x2="600" y2="0" gradientUnits="userSpaceOnUse">
        <stop stop-color="#00FFF0" />
        <stop offset="0.5" stop-color="#7928CA" />
        <stop offset="1" stop-color="#FF007A" />
      </linearGradient>
    </defs>
  </svg>
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
<!-- Animated Footer -->
<p align="center">
  <svg width="200" height="50" viewBox="0 0 200 50" fill="none" xmlns="http://www.w3.org/2000/svg">
    <g filter="url(#glow)">
      <path d="M20 25C50 5 150 45 180 25" stroke="url(#footer_gradient)" stroke-width="3" stroke-linecap="round"/>
    </g>
    <defs>
      <filter id="glow" x="0" y="0" width="200" height="50" filterUnits="userSpaceOnUse">
        <feGaussianBlur stdDeviation="3" result="blur"/>
        <feMerge>
          <feMergeNode in="blur"/>
          <feMergeNode in="SourceGraphic"/>
        </feMerge>
      </filter>
      <linearGradient id="footer_gradient" x1="20" y1="25" x2="180" y2="25" gradientUnits="userSpaceOnUse">
        <stop stop-color="#00FFF0" />
        <stop offset="0.5" stop-color="#7928CA" />
        <stop offset="1" stop-color="#FF007A" />
      </linearGradient>
    </defs>
  </svg>
  <br />
  <sub>Built with ⚡ for Google Antigravity Community</sub>
</p>
