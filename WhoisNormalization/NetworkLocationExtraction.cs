// -----------------------------------------------------------------------
// <copyright file="NetworkLocationExtraction.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.Whois.Normalization
{
    using System;
    using System.Collections.Generic;
    using Parsers;

    public class NetworkLocationExtraction
    {
        public NetworkLocationExtraction(IWhoisParser parser)
        {
            this.Parser = parser;
            this.OrganizationIdsToOrganizations = new Dictionary<string, List<NormalizedOrganization>>();
            this.OrganizationNamesToOrganizations = new Dictionary<string, List<NormalizedOrganization>>();
        }

        public IWhoisParser Parser { get; set; }

        private Dictionary<string, List<NormalizedOrganization>> OrganizationIdsToOrganizations { get; set; }

        private Dictionary<string, List<NormalizedOrganization>> OrganizationNamesToOrganizations { get; set; }

        public void LoadOrganizations(IEnumerable<string> filePaths)
        {
            if (filePaths == null)
            {
                throw new ArgumentNullException("filePaths");
            }

            foreach (var filePath in filePaths)
            {
                foreach (var section in this.Parser.RetrieveSections(filePath))
                {
                    var organization = NormalizedOrganization.TryParseFromSection(section);

                    if (organization != null && organization.Location.AddressSeemsValid())
                    {
                        this.CreateIdNameMappingForOrganization(organization);
                    }
                }
            }
        }

        public void LoadOrganizations(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            this.LoadOrganizations(new List<string>() { filePath });
        }

        public IEnumerable<NormalizedNetwork> ExtractNetworksWithLocations(IEnumerable<string> organizationFilePaths, IEnumerable<string> networkFilePaths)
        {
            this.LoadOrganizations(organizationFilePaths);
            return this.ExtractNetworksWithLocations(networkFilePaths);
        }

        public IEnumerable<NormalizedNetwork> ExtractNetworksWithLocations(string organizationFilePath, string networkFilePath)
        {
            this.LoadOrganizations(organizationFilePath);
            return this.ExtractNetworksWithLocations(new List<string>() { networkFilePath });
        }

        public IEnumerable<NormalizedNetwork> ExtractNetworks(IEnumerable<string> networkFilePaths)
        {
            foreach (var networkFilePath in networkFilePaths)
            {
                foreach (var section in this.Parser.RetrieveSections(networkFilePath))
                {
                    var network = NormalizedNetwork.TryParseFromSection(section);

                    if (network != null)
                    {
                        if (this.OrganizationIdsToOrganizations.Count > 0)
                        {
                            network.FindExternalOrganization(section, this.OrganizationIdsToOrganizations);
                        }
                        else if (network.ExternalOrganization == null && this.OrganizationNamesToOrganizations.Count > 0)
                        {
                            network.FindExternalOrganization(section, this.OrganizationNamesToOrganizations);
                        }

                        var networkLocation = network.Location;
                        var organizationLocation = network.ExternalOrganization?.Location;

                        if (networkLocation == null || !networkLocation.AddressSeemsValid())
                        {
                            if (organizationLocation != null && organizationLocation.AddressSeemsValid())
                            {
                                network.Location = organizationLocation;
                            }
                        }

                        yield return network;
                    }
                }
            }
        }

        public IEnumerable<NormalizedNetwork> ExtractNetworksWithLocations(IEnumerable<string> networkFilePaths)
        {
            foreach (var network in this.ExtractNetworks(networkFilePaths))
            {
                if (network.Location != null && network.Location.AddressSeemsValid())
                {
                    yield return network;
                }
            }
        }

        private void CreateIdNameMappingForOrganization(NormalizedOrganization organization)
        {
            if (organization != null)
            {
                if (organization.Id != null)
                {
                    List<NormalizedOrganization> organizationIdsOrganizations;

                    if (!this.OrganizationIdsToOrganizations.TryGetValue(organization.Id, out organizationIdsOrganizations))
                    {
                        organizationIdsOrganizations = new List<NormalizedOrganization>();
                    }

                    organizationIdsOrganizations.Add(organization);

                    this.OrganizationIdsToOrganizations[organization.Id] = organizationIdsOrganizations;
                }

                if (organization.Name != null)
                {
                    List<NormalizedOrganization> organizationNamesOrganizations;

                    if (!this.OrganizationIdsToOrganizations.TryGetValue(organization.Name, out organizationNamesOrganizations))
                    {
                        organizationNamesOrganizations = new List<NormalizedOrganization>();
                    }

                    organizationNamesOrganizations.Add(organization);

                    this.OrganizationIdsToOrganizations[organization.Name] = organizationNamesOrganizations;
                }
            }
        }
    }
}
