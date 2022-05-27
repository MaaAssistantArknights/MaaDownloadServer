// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using System.Collections.ObjectModel;
using MaaDownloadServer.Core.Domain.Models.Module;

namespace MaaDownloadServer.Core.Domain.Dto.Module;

/// <summary>
/// 获取组件列表
/// </summary>
public class GetModuleListDto : Collection<MaaModuleInfo> { }
