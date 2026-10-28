using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;
namespace CodexProfiles;
public record ProcessRow(int ProcessId,int ParentProcessId,string? CommandLine,string CreationDate);
public static class ProfileProcesses {
 [DllImport("shell32.dll",SetLastError=true)] static extern IntPtr CommandLineToArgvW([MarshalAs(UnmanagedType.LPWStr)]string commandLine,out int count);
 [DllImport("kernel32.dll")] static extern IntPtr LocalFree(IntPtr memory);
 public static bool Matches(string? command,string dataPath){
 if(string.IsNullOrWhiteSpace(command))return false;
 var ptr=CommandLineToArgvW(command,out var count);if(ptr==IntPtr.Zero)return false;
 try{
 for(var i=1;i<count;i++){
 var arg=Marshal.PtrToStringUni(Marshal.ReadIntPtr(ptr,i*IntPtr.Size))??"";
 string? value=null;
 if(arg.Equals("--user-data-dir",StringComparison.OrdinalIgnoreCase)&&i+1<count)value=Marshal.PtrToStringUni(Marshal.ReadIntPtr(ptr,(i+1)*IntPtr.Size));
 else if(arg.StartsWith("--user-data-dir=",StringComparison.OrdinalIgnoreCase))value=arg["--user-data-dir=".Length..];
 if(value is not null)try{if(Path.GetFullPath(value).TrimEnd('\\','/').Equals(Path.GetFullPath(dataPath).TrimEnd('\\','/'),StringComparison.OrdinalIgnoreCase))return true;}catch{}
 }
 return false;
 }finally{LocalFree(ptr);}
 }
 public static async Task<List<ProcessRow>> Snapshot(){
 var start=new ProcessStartInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System),"WindowsPowerShell","v1.0","powershell.exe")){UseShellExecute=false,CreateNoWindow=true,RedirectStandardOutput=true,RedirectStandardError=true};
 start.ArgumentList.Add("-NoProfile");start.ArgumentList.Add("-NonInteractive");start.ArgumentList.Add("-Command");
 // All parent IDs are needed to identify backend descendants. No command lines are logged.
 start.ArgumentList.Add("$ErrorActionPreference='Stop'; $rows=@(Get-CimInstance Win32_Process | Where-Object { $_.CreationDate -and $_.ProcessId -gt 0 } | ForEach-Object { [pscustomobject]@{ProcessId=[int]$_.ProcessId;ParentProcessId=[int]$_.ParentProcessId;CommandLine=$_.CommandLine;CreationDate=$_.CreationDate.ToUniversalTime().ToString('o')} }); ConvertTo-Json -InputObject $rows -Compress");
 using var process=Process.Start(start)??throw new IOException(AppLanguage.T("Không đọc được tiến trình Windows."));
 var output=process.StandardOutput.ReadToEndAsync();var error=process.StandardError.ReadToEndAsync();
 using var timeout=new CancellationTokenSource(TimeSpan.FromSeconds(15));
 try{await process.WaitForExitAsync(timeout.Token);}catch(OperationCanceledException){try{process.Kill(true);}catch{}throw new IOException(AppLanguage.T("Kiểm tra tiến trình quá thời gian."));}
 var json=await output;await error;
 if(process.ExitCode!=0)throw new IOException(AppLanguage.T("Windows không cho kiểm tra tiến trình."));
 var rows=JsonSerializer.Deserialize<List<ProcessRow>>(json)??throw new IOException(AppLanguage.T("Không đọc được trạng thái."));
 // A missing command line for a Codex process cannot prove that a profile stopped.
 foreach(var row in rows)if(row.CommandLine is null){
 try{using var p=Process.GetProcessById(row.ProcessId);if(p.ProcessName.Equals("ChatGPT",StringComparison.OrdinalIgnoreCase)||p.ProcessName.Equals("Codex",StringComparison.OrdinalIgnoreCase))throw new IOException(AppLanguage.T("Không kiểm tra được một tiến trình Codex."));}catch(ArgumentException){}
 }
 return rows;
 }
 public static List<ProcessRow> Owned(List<ProcessRow> rows,string dataPath){
 var found=rows.Where(r=>Matches(r.CommandLine,dataPath)).ToList();
 bool added;do{added=false;foreach(var row in rows)if(!found.Any(p=>p.ProcessId==row.ProcessId)&&found.Any(p=>p.ProcessId==row.ParentProcessId && DateTime.Parse(p.CreationDate)<=DateTime.Parse(row.CreationDate))&&row.ProcessId!=Environment.ProcessId){found.Add(row);added=true;}}while(added);
 return found;
 }
 public static async Task Stop(string dataPath){
 for(var attempt=0;attempt<3;attempt++){
 var rows=await Snapshot();var owned=Owned(rows,dataPath);
 if(owned.Count==0)return;
 // Capture verified handles first. Never kill an entire unchecked process tree.
 var handles=new List<Process>();
 try{
 foreach(var row in owned.AsEnumerable().Reverse()){
 try{
 var p=Process.GetProcessById(row.ProcessId);
 if(Math.Abs((p.StartTime.ToUniversalTime()-DateTime.Parse(row.CreationDate).ToUniversalTime()).TotalSeconds)>1){p.Dispose();throw new IOException(AppLanguage.T("Tiến trình đã thay đổi; hãy thử lại."));}
 handles.Add(p);
 }catch(ArgumentException){}
 }
 foreach(var p in handles)try{if(!p.HasExited)p.Kill(false);}catch(InvalidOperationException){}
 foreach(var p in handles)try{using var timeout=new CancellationTokenSource(TimeSpan.FromSeconds(5));await p.WaitForExitAsync(timeout.Token);}catch(OperationCanceledException){}
 if(handles.Any(p=>!p.HasExited))throw new IOException(AppLanguage.T("Một tiến trình của profile chưa dừng hẳn."));
}finally{foreach(var p in handles)p.Dispose();}
 }
 if(Owned(await Snapshot(),dataPath).Count!=0)throw new IOException(AppLanguage.T("Profile vẫn còn tiến trình. Hãy thử dừng lại."));
 }
}

