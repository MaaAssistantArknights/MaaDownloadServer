// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using MaaDownloadServer.Shared.Utils.Exceptions;

namespace MaaDownloadServer.Shared.Utils.Extensions;

public static class FileInfoExtension
{
    /// <summary>
    /// 确认文件存在，若文件不存在，则抛出 <see cref="FileNotFoundException"/> 异常
    /// </summary>
    /// <param name="fileInfo"><see cref="FileInfo"/> 实例</param>
    /// <param name="paramName"><see cref="CallerArgumentExpressionAttribute"/></param>
    /// <param name="memberName"><see cref="CallerMemberNameAttribute"/></param>
    /// <returns>确保文件存在的 <see cref="FileInfo"/> 实例</returns>
    /// <exception cref="FileNotFoundException">文件不存在异常</exception>
    public static FileInfo AssertExist(this FileInfo? fileInfo,
        [CallerArgumentExpression("fileInfo")] string paramName = "UnknownParamName",
        [CallerMemberName] string memberName = "UnknownMemberName")
    {
        var fi = fileInfo.NotNull(paramName, memberName);
        if (fi.Exists is false)
        {
            throw new FileNotFoundException($"从 {memberName} 请求确认的 FileInfo {paramName}，文件不存在", fi.FullName);
        }

        return fi;
    }

    /// <summary>
    /// 确认文件不存在，若文件存在，则抛出 <see cref="FileFoundException"/> 异常
    /// </summary>
    /// <param name="fileInfo"><see cref="FileInfo"/> 实例</param>
    /// <param name="paramName"><see cref="CallerArgumentExpressionAttribute"/></param>
    /// <param name="memberName"><see cref="CallerMemberNameAttribute"/></param>
    /// <returns>确保文件不存在的 <see cref="FileInfo"/> 实例</returns>
    /// <exception cref="FileFoundException">文件存在异常</exception>
    public static FileInfo AssertNotExist(this FileInfo? fileInfo,
        [CallerArgumentExpression("fileInfo")] string paramName = "UnknownParamName",
        [CallerMemberName] string memberName = "UnknownMemberName")
    {
        var fi = fileInfo.NotNull(paramName, memberName);
        if (fi.Exists is false)
        {
            throw new FileFoundException($"从 {memberName} 请求确认的 FileInfo {paramName}，文件存在", fi.FullName);
        }

        return fi;
    }

    /// <summary>
    /// 确保文件不存在
    /// </summary>
    /// <param name="fileInfo"><see cref="FileInfo"/> 实例</param>
    /// <returns>确保文件不存在的 <see cref="FileInfo"/> 实例</returns>
    public static FileInfo EnsureDeleted(this FileInfo fileInfo)
    {
        if (fileInfo.Exists)
        {
            fileInfo.Delete();
        }

        return fileInfo;
    }

    /// <summary>
    /// 获取文件 MD5 校验码
    /// </summary>
    /// <param name="fileInfo"><see cref="FileInfo"/> 实例</param>
    /// <returns>文件 MD5 校验码</returns>
    public static string GetMd5(this FileInfo fileInfo)
    {
        var md5 = MD5.Create();
        var stream = fileInfo.OpenRead();
        var hashBytes = md5.ComputeHash(stream);
        stream.Close();
        var hashStr = Convert.ToBase64String(hashBytes)
            .Replace("-", string.Empty)
            .ToLower();
        return hashStr;
    }

    /// <summary>
    /// 异步获取文件 MD5 校验码
    /// </summary>
    /// <param name="fileInfo"><see cref="FileInfo"/> 实例</param>
    /// <returns>文件 MD5 校验码</returns>
    public static async Task<string> GetMd5Async(this FileInfo fileInfo)
    {
        var md5 = MD5.Create();
        var stream = fileInfo.OpenRead();
        var hashBytes = await md5.ComputeHashAsync(stream);
        stream.Close();
        var hashStr = Convert.ToBase64String(hashBytes)
            .Replace("-", string.Empty)
            .ToLower();
        return hashStr;
    }

    /// <summary>
    /// 检查两个文件的 MD5 校验码是否一致
    /// </summary>
    /// <param name="fileInfo"><see cref="FileInfo"/> 实例</param>
    /// <param name="another"><see cref="FileInfo"/> 实例</param>
    /// <returns>文件是否一致</returns>
    public static bool IsSameMd5With(this FileInfo fileInfo, FileInfo another)
    {
        var hash1 = fileInfo.GetMd5();
        var hash2 = another.GetMd5();
        return hash1 == hash2;
    }
}
