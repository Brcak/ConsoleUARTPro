# Console UART Pro

Python-free console UART diagnostic platform: Windows C#/.NET application and native C/C++ firmware.

## Current status

This repository is being initialized. The v0.4 native source files still need to be uploaded before the Windows EXE workflow can compile.

## Safety

Do not connect UART TX or power to an unidentified console header. Verify pinout and logic levels, and use a protected receive-only adapter first.

## Build

After uploading the contents of `ConsoleUARTPro_v0.4_Native_Source.zip` (the files *inside* the top-level folder), open GitHub Actions > Windows EXE. The workflow will restore, test, and publish a self-contained Windows executable.

Console-specific fault codes must be classified as verified, community-reported, or unverified; generic text patterns are not proof of a failed component.
