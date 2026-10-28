using System.IO;
namespace CodexProfiles;
public static class ProfileStorage {
 public static string Validate(string parent,string id){
 if(!Guid.TryParseExact(id,"N",out _))throw new IOException(AppLanguage.T("ID profile không hợp lệ."));
 var basePath=Path.GetFullPath(parent);
 var target=Path.GetFullPath(Path.Combine(basePath,id));
 for(var ancestor=new DirectoryInfo(basePath);ancestor!=null;ancestor=ancestor.Parent)if(ancestor.Exists&&(ancestor.Attributes&FileAttributes.ReparsePoint)!=0)throw new IOException(AppLanguage.T("Không xóa qua liên kết thư mục."));
 if(!string.Equals(Path.GetDirectoryName(target),basePath,StringComparison.OrdinalIgnoreCase))throw new IOException(AppLanguage.T("Đường dẫn không an toàn."));
 if(Directory.Exists(basePath)&&(File.GetAttributes(basePath)&FileAttributes.ReparsePoint)!=0)throw new IOException(AppLanguage.T("Không xóa qua liên kết thư mục."));
 if(Directory.Exists(target)) {
 var pending=new Stack<string>();pending.Push(target);
 while(pending.Count>0){
 var entry=pending.Pop();
 if((File.GetAttributes(entry)&FileAttributes.ReparsePoint)!=0)throw new IOException(AppLanguage.T("Profile chứa liên kết. Không xóa tự động."));
 if(Directory.Exists(entry))foreach(var child in Directory.EnumerateFileSystemEntries(entry))pending.Push(child);
 }
 }
 return target;
 }
 public static void Delete(string parent,string id){var path=Validate(parent,id);if(Directory.Exists(path))Directory.Delete(path,true);}
}

