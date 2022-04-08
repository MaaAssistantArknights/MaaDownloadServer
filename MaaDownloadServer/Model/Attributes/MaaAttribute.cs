namespace MaaDownloadServer.Model.Attributes;

public abstract class MaaAttribute : Attribute
{
    public abstract string GetValue();
}
