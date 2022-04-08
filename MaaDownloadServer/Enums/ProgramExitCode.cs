namespace MaaDownloadServer.Enums;

public struct ProgramExitCode
{
    public const int ConfigurationProviderIsNull = -1;
    public const int NoPythonInterpreter = -2;
    public const int ScriptDoNotHaveConfigFile = -3;
    public const int FailedToParseScriptConfigFile = -4;
    public const int FailedToCreatePythonVenv = -5;
}
