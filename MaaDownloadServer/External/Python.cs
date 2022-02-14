using System.Diagnostics;

namespace MaaDownloadServer.External;

public class Python
{
    public static bool EnvironmentCheck(ILogger logger, string pythonExecutable)
    {
        // 检查 Python 是否存在
        try
        {
            var pythonVersionProcessStartInfo = new ProcessStartInfo(pythonExecutable, "--version")
            {
                RedirectStandardOutput = true, RedirectStandardError = true
            };
            logger.LogInformation("运行 {cmd}", pythonVersionProcessStartInfo.FileName + " " + pythonVersionProcessStartInfo.Arguments);
            var pythonVersionProcess = Process.Start(pythonVersionProcessStartInfo);
            if (pythonVersionProcess is null)
            {
                logger.LogCritical("Python 环境检查失败，无法启动 Python 进程");
                return false;
            }
            var standardError = pythonVersionProcess.StandardError.ReadToEnd();
            var standardOutput = pythonVersionProcess.StandardOutput.ReadToEnd();
            if (standardError is not "")
            {
                logger.LogCritical("Python 环境检查失败，错误信息：{Err}", standardError);
                return false;
            }
            logger.LogInformation("Python 环境检查成功，版本：{Out}", standardOutput);
        }
        catch (Exception e)
        {
            logger.LogCritical(e, "检查 Python 环境时出现异常");
            return false;
        }

        // 检查 Python 是否存在 Pip
        try
        {
            var pipProcessStartInfo = new ProcessStartInfo(pythonExecutable, "-m pip --version")
            {
                RedirectStandardOutput = true, RedirectStandardError = true
            };
            logger.LogInformation("运行 {cmd}", pipProcessStartInfo.FileName + " " + pipProcessStartInfo.Arguments);
            var pipProcess = Process.Start(pipProcessStartInfo);
            if (pipProcess is null)
            {
                logger.LogCritical("Python Pip 环境检查失败，无法启动 pip 进程");
                return false;
            }

            var standardError = pipProcess.StandardError.ReadToEnd();
            var standardOutput = pipProcess.StandardOutput.ReadToEnd();
            if (standardError is not "")
            {
                logger.LogCritical("Python Pip 环境检查失败，错误信息：{Err}", standardError);
                return false;
            }

            logger.LogInformation("Python Pip 环境检查成功，pip 版本：{Out}", standardOutput);
        }
        catch (Exception e)
        {
            logger.LogCritical(e, "检查 Python Pip 环境时出现异常");
            return false;
        }

        // 检查是否安装了 virtualenv
        try
        {
            var virtualenvProcessStartInfo = new ProcessStartInfo(pythonExecutable, "-m pip show virtualenv")
            {
                RedirectStandardOutput = true, RedirectStandardError = true
            };
            logger.LogInformation("运行 {cmd}", virtualenvProcessStartInfo.FileName + " " + virtualenvProcessStartInfo.Arguments);
            var virtualenvProcess = Process.Start(virtualenvProcessStartInfo);
            if (virtualenvProcess is null)
            {
                logger.LogCritical("Python virtualenv 环境检查失败，无法启动 virtualenv 进程");
                return false;
            }

            var standardError = virtualenvProcess.StandardError.ReadToEnd();
            var standardOutput = virtualenvProcess.StandardOutput.ReadToEnd();
            if (standardError is not "")
            {
                logger.LogCritical("Python virtualenv 环境检查失败，错误信息：{Err}", standardError);
                return false;
            }

            logger.LogInformation("Python virtualenv 环境检查成功，virtualenv 版本：{Out}", standardOutput);
        }
        catch (Exception e)
        {
            logger.LogCritical(e, "检查 Python virtualenv 环境时出现异常");
            return false;
        }

        return true;
    }

