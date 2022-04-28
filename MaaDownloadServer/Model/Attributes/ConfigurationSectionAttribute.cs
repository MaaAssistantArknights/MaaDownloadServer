namespace MaaDownloadServer.Model.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ConfigurationSectionAttribute : MaaAttribute
{
    private readonly string _sectionName;

    public ConfigurationSectionAttribute(string sectionName)
    {
        _sectionName = sectionName;
    }

    public override string GetValue()
    {
        return _sectionName;
    }
}
