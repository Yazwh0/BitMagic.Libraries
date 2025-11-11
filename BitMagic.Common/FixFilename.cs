using System;
using System.IO;
using System.Linq;

namespace BitMagic.Common;

public static class FixFilenameExtension
{
    public static string FixFilename(this string filename)
    {
#if OS_WINDOWS
        if (filename[0] == '/')
            filename = filename[1..];

        string fullFilePath = Path.GetFullPath(filename);

        if (!File.Exists(fullFilePath))
        {
            return filename;
        }

        string fixedPath = "";
        foreach (string token in fullFilePath.Split('\\'))
        {
            //first token should be drive token
            if (fixedPath == "")
            {
                //fix drive casing
                string drive = string.Concat(token, "\\");
                drive = DriveInfo.GetDrives()
                    .First(driveInfo => driveInfo.Name.Equals(drive, StringComparison.OrdinalIgnoreCase)).Name;

                fixedPath = drive.ToLower();
            }
            else
            {
                fixedPath = Directory.GetFileSystemEntries(fixedPath, token).First();
            }
        }

        return fixedPath;
        //return char.ToLower(filename[0]) + filename[1..];
#endif
#if OS_LINUX
        return filename;
#endif
    }
}
