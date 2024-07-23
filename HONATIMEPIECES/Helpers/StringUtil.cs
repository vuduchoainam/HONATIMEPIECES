using System.Text.RegularExpressions;
using System.Text;

namespace HONATIMEPIECES.Helpers
{
    public static class StringUtil
    {
        public static string RemoveVietnameseDiacritics(string str)
        {
            str = Regex.Replace(str, "à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ|À|Á|Ạ|Ả|Ã|Â|Ầ|Ấ|Ậ|Ẩ|Ẫ|Ă|Ằ|Ắ|Ặ|Ẳ|Ẵ", "a");
            str = Regex.Replace(str, "è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ|È|É|Ẹ|Ẻ|Ẽ|Ê|Ề|Ế|Ệ|Ể|Ễ", "e");
            str = Regex.Replace(str, "ì|í|ị|ỉ|ĩ|Ì|Í|Ị|Ỉ|Ĩ", "i");
            str = Regex.Replace(str, "ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ|Ò|Ó|Ọ|Ỏ|Õ|Ô|Ồ|Ố|Ộ|Ổ|Ỗ|Ơ|Ờ|Ớ|Ợ|Ở|Ỡ", "o");
            str = Regex.Replace(str, "ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ|Ù|Ú|Ụ|Ủ|Ũ|Ư|Ừ|Ứ|Ự|Ử|Ữ", "u");
            str = Regex.Replace(str, "ỳ|ý|ỵ|ỷ|ỹ|Ỳ|Ý|Ỵ|Ỷ|Ỹ", "y");
            str = str.Replace(" ", "-");
            str = str.Replace("đ", "d").Replace("Đ", "D");
            str = Regex.Replace(str.Normalize(NormalizationForm.FormD), @"[\u0300-\u036f]", string.Empty);
            return str;
        }

        public static string GenerateSlug(string name)
        {
            name = name.ToLower();
            name = RemoveVietnameseDiacritics(name);
            name = name.Trim('-');
            return string.IsNullOrEmpty(name) ? "default-slug" : name;
        }

        public static string FormatDate(DateTime? date)
        {
            return date?.ToString("dd/MM/yyyy hh:mm:ss tt");
        }
    }
}
