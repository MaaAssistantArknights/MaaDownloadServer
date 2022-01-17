﻿using Semver;

namespace MaaDownloadServer.Services.IServices;

public interface IVersionService
{
    Task<(string, DateTime)> GetLatestVersion(Platform platform, Architecture architecture);
    Task<(string, DateTime)> GetVersion(Platform platform, Architecture architecture, SemVersion semVersion);
    Task<List<Platform>> GetSupportedPlatforms();
    Task<List<Architecture>> GetSupportedArchitectures(Platform platform);
    Task<Dictionary<string, DateTime>> GetVersions(Platform platform, Architecture architecture, int page);
}
