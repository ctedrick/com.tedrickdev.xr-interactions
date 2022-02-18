using UnityEditor;

namespace TedrickDev.HandPoser.Utility
{
    public static class Extensions
    {
        public static string ConvertToProjectRelativePath(this string path) => FileUtil.GetProjectRelativePath(path);
    }
}