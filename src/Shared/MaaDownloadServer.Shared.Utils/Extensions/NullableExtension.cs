// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using System.Runtime.CompilerServices;

namespace MaaDownloadServer.Shared.Utils.Extensions;

public static class NullableExtension
{
    /// <summary>
    /// 检查对象不为 Null，否则抛出 <see cref="ArgumentNullException"/> 异常
    /// </summary>
    /// <param name="obj">对象</param>
    /// <param name="paramName"><see cref="CallerArgumentExpressionAttribute"/></param>
    /// <param name="memberName"><see cref="CallerMemberNameAttribute"/></param>
    /// <typeparam name="T">对象类型</typeparam>
    /// <returns>确保不为 Null 的对象</returns>
    /// <exception cref="ArgumentNullException">对象为 Null</exception>
    public static T NotNull<T>(this T? obj,
        [CallerArgumentExpression("obj")] string paramName = "UnknownParamName",
        [CallerMemberName] string memberName = "UnknownMemberName")
    {
        if (obj is not null)
        {
            return obj;
        }

        var typeName = typeof(T).FullName ?? "UnknownType";
        throw new ArgumentNullException(paramName, $"从 {memberName} 请求确认的 {typeName} {paramName} 为 Null");
    }
}
