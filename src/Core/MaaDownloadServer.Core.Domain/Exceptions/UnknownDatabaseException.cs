// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

namespace MaaDownloadServer.Core.Domain.Exceptions;

public class UnknownDatabaseException : Exception
{
    public UnknownDatabaseException(string dbType) : base($"未知的数据库类型: {dbType}") { }
}
