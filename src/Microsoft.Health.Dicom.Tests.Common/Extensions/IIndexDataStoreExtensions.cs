﻿// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using Dicom;
using Microsoft.Health.Dicom.Core.Features.ExtendedQueryTag;
using Microsoft.Health.Dicom.Core.Features.Store;

namespace Microsoft.Health.Dicom.Tests.Common.Extensions
{
    public static class IIndexDataStoreExtensions
    {
        public static Task<long> BeginCreateInstanceIndexAsync(this IIndexDataStore indexDataStore, string partitionId, DicomDataset dicomDataset, CancellationToken cancellationToken = default)
            => indexDataStore.BeginCreateInstanceIndexAsync(partitionId, dicomDataset, Array.Empty<QueryTag>(), cancellationToken);

        public static Task EndCreateInstanceIndexAsync(this IIndexDataStore indexDataStore, string partitionId, DicomDataset dicomDataset, long watermark, CancellationToken cancellationToken = default)
            => indexDataStore.EndCreateInstanceIndexAsync(partitionId, dicomDataset, watermark, Array.Empty<QueryTag>(), true, cancellationToken);
    }
}
