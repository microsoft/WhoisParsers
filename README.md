# WhoisParsers: Whois and RWhois Parsers and Crawlers C# Library

[![license:isc](https://img.shields.io/badge/license-mit-brightgreen.svg?style=flat-square)](https://github.com/Microsoft/WhoisParsers/blob/master/LICENSE) [![NuGet Package version](https://img.shields.io/nuget/v/WhoisParsers.svg?style=flat-square)](https://www.nuget.org/packages/WhoisParsers/) [![Build Status](https://img.shields.io/travis/Microsoft/WhoisParsers.svg?style=flat-square)](https://travis-ci.org/Microsoft/WhoisParsers)

**Repo Under Construction - Please check back later**

This library provides two main features:

1. Parsers to read Whois records from **offline** whois database dumps of IANA organizations (ARIN, AFRINIC, APNIC, LACNIC, RIPE )
2. Crawlers to retrieve **online** RWhois data from ARIN Referral Whois servers. This is a partial implementation of RFC 2167 that supports both bulk crawls using the -xfer command and incremental crawls.

This library does **NOT** provide features to contact the REST APIs such as [ARIN's Whois-RWS](https://www.arin.net/resources/whoisrws/).

## Nuget Package Installation

Create a new console C# project, then install the [WhoisParsers NuGet package](https://www.nuget.org/packages/WhoisParsers/) by using the Visual Studio GUI or by using this command in the [Package Manager Console](http://docs.nuget.org/consume/package-manager-console):

```PowerShell
Install-Package WhoisParsers
```
