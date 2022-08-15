# AireLyrics
[![.NET](https://github.com/mark5500/AireLyrics/actions/workflows/dotnet.yml/badge.svg)](https://github.com/mark5500/AireLyrics/actions/workflows/dotnet.yml)

This is a technical excercise to produce a CLI application which, when given the name of an artist will produce the average (mean) number of words in their songs.

## Getting Started
The commands below can be run in the folder of the AireLyrics executable. The application can also be run without arguments to provide an interactive experience.
```
USAGE:
    AireLyrics [OPTIONS]

EXAMPLES:
    AireLyrics --artist Ed Sheeran
    AireLyrics --artist Ed Sheeran --sampleSize 10
    AireLyrics --artist Ed Sheeran --id 1
    AireLyrics --artist Ed Sheeran --sampleSize 10 --id 1

OPTIONS:
    -h, --help                       Prints help information
    -a, --artist <NAME>              Name of the artist to search lyrics for
    -s, --sampleSize <SAMPLESIZE>    The amount of works that should be sampled for calculating average word count. (Default: 10)
    -i, --id <ID>                    The selected search result position of artist, if known.
```
## Dependencies
```
Dependencies defined in AireLyrics/AireLyrics.csproj

@ardalis            ardalis / GuardClauses               Ardalis.GuardClauses        4.0.1
@dotnet             dotnet / runtime                     Microsoft.Extensions.Http   6.0.0
@spectreconsole     spectreconsole / spectre.console     Spectre.Console             0.44.0
```

```
Dependencies defined in AireLyrics.Tests/AireLyrics.Tests.csproj

@coverlet-coverage  coverlet-coverage / coverlet         coverlet.collector          3.1.2
@microsoft          microsoft / vstest                   Microsoft.NET.Test.Sdk      17.1.0
@moq                moq / moq4                           Moq                         4.18.2
@xunit              xunit / xunit                                                    2.4.1
@xunit              xunit / visualstudio.xunit           xunit.runner.visualstudio   2.4.3
```
