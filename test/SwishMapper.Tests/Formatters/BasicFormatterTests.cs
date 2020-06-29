
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

using FluentAssertions;
using SwishMapper.Formatters;
using SwishMapper.Tests.TestHelpers;
using Xunit;

namespace SwishMapper.Tests.Formatters
{
    public class BasicFormatterTests
    {
        [Theory]
        [MemberData(nameof(TestData))]
        public void ConsoleFormatterDoesTheBasics(DocRoot root, string requiredText)
        {
            using (var memory = new MemoryStream())
            {
                // Verify the output contains the required text content
                RunTest(memory, w => new ConsoleFormatter(w), root, requiredText);
            }
        }


        [Theory]
        [MemberData(nameof(TestData))]
        public void HtmlFormatterDoesTheBasics(DocRoot root, string requiredText)
        {
            using (var memory = new MemoryStream())
            {
                // Verify the output contains the required text content
                var content = RunTest(memory, w => new HtmlFormatter(w), root, requiredText);

                // Validate the XHTML is well formed by loading it using a DTD-validating XmlReader
                var settings = new XmlReaderSettings
                {
                    DtdProcessing = DtdProcessing.Parse,
                    ValidationType = ValidationType.DTD,
                    XmlResolver = new CustomXmlResolver()
                };

                memory.Position = 0;

                using (var textReader = new StreamReader(memory, leaveOpen: true))
                using (var xmlReader = XmlReader.Create(textReader, settings))
                {
                    var xmlDoc = new XmlDocument();

                    xmlDoc.Load(xmlReader);
                }
            }
        }


        public static IEnumerable<object[]> TestData()
        {
            const string myData = "My Fun Bit of Data";
            DocRoot root;

            // Simple one-cell table
            root = new DocRoot();
            root.AddTable().Row().Cell(myData);

            yield return new object[] { root, myData };
        }


        private static string RunTest(MemoryStream memory, Func<TextWriter, IDocFormatter> factory, DocRoot root, string requiredText)
        {
            using (var writer = new StreamWriter(memory, leaveOpen: true))
            {
                // Arrange
                var formatter = factory(writer);

                // Act
                formatter.Write(root);

                writer.Flush();
            }

            // Assert
            memory.Position = 0;

            using (var reader = new StreamReader(memory, leaveOpen: true))
            {
                var content = reader.ReadToEnd();

                content.Should().Contain(requiredText);

                return content;
            }
        }
    }
}
