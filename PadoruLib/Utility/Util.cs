using System.IO;

namespace PadoruLib.Utility
{
    /// <summary>
    /// General Utility Class
    /// </summary>
    public static class Util
    {
        /// <summary>
        /// Create a Relative path from the given paths
        /// </summary>
        /// <param name="basePath">the base path the resulting path will be relative to</param>
        /// <param name="fullPath">the full (absolute) path to make relative</param>
        /// <returns>the relative path, or string.Empty if the full path does not start with the base path</returns>
        public static string MakeRelativePath(string basePath, string fullPath)
        {
            //check inputs
            if (string.IsNullOrWhiteSpace(basePath) || string.IsNullOrWhiteSpace(fullPath)) return string.Empty;

            //get paths in upper case
            string basePathUc = basePath.ToUpper();
            string fullPathUc = fullPath.ToUpper();

            //check that full path begins with the base path
            if (!fullPathUc.StartsWith(basePathUc)) return string.Empty;

            //remove base path from full path
            string relPath = fullPath.Substring(basePath.Length).TrimStart('/', '\\');
            return relPath;
        }

        /// <summary>
        /// Create a Absolute path from the given base and relative paths
        /// </summary>
        /// <param name="basePath">the base path the relative path is relative to</param>
        /// <param name="relativePath">the relative path to make absolute</param>
        /// <returns>the absolute path</returns>
        public static string MakeAbsolutePath(string basePath, string relativePath)
        {
            //build full path
            return Path.Combine(basePath, relativePath.TrimStart('/', '\\'));
        }
    }
}
