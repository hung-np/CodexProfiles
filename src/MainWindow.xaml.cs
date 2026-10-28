using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows;
using Microsoft.Win32;
namespace CodexProfiles;
public class Profile : System.ComponentModel.INotifyPropertyChanged {
 public string Id {get;set;} public string Name {get;set;}
 public Profile(string id,string name){Id=id;Name=name;}
 string state="Đang kiểm tra…";
 [System.Text.Json.Serialization.JsonIgnore] public string State {get=>state;set{state=value;PropertyChanged?.Invoke(this,new(nameof(State)));}}
 public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;
}
public partial class MainWindow : Window {
 readonly string root=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"CodexProfileManager");
 readonly ObservableCollection<Profile> items=new();
 readonly System.Windows.Threading.DispatcherTimer refreshTimer=new(){Interval=TimeSpan.FromSeconds(4)};
 bool checking;
 async Task RefreshStates(){
 if(checking||!IsEnabled)return;checking=true;
 try{var rows=await ProfileProcesses.Snapshot();foreach(var p in items){var count=ProfileProcesses.Owned(rows,DataPath(p)).Count;p.State=count==0?"● Đã dừng":"● Đang chạy · "+count+" tiến trình";}}
 catch{foreach(var p in items)p.State="● Không kiểm tra được";}
 finally{checking=false;}
 }
 string DataPath(Profile p)=>Path.Combine(root,"profiles",p.Id,"app-data");
 async void StopSelected(object s,RoutedEventArgs e){
 var selected=Profiles.SelectedItems.Cast<Profile>().ToList();
 if(selected.Count==0){Status.Text="Chọn profile cần dừng.";return;}
 if(MessageBox.Show("Dừng các profile đã chọn? Tác vụ đang chạy có thể bị ngắt. Dữ liệu đăng nhập được giữ nguyên.","Dừng profile",MessageBoxButton.OKCancel,MessageBoxImage.Warning)!=MessageBoxResult.OK)return;
 refreshTimer.Stop();IsEnabled=false;
 try{
 foreach(var p in selected){p.State="● Đang dừng…";await ProfileProcesses.Stop(DataPath(p));p.State="● Đã dừng";}
 Status.Text="Đã dừng các profile đã chọn. Thông tin đăng nhập vẫn được giữ.";
 }catch(Exception ex){Status.Text="Chưa dừng hoàn tất: "+ex.Message;foreach(var p in selected)p.State="● Cần kiểm tra lại";}
 finally{IsEnabled=true;await RefreshStates();refreshTimer.Start();}
 }
 string store=>Path.Combine(root,"profiles.json");
 public MainWindow(){
 InitializeComponent(); Profiles.ItemsSource=items; refreshTimer.Tick+=async (_,_)=>await RefreshStates(); Loaded+=async (_,_)=>{await RefreshStates();refreshTimer.Start();}; Closed+=(_,_)=>refreshTimer.Stop();
 try { Directory.CreateDirectory(root); if(File.Exists(store)) foreach(var p in JsonSerializer.Deserialize<List<Profile>>(File.ReadAllText(store))??[]) { if(Guid.TryParseExact(p.Id,"N",out _)) items.Add(p); } }
 catch(Exception e){ Status.Text="Không đọc được danh sách; dữ liệu cũ được giữ lại. "+e.Message; IsEnabled=false; }
 }
 void Save(){ var temp=store+".tmp"; File.WriteAllText(temp,JsonSerializer.Serialize(items)); File.Move(temp,store,true); }
 void Add(object s,RoutedEventArgs e){var name=ProfileName.Text.Trim(); if(name.Length==0)return; if(items.Any(p=>p.Name.Equals(name,StringComparison.OrdinalIgnoreCase))){Status.Text="Tên profile đã tồn tại.";return;} var p=new Profile(Guid.NewGuid().ToString("N"),name);items.Add(p); try{PrepareProfile(p);Save();ProfileName.Clear();Profiles.SelectedItem=p;Status.Text="Đã tạo profile mới. Chọn Mở profile đã chọn để đăng nhập Codex.";}catch(Exception ex){items.Remove(p);Status.Text=ex.Message;}}
 bool ConfirmDeletion(List<Profile> selected){
 var dialog=new Window{Title="Xóa profile và dữ liệu",Owner=this,Width=510,Height=350,ResizeMode=ResizeMode.NoResize,WindowStartupLocation=WindowStartupLocation.CenterOwner,Background=System.Windows.Media.Brushes.White};
 var panel=new System.Windows.Controls.StackPanel{Margin=new Thickness(28)};
 panel.Children.Add(new System.Windows.Controls.TextBlock{Text="Xóa vĩnh viễn "+selected.Count+" profile?",FontSize=23,FontWeight=FontWeights.SemiBold});
 var summary=new System.Windows.Controls.TextBlock{Text=string.Join(", ",selected.Select(p=>p.Name)),TextWrapping=TextWrapping.Wrap,Margin=new Thickness(0,14,0,14)};
 panel.Children.Add(new System.Windows.Controls.ScrollViewer{Content=summary,MaxHeight=70});
 panel.Children.Add(new System.Windows.Controls.TextBlock{Text="Xóa toàn bộ dữ liệu cục bộ của các profile này: đăng nhập, lịch sử và cấu hình. Không thể hoàn tác. Tài khoản và các profile khác không bị xóa.",TextWrapping=TextWrapping.Wrap});
 var buttons=new System.Windows.Controls.StackPanel{Orientation=System.Windows.Controls.Orientation.Horizontal,HorizontalAlignment=HorizontalAlignment.Right,Margin=new Thickness(0,22,0,0)};
 var cancel=new System.Windows.Controls.Button{Content="Giữ lại",IsCancel=true,MinWidth=100,Padding=new Thickness(14,10,14,10),Margin=new Thickness(0,0,10,0)};
 var delete=new System.Windows.Controls.Button{Content="Xóa vĩnh viễn",Background=System.Windows.Media.Brushes.Firebrick,Foreground=System.Windows.Media.Brushes.White,Padding=new Thickness(14,10,14,10)};
 cancel.Click+=(_,_)=>dialog.DialogResult=false;delete.Click+=(_,_)=>dialog.DialogResult=true;
 buttons.Children.Add(cancel);buttons.Children.Add(delete);panel.Children.Add(buttons);dialog.Content=panel;dialog.Loaded+=(_,_)=>cancel.Focus();
 return dialog.ShowDialog()==true;
 }
 async void Remove(object s,RoutedEventArgs e){
 var chosen=Profiles.SelectedItems.Cast<Profile>().ToList();if(chosen.Count==0){Status.Text="Chọn profile cần xóa.";return;}
 if(!ConfirmDeletion(chosen))return;
 IsEnabled=false;
 try{
 Status.Text="Đang kiểm tra các phiên Codex…";
 var rows=await ProfileProcesses.Snapshot();
 var parent=Path.Combine(root,"profiles");
 foreach(var p in chosen){ProfileStorage.Validate(parent,p.Id);if(ProfileProcesses.Owned(rows,DataPath(p)).Count>0)throw new IOException("Profile "+p.Name+" vẫn chạy nền. Chọn Dừng đã chọn, chờ Đã dừng rồi xóa lại.");}
 var removed=0;
 foreach(var p in chosen){ProfileStorage.Delete(parent,p.Id);items.Remove(p);try{Save();}catch{items.Add(p);throw;}removed++;}
 Status.Text="Đã xóa "+removed+" profile cùng toàn bộ dữ liệu đăng nhập cục bộ.";
 }catch(Exception ex){Status.Text="Không hoàn tất xóa: "+ex.Message;}finally{IsEnabled=true;}
 }
 void OpenData(object s,RoutedEventArgs e){try{Process.Start(new ProcessStartInfo(root){UseShellExecute=true});}catch(Exception ex){Status.Text=ex.Message;}}
 void PrepareProfile(Profile p){
 var folder=Path.Combine(root,"profiles",p.Id);
 var home=Path.Combine(folder,"codex-home");
 Directory.CreateDirectory(home);Directory.CreateDirectory(Path.Combine(folder,"app-data"));
 var config=Path.Combine(home,"config.toml");
 if(!File.Exists(config))File.WriteAllText(config,"cli_auth_credentials_store = \"file\"\n");
 }
 async Task<string?> FindApp(){
 // Query the current user's registered package, not the protected directory listing.
 try {
 var psi=new ProcessStartInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System),"WindowsPowerShell","v1.0","powershell.exe")){
 UseShellExecute=false,CreateNoWindow=true,RedirectStandardOutput=true,RedirectStandardError=true};
 psi.ArgumentList.Add("-NoProfile");psi.ArgumentList.Add("-NonInteractive");psi.ArgumentList.Add("-Command");
 psi.ArgumentList.Add("$ErrorActionPreference='Stop'; Get-AppxPackage -Name OpenAI.Codex | Sort-Object Version -Descending | ForEach-Object { Join-Path $_.InstallLocation 'app\\ChatGPT.exe' }");
 using var process=Process.Start(psi);
 if(process is not null){
 var output=process.StandardOutput.ReadToEndAsync();
 var error=process.StandardError.ReadToEndAsync();
 using var timeout=new CancellationTokenSource(TimeSpan.FromSeconds(15));
 try { await process.WaitForExitAsync(timeout.Token); }
 catch(OperationCanceledException){try{process.Kill(true);}catch{}throw new TimeoutException();}
 var paths=await output;await error;
 if(process.ExitCode==0)foreach(var candidate in paths.Split(new[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries)){
 var path=candidate.Trim();if(File.Exists(path))return path;
 }
 }
 } catch { }
 var saved=Path.Combine(root,"app-path.txt");
 if(File.Exists(saved)){var path=File.ReadAllText(saved).Trim();if(File.Exists(path))return path;}
 if(MessageBox.Show("Không tìm được gói Codex cho tài khoản Windows hiện tại. Hãy kiểm tra Codex đã cài và mở được từ Start. Bạn có muốn chọn ChatGPT.exe thủ công không? Không cần thay đổi quyền WindowsApps.","Không tìm thấy Codex",MessageBoxButton.YesNo,MessageBoxImage.Information)!=MessageBoxResult.Yes)return null;
 var picker=new OpenFileDialog{Title="Chọn ChatGPT.exe của Codex",Filter="ChatGPT.exe|ChatGPT.exe",InitialDirectory=Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)};
 if(picker.ShowDialog()==true){File.WriteAllText(saved,picker.FileName);return picker.FileName;}return null;
 }
 async void Launch(object s,RoutedEventArgs e){
 var chosen=Profiles.SelectedItems.Cast<Profile>().ToList();if(chosen.Count==0){Status.Text="Hãy chọn ít nhất một profile.";return;}
 try{IsEnabled=false;Status.Text="Đang tìm Codex…";var exe=await FindApp();if(exe is null){Status.Text="Chưa tìm được Codex. Profile đã tạo vẫn được giữ lại.";return;}foreach(var p in chosen){
 var folder=Path.Combine(root,"profiles",p.Id);var home=Path.Combine(folder,"codex-home");var data=Path.Combine(folder,"app-data");Directory.CreateDirectory(home);Directory.CreateDirectory(data);
 var config=Path.Combine(home,"config.toml");if(!File.Exists(config))File.WriteAllText(config,"cli_auth_credentials_store = \"file\"\n");
 var start=new ProcessStartInfo(exe){UseShellExecute=false};start.Environment["CODEX_HOME"]=home;start.Environment["CODEX_ELECTRON_USER_DATA_PATH"]=data;start.ArgumentList.Add("--user-data-dir="+data);Process.Start(start);
 }Status.Text="Đã gửi yêu cầu mở "+chosen.Count+" profile. Kiểm tra email trong từng cửa sổ Codex.";
 }catch(Exception ex){Status.Text="Không mở được Codex: "+ex.Message;}finally{IsEnabled=true;}
 }
}
