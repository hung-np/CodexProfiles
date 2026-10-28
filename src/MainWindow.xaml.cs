using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows;
using Microsoft.Win32;
namespace CodexProfiles;
public record Profile(string Id, string Name);
public partial class MainWindow : Window {
 readonly string root=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"CodexProfileManager");
 readonly ObservableCollection<Profile> items=new();
 string store=>Path.Combine(root,"profiles.json");
 public MainWindow(){
 InitializeComponent(); Profiles.ItemsSource=items;
 try { Directory.CreateDirectory(root); if(File.Exists(store)) foreach(var p in JsonSerializer.Deserialize<List<Profile>>(File.ReadAllText(store))??[]) { if(Guid.TryParseExact(p.Id,"N",out _)) items.Add(p); } }
 catch(Exception e){ Status.Text="Không đọc được danh sách; dữ liệu cũ được giữ lại. "+e.Message; IsEnabled=false; }
 }
 void Save(){ var temp=store+".tmp"; File.WriteAllText(temp,JsonSerializer.Serialize(items)); File.Move(temp,store,true); }
 void Add(object s,RoutedEventArgs e){var name=ProfileName.Text.Trim(); if(name.Length==0)return; if(items.Any(p=>p.Name.Equals(name,StringComparison.OrdinalIgnoreCase))){Status.Text="Tên profile đã tồn tại.";return;} var p=new Profile(Guid.NewGuid().ToString("N"),name);items.Add(p); try{Save();ProfileName.Clear();Profiles.SelectedItem=p;}catch(Exception ex){items.Remove(p);Status.Text=ex.Message;}}
 void Remove(object s,RoutedEventArgs e){var chosen=Profiles.SelectedItems.Cast<Profile>().ToList();if(chosen.Count==0)return;if(MessageBox.Show("Xóa các mục đã chọn khỏi danh sách? Thư mục và thông tin đăng nhập vẫn được giữ lại.","Xóa profile",MessageBoxButton.YesNo)!=MessageBoxResult.Yes)return;try{foreach(var p in chosen)items.Remove(p);Save();Status.Text="Đã xóa mục; dữ liệu vẫn giữ trong thư mục profiles.";}catch(Exception ex){foreach(var p in chosen)if(!items.Contains(p))items.Add(p);Status.Text=ex.Message;}}
 void OpenData(object s,RoutedEventArgs e){try{Process.Start(new ProcessStartInfo(root){UseShellExecute=true});}catch(Exception ex){Status.Text=ex.Message;}}
 string? FindApp(){
 var saved=Path.Combine(root,"app-path.txt");if(File.Exists(saved)&&File.Exists(File.ReadAllText(saved)))return File.ReadAllText(saved);
 try {var parent=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),"WindowsApps");foreach(var dir in Directory.GetDirectories(parent,"OpenAI.Codex_*").OrderDescending()){var path=Path.Combine(dir,"app","ChatGPT.exe");if(File.Exists(path))return path;}}catch{}
 var picker=new OpenFileDialog{Title="Chọn ChatGPT.exe trong thư mục cài Codex",Filter="ChatGPT.exe|ChatGPT.exe",InitialDirectory=@"C:\Program Files\WindowsApps"};
 if(picker.ShowDialog()==true){File.WriteAllText(saved,picker.FileName);return picker.FileName;}return null;
 }
 void Launch(object s,RoutedEventArgs e){
 var chosen=Profiles.SelectedItems.Cast<Profile>().ToList();if(chosen.Count==0){Status.Text="Hãy chọn ít nhất một profile.";return;}
 try{var exe=FindApp();if(exe is null)return;foreach(var p in chosen){
 var folder=Path.Combine(root,"profiles",p.Id);var home=Path.Combine(folder,"codex-home");var data=Path.Combine(folder,"app-data");Directory.CreateDirectory(home);Directory.CreateDirectory(data);
 var config=Path.Combine(home,"config.toml");if(!File.Exists(config))File.WriteAllText(config,"cli_auth_credentials_store = \"file\"\n");
 var start=new ProcessStartInfo(exe){UseShellExecute=false};start.Environment["CODEX_HOME"]=home;start.Environment["CODEX_ELECTRON_USER_DATA_PATH"]=data;start.ArgumentList.Add("--user-data-dir="+data);Process.Start(start);
 }Status.Text="Đã gửi yêu cầu mở "+chosen.Count+" profile. Kiểm tra email trong từng cửa sổ Codex.";
 }catch(Exception ex){Status.Text="Không mở được Codex: "+ex.Message;}
 }
}
