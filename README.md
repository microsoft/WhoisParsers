# WhoisParsers: Whois and RWhois Parsers and Crawlers C# Library

[![license:isc](https://img.shields.io/badge/license-mit-brightgreen.svg?style=flat-square#123)](https://github.com/Microsoft/WhoisParsers/blob/master/LICENSE) [![NuGet Package version](https://img.shields.io/nuget/v/WhoisParsers.svg?style=flat-square)](https://www.nuget.org/packages/WhoisParsers/) [![Build Status](https://img.shields.io/travis/Microsoft/WhoisParsers.svg?style=flat-square)](https://travis-ci.org/Microsoft/WhoisParsers) 

This library provides two main features:

1. Parsers to read Whois records from **offline bulk whois database dumps** of [IANA](https://www.iana.org/) organizations ([ARIN](https://www.arin.net/), [AFRINIC](https://www.afrinic.net/), [APNIC](https://www.apnic.net/), [LACNIC](http://www.lacnic.net), and [RIPE](https://www.ripe.net/))
2. Crawlers to retrieve **online RWhois** data from ARIN Referral Whois servers. This is a partial implementation of [RFC 2167](https://tools.ietf.org/html/rfc2167) that supports both bulk crawls using the *-xfer* command and incremental crawls.

This library does **NOT** provide features to contact the REST APIs such as [ARIN's Whois-RWS](https://www.arin.net/resources/whoisrws/).

## Nuget Package Installation

Create a new console C# project, then install the [WhoisParsers NuGet package](https://www.nuget.org/packages/WhoisParsers/) by using the Visual Studio GUI or by using this command in the [Package Manager Console](http://docs.nuget.org/consume/package-manager-console):

```PowerShell
Install-Package WhoisParsers
```

## Parsing offline bulk Whois databases

#### Creating a parser instance for reading ARIN, APNIC, LACNIC, or RIPE databases
```C#
var parser = new WhoisParser(new SectionTokenizer(), new SectionParser());
```

#### Creating a parser instance for reading AFRINIC databases
```C#
var parser = new WhoisParser(new AfrinicSectionTokenizer(), new SectionParser());
```

#### Parsing Whois sections

You can get the sample *arin.sample.txt* file from [here](WhoisDatabaseParsers.Tests/afrinic.sample.txt).

```C#
var parser = new WhoisParser(new SectionTokenizer(), new SectionParser());
var sections = parser.RetrieveSections(@"arin.sample.txt");

foreach (var section in sections)
{
    Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "Section ID: {0}", section.Id));
    Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "Number of records: {0}", section.Records.Count));
    Console.WriteLine("---- Section Records:");
    Console.WriteLine(section);
    Console.WriteLine();
}
```

Public functions provided by WhoisParser include:

| Function | Description |
| -------- | ----------- |
| **ColumnsPerType** | Retrieve a list of unique record names for each type of records in a database dump. Signature variations: <ul><li>*(string filePath)* - read from text file path</li><li>*(StreamReader reader)* - read from an existing reader</li></ul> |
| **RetrieveSections** | Retrieve parsed sections from the bulk database. Signature variations: <ul><li>*(string filePath)* - read from text file path</li><li>*(string filePath, string desiredType)* - read sections with only a certain type from text file path</li><li>*(string filePath, HashSet<string> desiredTypes)* - read sections with only certain *types* from text file path</li><li>*(StreamReader reader, string desiredType)* - read sections with only a certain type from an existing reader</li><li>*(StreamReader reader, HashSet<string> desiredTypes)* - read sections with only certain *types* from an existing reader</li></ul>  |
| **RetrieveSectionsFromString** | Retrieve parsed sections from the bulk database where the database is passed in as a string. Signature variations: <ul><li>*(string text)* - read all sections from the string</li><li>*(string text, string desiredType)* - read sections with only a certain type from the string</li><li>*(string text, HashSet<string> desiredTypes)* - read sections with only certain *types* from the string</li></ul> |

## Incrementing IPv4 and IPv6 IP addresses

The library contains functions to increment IPV4 and (more importantly) IPv6 IP addresses.

```C#
using Microsoft.Geolocation.Whois.Utils;
...
var ipv4Address = IPAddress.Parse("192.168.0.1");
Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "Before: {0}, After: {1}", ipv4Address, ipv4Address.Increment()));

var ipv6Address = IPAddress.Parse("2001:db8:a0b:12f0::1");
Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "Before: {0}, After: {1}", ipv6Address, ipv6Address.Increment()));
```

The output looks like this:

```
Before: 192.168.0.1, After: 192.168.0.2
Before: 2001:db8:a0b:12f0::1, After: 2001:db8:a0b:12f0::2
```

## Converting parsed Whois database sections to structured C# objects

**Documentation TODO**

## Crawling an RWhois Referral server

**Documentation TODO**

## Crawling multiple RWhois Referral servers at the same time

**Documentation TODO**
