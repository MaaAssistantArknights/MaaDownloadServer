// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

namespace MaaDownloadServer.Shared.Utils.Exceptions;

public class FileFoundException : IOException
{
    private readonly string _message;
    private readonly string _fileName;

    public FileFoundException(string message, string fileName)
    {
        _message = message;
        _fileName = fileName;
    }

    public override string Message
    {
        get
        {
            var message = $"{_message}\nFileExist {_fileName}";
            return message;
        }
    }
}
