Unicode True
!include "MUI2.nsh"
Name "Console UART Pro"
OutFile "ConsoleUARTPro-Setup.exe"
InstallDir "$LOCALAPPDATA\Programs\ConsoleUARTPro"
RequestExecutionLevel user
!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_LICENSE "installer\LICENSE.txt"
!insertmacro MUI_PAGE_COMPONENTS
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES
!insertmacro MUI_LANGUAGE "English"
Section "Core application (required)" SEC_CORE
 SectionIn RO
 SetOutPath "$INSTDIR"
 File /r "publish\*.*"
 WriteUninstaller "$INSTDIR\Uninstall.exe"
 CreateDirectory "$SMPROGRAMS\Console UART Pro"
 CreateShortcut "$SMPROGRAMS\Console UART Pro\Console UART Pro.lnk" "$INSTDIR\ConsoleUARTPro.exe"
 CreateShortcut "$SMPROGRAMS\Console UART Pro\Uninstall.lnk" "$INSTDIR\Uninstall.exe"
 WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\ConsoleUARTPro" "DisplayName" "Console UART Pro"
 WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\ConsoleUARTPro" "UninstallString" '"$INSTDIR\Uninstall.exe"'
SectionEnd
Section /o "Desktop shortcut" SEC_DESKTOP
 CreateShortcut "$DESKTOP\Console UART Pro.lnk" "$INSTDIR\ConsoleUARTPro.exe"
SectionEnd
Section "Uninstall"
 Delete "$DESKTOP\Console UART Pro.lnk"
 RMDir /r "$SMPROGRAMS\Console UART Pro"
 RMDir /r "$INSTDIR"
 DeleteRegKey HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\ConsoleUARTPro"
SectionEnd
