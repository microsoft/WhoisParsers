// -----------------------------------------------------------------------
// <copyright file="ICommonRecordMetadata.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.Whois.Normalization
{
    public interface ICommonRecordMetadata
    {
        string Id { get; set; }

        string Name { get; set; }

        string Created { get; set; }

        string Updated { get; set; }

        string UpdatedBy { get; set; }

        string Description { get; set; }

        string Comment { get; set; }
    }
}
