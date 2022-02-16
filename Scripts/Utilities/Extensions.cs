using UnityEditor;

namespace TedrickDev.HandPoser.Utilities
{
    public static class Extensions
    {
        public static string ConvertToProjectRelativePath(this string path) => FileUtil.GetProjectRelativePath(path);
    }
}