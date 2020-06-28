# swish-mapper

This code in this repo is intended to help with cases where there are multiple data sources
and sinks, and there is a desire to document the mappings between them. One important aspect
of this is the ability to identify gaps in a data source and a data sink, such as when an
existing data file/report needs to be generated from a new data source.

Currently, the data sources and sinks are largely specified via XML schema documents (XSDs).
The content of these can be amended by reading Excel files containing additional comments
on the properties (elements, attributes) in the XSDs.

The data sources, sinks, desired reports, etc., is specified via a "project" file, which
defaults to `project.sm`. This file is written using a simple domain specific language (DSL),
documented below.

Currently, everything is done via the command-line.
The bulk of the code lives in a shared library, so it should be possible to wrap it all up
in a GUI, if such a thing is desired.


# Running the Code

TODO - package as a dotnet command?

dotnet run -- [options]

TODO - define options!


# Writing a Project File

TODO.


# Reports

The set of reports to generate is specified in the project file (see above).
This section describes each report.

## Document Report

* Format: HTML
* Status: Not yet implemented

A list of all elements in one data source or sink.


## Dictionary Report

* Format: HTML
* Status: Not yet implemented

A list of every attribute and element, which sources/sinks it appears in, and
any mappings between sources and sinks.


## Mapping Report

* Format: HTML
* Status: Not yet implemented

The mapping between one sink and one or more sources, including gaps.
Gaps are defined as data items in the sink that are not present in any of the
specified sources.


## Source/Sink Diagram

* Format: SVG
* Status: Not yet implemented

A diagram illustrating the structure (hierarchy) of a single data source or sink.

