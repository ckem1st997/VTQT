using System;
using System.IO;

namespace VTQT.Web.Framework.Helpers
{
    public static class PathHelper
    {
        public static class Images
        {
            public static class Flags
            {
                public const string RootPath = "/Content/global/img/flags/";
            }

            public static class Common
            {
                public const string NoAvatar = "/Content/global/img/default-avatar.gif";
                public const string NoImage = "/Content/global/img/default-image.jpg";
            }
        }

        public static class Helpers
        {
            /// <summary>Safe way to delete a file.</summary>
            public static bool Delete(string path)
            {
                if (path.IsEmpty())
                    return true;

                bool result = true;
                try
                {
                    if (Directory.Exists(path))
                    {
                        throw new MemberAccessException("Deleting folders cause of security reasons not possible: {0}".FormatWith(path));
                    }

                    System.IO.File.Delete(path);
                }
                catch (Exception exc)
                {
                    result = false;
                    exc.Dump();
                }
                return result;
            }

            /// <summary>Safe way to copy a file.</summary>
            public static bool Copy(string sourcePath, string destinationPath, bool overwrite = true, bool deleteSource = false)
            {
                bool result = true;
                try
                {
                    System.IO.File.Copy(sourcePath, destinationPath, overwrite);

                    if (deleteSource)
                        Delete(sourcePath);
                }
                catch (Exception exc)
                {
                    result = false;
                    exc.Dump();
                }
                return result;
            }

            public static void CopyDirectory(DirectoryInfo source, DirectoryInfo target, bool overwrite = true)
            {
                foreach (FileInfo fi in source.GetFiles())
                {
                    try
                    {
                        fi.CopyTo(Path.Combine(target.ToString(), fi.Name), overwrite);
                    }
                    catch (Exception exc)
                    {
                        exc.Dump();
                    }
                }

                foreach (DirectoryInfo sourceSubDir in source.GetDirectories())
                {
                    try
                    {
                        DirectoryInfo targetSubDir = target.CreateSubdirectory(sourceSubDir.Name);
                        CopyDirectory(sourceSubDir, targetSubDir, overwrite);
                    }
                    catch (Exception exc)
                    {
                        exc.Dump();
                    }
                }
            }

            public static string GetUniqueFilePath(string path)
            {
                var newPath = path;
                if (File.Exists(newPath))
                {
                    var fileNameOnly = Path.GetFileNameWithoutExtension(path);
                    var extension = Path.GetExtension(path);
                    var directoryName = Path.GetDirectoryName(path);
                    var newFileName = Path.GetFileName(path);
                    var count = 1;
                    do
                    {
                        var tmpFileName = string.Format("{0} ({1})", fileNameOnly, count++);
                        newFileName = tmpFileName + extension;
                        newPath = Path.Combine(directoryName, newFileName);
                    } while (File.Exists(newPath));
                }
                return newPath;
            }
        }
    }
}
