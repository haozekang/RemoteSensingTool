#define MyAppName "Remote Sensing Tool"
#define MyAppVersion "1.2"
#define MyAppPublisher "Dr.Kang"
#define MyAppURL "http://www.haozekang.com/"
#define MyAppExeName "RemoteSensingTool.exe"
[Setup]
AppId={{E19F07E8-BB27-11E9-8248-1C1B0DF614E9}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\RemoteSensingTool
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
;LicenseFile=C:\Users\kang\Documents\DakaPath2\杭州大伽软件安装协议.rtf
OutputBaseFilename=Setup
SetupIconFile=C:\Users\Administrator\Documents\Visual Studio 2015\Projects\RemoteSensingTool\RemoteSensingTool\bin\Release\tools2.ico
WizardSmallImageFile=C:\Users\Administrator\Documents\Visual Studio 2015\Projects\RemoteSensingTool\RemoteSensingTool\bin\Release\tools2.bmp
Compression=lzma
SolidCompression=yes
PrivilegesRequired=admin
[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: checkablealone;
[Files]
;Source: "C:\Users\Administrator\Desktop\SchoolSystem\locales\*"; DestDir: "{app}\locales"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "C:\Users\Administrator\Documents\Visual Studio 2015\Projects\RemoteSensingTool\RemoteSensingTool\bin\Release\tools2.ico"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Administrator\Documents\Visual Studio 2015\Projects\RemoteSensingTool\RemoteSensingTool\bin\Release\config.ini"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Administrator\Documents\Visual Studio 2015\Projects\RemoteSensingTool\RemoteSensingTool\bin\Release\RemoteSensingTool.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Administrator\Documents\Visual Studio 2015\Projects\RemoteSensingTool\RemoteSensingTool\bin\Release\spei.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Administrator\Documents\Visual Studio 2015\Projects\RemoteSensingTool\RemoteSensingTool\bin\Release\ControlzEx.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Administrator\Documents\Visual Studio 2015\Projects\RemoteSensingTool\RemoteSensingTool\bin\Release\HtmlAgilityPack.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Administrator\Documents\Visual Studio 2015\Projects\RemoteSensingTool\RemoteSensingTool\bin\Release\ICSharpCode.SharpZipLib.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Administrator\Documents\Visual Studio 2015\Projects\RemoteSensingTool\RemoteSensingTool\bin\Release\Kang.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Administrator\Documents\Visual Studio 2015\Projects\RemoteSensingTool\RemoteSensingTool\bin\Release\MahApps.Metro.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Administrator\Documents\Visual Studio 2015\Projects\RemoteSensingTool\RemoteSensingTool\bin\Release\MahApps.Metro.SimpleChildWindow.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Administrator\Documents\Visual Studio 2015\Projects\RemoteSensingTool\RemoteSensingTool\bin\Release\Microsoft.mshtml.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Administrator\Documents\Visual Studio 2015\Projects\RemoteSensingTool\RemoteSensingTool\bin\Release\Microsoft.Threading.Tasks.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Administrator\Documents\Visual Studio 2015\Projects\RemoteSensingTool\RemoteSensingTool\bin\Release\Microsoft.Threading.Tasks.Extensions.Desktop.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Administrator\Documents\Visual Studio 2015\Projects\RemoteSensingTool\RemoteSensingTool\bin\Release\Microsoft.Threading.Tasks.Extensions.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Administrator\Documents\Visual Studio 2015\Projects\RemoteSensingTool\RemoteSensingTool\bin\Release\MySql.Data.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Administrator\Documents\Visual Studio 2015\Projects\RemoteSensingTool\RemoteSensingTool\bin\Release\NPOI.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Administrator\Documents\Visual Studio 2015\Projects\RemoteSensingTool\RemoteSensingTool\bin\Release\NPOI.OOXML.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Administrator\Documents\Visual Studio 2015\Projects\RemoteSensingTool\RemoteSensingTool\bin\Release\NPOI.OpenXml4Net.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Administrator\Documents\Visual Studio 2015\Projects\RemoteSensingTool\RemoteSensingTool\bin\Release\NPOI.OpenXmlFormats.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Administrator\Documents\Visual Studio 2015\Projects\RemoteSensingTool\RemoteSensingTool\bin\Release\O2S.Components.PDFRender4NET.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Administrator\Documents\Visual Studio 2015\Projects\RemoteSensingTool\RemoteSensingTool\bin\Release\SHZConvertNative.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Administrator\Documents\Visual Studio 2015\Projects\RemoteSensingTool\RemoteSensingTool\bin\Release\Spire.License.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Administrator\Documents\Visual Studio 2015\Projects\RemoteSensingTool\RemoteSensingTool\bin\Release\Spire.Pdf.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Administrator\Documents\Visual Studio 2015\Projects\RemoteSensingTool\RemoteSensingTool\bin\Release\System.IO.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Administrator\Documents\Visual Studio 2015\Projects\RemoteSensingTool\RemoteSensingTool\bin\Release\System.Net.Http.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Administrator\Documents\Visual Studio 2015\Projects\RemoteSensingTool\RemoteSensingTool\bin\Release\System.Runtime.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Administrator\Documents\Visual Studio 2015\Projects\RemoteSensingTool\RemoteSensingTool\bin\Release\System.Threading.Tasks.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Administrator\Documents\Visual Studio 2015\Projects\RemoteSensingTool\RemoteSensingTool\bin\Release\System.Windows.Interactivity.dll"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:ProgramOnTheWeb,{#MyAppName}}"; Filename: "{#MyAppURL}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{userdesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}";
[Code]
function SetUninstallIcon(iconPath:string): Boolean;
var
  InstalledVersion,SubKeyName: String;
begin
if (IsWin64()) then begin
SubKeyName :=  'Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\{4729B530-0A8D-4944-A41A-5F4F4A5F7E64}_is1';
    RegWriteStringValue(HKLM64,SubKeyName,'DisplayIcon',iconPath);
  end else begin
SubKeyName :=  'Software\Microsoft\Windows\CurrentVersion\Uninstall\{4729B530-0A8D-4944-A41A-5F4F4A5F7E64}_is1';
    RegWriteStringValue(HKLM,SubKeyName,'DisplayIcon',iconPath);
  end;
end;

procedure CurPageChanged(curpage: integer);
begin
if curpage = wpFinished then
  begin
    SetUninstallIcon(ExpandConstant('{app}\icon.ico'));
  end;
end;
[Run]
;Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilentinished then begin SetUninstallIcon(ExpandConstant('{app}\icon.ico')); end; end; [Run] ;Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent