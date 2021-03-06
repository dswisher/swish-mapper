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

A project file defines the various data models and the mappings between them.

A data model can be composed from various sources.
For example, an XSD may define the structure and types of the model, a CSV file may provide
descriptions for the attributes, and a bunch of XML files may be read for sample values.

In a sense, a project file is also like a `make` file or pipeline, in that it defines the
processing steps to construct the data model. Intermediate outputs will only be rebuilt
if the inputs are newer.

Here is the overall structure of a project file, defining two data models, `article-metadata`
and `news-feed`:

    project "My web site project"
    {
        // TODO - settings, like the output and intermediate directories

        model article-metadata
        {
            ...
        }

        model news-feed
        {
            ...
        }

        // TODO - mappings
    }

The data model blocks define the details of the model, including the sources. For example,
if the `article-metadata` was XML and has an associated XSD, along with a bunch of sample
files, the definition might look like:

    model article-metadata
    {
        xsd "article.xsd";

        samples "/ArticleData/2018/06/*.xml";
        samples "/ArticleData/2018/07/*.xml";
    }

A parsed project is represented by the `Project` class.
The result of processing the file is represented by the `DataProject` class.


## Samples

Sample files are processed and aggregated into one file, containing a summary of the samples.
This is an intermediate file, and will only be rebuilt if newer samples match one or more of
the `samples` directives. All `samples` are lumped together, regardless of how many `samples`
directives are present.


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


# Links

## XSD

* [Hide vs Expose Namespaces](http://www.xfront.com/HideVersusExpose.html) - in-depth discussion of `elementFormDefault`

## Markup

* [mdast](https://github.com/syntax-tree/mdast) - Markdown Abstract Syntax Tree - one model of an AST for Markdown

