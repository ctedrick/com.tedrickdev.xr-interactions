using UnityEditor;

namespace TedrickDev.InteractionsToolkit.Utility
{
    public static class Extensions
    {
        public static string ConvertToProjectRelativePath(this string path) => FileUtil.GetProjectRelativePath(path);
    }
}