using System.Reflection;

namespace MaaDownloadServer.Utils;

public static class AttributeUtil
{
    public static string ReadAttributeValue<T, TAttribute>()
        where TAttribute : MaaAttribute where T : class
    {
        var attr = typeof(T).GetCustomAttribute<TAttribute>();

        return attr?.GetValue();
    }
}