    public static bool CreateVirtualEnvironment(ILogger logger, string pythonExecutable, string virtualenvPath, string requirements)
    {
        if (Directory.Exists(virtualenvPath))
        {
            var bin = Path.Combine(virtualenvPath, "bin");
            if (Directory.Exists(bin))
            {
                var pythonExist = Directory.GetFiles(bin, "python*").Any();
                if (pythonExist)
                {
                    logger.LogWarning("已存在的 Python 虚拟环境：{Path}", virtualenvPath);
                    return true;
                }
            }
        }

        try
        {
            var virtualenvProcessStartInfo = new ProcessStartInfo(pythonExecutable, $"-m venv {virtualenvPath}")
            {
                RedirectStandardError = true
            };
            logger.LogInformation("运行 {cmd}", virtualenvProcessStartInfo.FileName + " " + virtualenvProcessStartInfo.Arguments);
            var virtualenvProcess = Process.Start(virtualenvProcessStartInfo);
            if (virtualenvProcess is null)
            {
                logger.LogCritical("Python virtualenv 创建失败，无法启动 virtualenv 进程");
                if (Directory.Exists(virtualenvPath))
                {
                    Directory.Delete(virtualenvPath, true);
                }
                return false;
            }

            var standardError = virtualenvProcess.StandardError.ReadToEnd();
            if (standardError is not "")
            {
                logger.LogCritical("Python virtualenv 创建失败，错误信息：{Err}", standardError);
                if (Directory.Exists(virtualenvPath))
                {
                    Directory.Delete(virtualenvPath, true);
                }
                return false;
            }

            if (requirements is null)
            {
                logger.LogInformation("Python virtualenv 在 {Path} 创建成功，无依赖项", virtualenvPath);
                return true;
            }

            Debug.Assert(virtualenvPath != null, nameof(virtualenvPath) + " != null");
            var pipProcessStartInfo = new ProcessStartInfo(Path.Combine(virtualenvPath, "bin", "pip"), $"install -r {requirements}")
            {
                RedirectStandardError = true
            };
            logger.LogInformation("运行 {cmd}", pipProcessStartInfo.FileName + " " + pipProcessStartInfo.Arguments);
            var pipProcess = Process.Start(pipProcessStartInfo);
            if (pipProcess is null)
            {
                logger.LogCritical("Python 依赖项安装失败，无法启动 pip 进程");
                if (Directory.Exists(virtualenvPath))
                {
                    Directory.Delete(virtualenvPath, true);
                }
                return false;
            }

            standardError = pipProcess.StandardError.ReadToEnd();
            if (standardError is not "")
            {
                logger.LogCritical("Python 依赖项安装失败，错误信息：{Err}", standardError);
                if (Directory.Exists(virtualenvPath))
                {
                    Directory.Delete(virtualenvPath, true);
                }
                return false;
            }

            logger.LogInformation("Python 依赖项安装成功");
        }
        catch (Exception e)
        {
            logger.LogCritical(e, "创建 Python virtualenv 环境时出现异常");
            return false;
        }

        return true;
    }

    public static string Run(ILogger logger, string pythonExecutable, string scriptFile, IEnumerable<string> args)
    {
        try
        {
            var pythonStartInfo = new ProcessStartInfo(pythonExecutable, $"{scriptFile} {string.Join(" ", args)}")
            {
                RedirectStandardError = true, RedirectStandardOutput = true, UseShellExecute = false
            };
            logger.LogDebug("运行 {cmd}", pythonStartInfo.FileName + " " + pythonStartInfo.Arguments);
            var pythonProcess = Process.Start(pythonStartInfo);
            if (pythonProcess is null)
            {
                logger.LogCritical("Python 脚本运行失败，无法启动 Python 进程");
                return null;
            }

            var standardOutput = pythonProcess.StandardOutput.ReadToEnd();
            var standardError = pythonProcess.StandardError.ReadToEnd();

            if (standardError is not "")
            {
                logger.LogError("Python 脚本运行失败，错误信息：{Err}", standardError);
            }

            return standardOutput;
        }
        catch (Exception e)
        {
            logger.LogCritical(e, "运行 Python 脚本时出现异常");
            return null;
        }
    }
}
