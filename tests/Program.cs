using CodexProfiles;
if(args.Contains("--child")){Thread.Sleep(60000);return;}
var root=Path.Combine(Path.GetTempPath(),"CodexProfilesTests-"+Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(root);
var parent=Path.Combine(root,"profiles");Directory.CreateDirectory(parent);
var a=Guid.NewGuid().ToString("N");var b=Guid.NewGuid().ToString("N");
Directory.CreateDirectory(Path.Combine(parent,a,"app-data"));File.WriteAllText(Path.Combine(parent,a,"app-data","cookie"),"fake-session");
Directory.CreateDirectory(Path.Combine(parent,b));File.WriteAllText(Path.Combine(parent,b,"sentinel"),"keep");
foreach(var bad in new[]{"..","../outside","","invalid"}){try{ProfileStorage.Delete(parent,bad);throw new Exception("Invalid ID accepted");}catch(IOException){}}
var locked=Path.Combine(parent,a,"lock");
using(var handle=new FileStream(locked,FileMode.Create,FileAccess.ReadWrite,FileShare.None)){
 try{ProfileStorage.Delete(parent,a);throw new Exception("Locked file unexpectedly deleted");}catch(IOException){}
}
ProfileStorage.Delete(parent,a);
if(Directory.Exists(Path.Combine(parent,a))||!File.Exists(Path.Combine(parent,b,"sentinel")))throw new Exception("Deletion boundary failure");
ProfileStorage.Delete(parent,a);
Console.WriteLine("PASS: invalid IDs rejected, locked files fail, nested data removed, sibling preserved, missing directory tolerated.");
// Only generated test files exist here.
if(Path.GetFullPath(root).StartsWith(Path.GetFullPath(Path.GetTempPath()),StringComparison.OrdinalIgnoreCase))Directory.Delete(root,true);
var data=Path.Combine(Path.GetTempPath(),"Fake Profile","app-data");
string Cmd(string path)=>"ChatGPT.exe --user-data-dir=\""+path+"\"";
if(!ProfileProcesses.Matches(Cmd(data),data))throw new Exception("Exact quoted profile not matched");
if(ProfileProcesses.Matches(Cmd(data+"-other"),data))throw new Exception("Prefix false positive");
if(ProfileProcesses.Matches("ChatGPT.exe --title=\""+data+"\"",data))throw new Exception("Unrelated argument matched");
if(ProfileProcesses.Matches("ChatGPT.exe",data))throw new Exception("Default app matched");
var fake=new List<ProcessRow>{
 new(101,1,Cmd(data),"2026-10-28T00:00:00Z"),
 new(102,101,"codex.exe app-server","2026-10-28T00:00:00Z"),
 new(103,1,"ChatGPT.exe","2026-10-28T00:00:00Z"),
 new(104,103,"codex.exe app-server","2026-10-28T00:00:00Z")};
var owned=ProfileProcesses.Owned(fake,data);
if(owned.Count!=2||owned.Any(x=>x.ProcessId==103||x.ProcessId==104))throw new Exception("Process ownership crossed profile boundary");
Console.WriteLine("PASS: exact user-data path, spaced paths, unrelated arguments, default app exclusion, backend descendants.");

if(args.Contains("--process-integration")){
 var fakePath=Path.Combine(Path.GetTempPath(),"CodexProfileProcessTest-"+Guid.NewGuid().ToString("N"),"app-data");
 var start=new System.Diagnostics.ProcessStartInfo(Environment.ProcessPath!){UseShellExecute=false,CreateNoWindow=true};
 start.ArgumentList.Add("--child");start.ArgumentList.Add("--user-data-dir="+fakePath);
 using var child=System.Diagnostics.Process.Start(start)!;
 try{
 await Task.Delay(500);
 var snap=await ProfileProcesses.Snapshot(); if(!ProfileProcesses.Owned(snap,fakePath).Any(p=>p.ProcessId==child.Id))throw new Exception("Fake child not detected");
await ProfileProcesses.Stop(fakePath);
 child.Refresh();if(!child.HasExited)throw new Exception("Fake child not stopped");
 Console.WriteLine("PASS: live Windows snapshot detects and stops only generated fake profile process.");
 }finally{if(!child.HasExited)child.Kill();}
}



