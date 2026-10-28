using System.Globalization;
using System.Windows.Data;
namespace CodexProfiles;

// Lấy chữ cái đầu của tên profile để hiện trên avatar.
public class InitialConverter : IValueConverter {
 public object Convert(object? value,Type targetType,object? parameter,CultureInfo culture){
  var text=(value as string)?.Trim();
  if(string.IsNullOrEmpty(text))return "?";
  return StringInfo.GetNextTextElement(text).ToUpperInvariant();
 }
 public object ConvertBack(object? value,Type targetType,object? parameter,CultureInfo culture)=>throw new NotSupportedException();
}
