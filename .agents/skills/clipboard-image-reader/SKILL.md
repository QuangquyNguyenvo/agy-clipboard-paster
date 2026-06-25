---
name: clipboard-image-reader
description: "Triggers when prompt mentions '[#image-1]', '[#image-2]', etc., and views the corresponding compressed clipboard_temp_*.jpg files."
---

# Clipboard Image Reader Skill

This skill automatically detects and reads Claude-style image tags (`[#image-1]`, `[#image-2]`, etc.) pasted via the `Alt + V` hotkey.

## How to Trigger
This skill triggers when:
- The user prompt contains tags like `[#image-1]`, `[#image-2]`, `[#image-3]`, etc.

## Execution Steps
1. For each `[#image-N]` tag found in the user prompt:
   - Identify the file path: `clipboard_temp_N.jpg` located in the user's home profile directory.
     - Path template: `<UserProfile>/clipboard_temp_N.jpg`
     - On Windows, resolve `<UserProfile>` from the environment variable `%USERPROFILE%` or the home directory of the current user (e.g. `C:/Users/Admin/` for user `Admin`).
     - Example: `C:/Users/Admin/clipboard_temp_N.jpg`.
   - Use the `view_file` tool to load this image.
2. Analyze the loaded image(s) to address the user's specific request.
3. Clean up: Once you have fully read and analyzed all images, delete all temp files using the `run_command` tool:
   ```powershell
   powershell -Command "Remove-Item -Force $env:USERPROFILE/clipboard_temp_*.jpg"
   ```
