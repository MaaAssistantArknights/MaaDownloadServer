﻿// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using MaaDownloadServer.Data.Base.Context;
using MaaDownloadServer.Data.Db.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

var dbType = "";
var connectionString = "";

if (args.Length == 2)
{
    dbType = args[0];
    connectionString = args[1];
}

if (string.IsNullOrEmpty(dbType))
{
    Console.WriteLine("请输入数据库类型:");
    dbType = Console.ReadLine();
}

if (string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine("请输入连接字符串:");
    connectionString = Console.ReadLine();
}

if (string.IsNullOrEmpty(dbType) || string.IsNullOrEmpty(connectionString))
{
    Console.Error.WriteLine("数据库类型或连接字符串为空");
    return -1;
}

MaaDbContext dbContext;

switch (dbType!.ToLower())
{
    case "postgres":
        dbContext = new MaaPgSqlDbContext(connectionString);
        break;
    default:
        Console.Error.WriteLine($"未知的数据库类型: {dbType}");
        Console.WriteLine("尝试为可能的 Migration 创建操作构建 IHost");
        var host = Host.CreateDefaultBuilder();
        var app = host.Build();
        app.Run();
        return 0;
}

var migrations = dbContext.Database.GetPendingMigrations().ToList();
if (migrations.Count != 0)
{
    Console.WriteLine($"将会应用 {migrations.Count} 个迁移");
    dbContext.Database.Migrate();
}
else
{
    Console.WriteLine("没有需要应用的迁移");
}

dbContext.Dispose();

return 0;