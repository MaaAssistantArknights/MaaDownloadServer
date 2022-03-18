namespace MaaDownloadServer.Extensions;

public static class FileSystemExtension
{
    public static void CopyTo(this DirectoryInfo srcPath, string destPath)
    {
        Directory.CreateDirectory(destPath);
        Parallel.ForEach(srcPath.GetDirectories("*", SearchOption.AllDirectories),
            srcInfo => Directory.CreateDirectory($"{destPath}{srcInfo.FullName[srcPath.FullName.Length..]}"));
        Parallel.ForEach(srcPath.GetFiles("*", SearchOption.AllDirectories),
            srcInfo => File.Copy(srcInfo.FullName, $"{destPath}{srcInfo.FullName[srcPath.FullName.Length..]}", true));
    }
}
