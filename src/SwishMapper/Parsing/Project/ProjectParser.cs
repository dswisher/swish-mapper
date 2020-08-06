
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using SwishMapper.Models.Project;

namespace SwishMapper.Parsing.Project
{
    public class ProjectParser : AbstractParser, IProjectParser
    {
        private readonly ILexerFactory lexerFactory;
        private readonly ILogger logger;

        public ProjectParser(ILexerFactory lexerFactory,
                             ILogger<ProjectParser> logger)
        {
            this.lexerFactory = lexerFactory;
            this.logger = logger;
        }


        public Task<ProjectDefinition> ParseAsync(string path)
        {
            var definition = new ProjectDefinition();

            using (var lexer = lexerFactory.CreateProjectLexer(path))
            {
                // Move to the first token
                Advance(lexer);

                // Keep parsing until nothing is left.
                while (lexer.Token.Kind != TokenKind.EOF)
                {
                    ParseStatement(definition, lexer);
                }
            }

            return Task.FromResult(definition);
        }


        private void ParseStatement(ProjectDefinition definition, ProjectLexer lexer)
        {
            VerifyToken(lexer, TokenKind.Keyword);

            var text = lexer.Token.Text;

            Advance(lexer);

            switch (text)
            {
                case "map":
                    ParseMap(definition, lexer);
                    break;

                case "model":
                    ParseModel(definition, lexer);
                    break;

                default:
                    throw new ParserException($"Unexpected statement keyword: {lexer.Token.Text}.", lexer.Token);
            }
        }


        private void ParseMap(ProjectDefinition definition, ProjectLexer lexer)
        {
            var map = new ProjectMap();
            definition.Maps.Add(map);

            Consume(lexer, TokenKind.LeftCurly);

            while (lexer.Token.Kind == TokenKind.Keyword)
            {
                var keyword = Consume(lexer, TokenKind.Keyword);

                switch (keyword)
                {
                    case "from":
                        map.FromModelId = Consume(lexer, TokenKind.Identifier);
                        OptionallyConsume(lexer, TokenKind.Semicolon);
                        break;

                    case "path":
                        map.Path = ConsumeFile(lexer);
                        OptionallyConsume(lexer, TokenKind.Semicolon);
                        break;

                    case "to":
                        map.ToModelId = Consume(lexer, TokenKind.Identifier);
                        OptionallyConsume(lexer, TokenKind.Semicolon);
                        break;

                    default:
                        throw new ParserException($"Unexpected map keyword: {keyword}.", lexer.Token);
                }
            }

            Consume(lexer, TokenKind.RightCurly);
        }


        private void ParseModel(ProjectDefinition definition, ProjectLexer lexer)
        {
            var model = new ProjectModel();
            definition.Models.Add(model);

            model.Id = Consume(lexer, TokenKind.Identifier);

            Consume(lexer, TokenKind.LeftCurly);

            while (lexer.Token.Kind == TokenKind.Keyword)
            {
                var keyword = Consume(lexer, TokenKind.Keyword);

                switch (keyword)
                {
                    case "csv":
                        ParseCsvPopulator(model, lexer);
                        break;

                    case "name":
                        model.Name = Consume(lexer, TokenKind.String);
                        OptionallyConsume(lexer, TokenKind.Semicolon);
                        break;

                    case "samples":
                        ParseSamplePopulator(model, lexer);
                        break;

                    case "xsd":
                        ParseXsdPopulator(model, lexer);
                        break;

                    default:
                        throw new ParserException($"Unexpected model keyword: {keyword}.", lexer.Token);
                }
            }

            Consume(lexer, TokenKind.RightCurly);
        }


        private void ParseCsvPopulator(ProjectModel model, ProjectLexer lexer)
        {
            var populator = new CsvProjectModelPopulator
            {
                Type = ProjectModelPopulatorType.Csv
            };

            model.Populators.Add(populator);

            Consume(lexer, TokenKind.LeftCurly);

            while (lexer.Token.Kind == TokenKind.Keyword)
            {
                var keyword = Consume(lexer, TokenKind.Keyword);

                switch (keyword)
                {
                    case "path":
                        populator.Path = ConsumeFile(lexer);
                        OptionallyConsume(lexer, TokenKind.Semicolon);
                        break;

                    default:
                        throw new ParserException($"Unexpected populator keyword: {keyword}.", lexer.Token);
                }
            }

            Consume(lexer, TokenKind.RightCurly);
        }


        private void ParseXsdPopulator(ProjectModel model, ProjectLexer lexer)
        {
            var populator = new XsdProjectModelPopulator
            {
                Type = ProjectModelPopulatorType.Xsd
            };

            model.Populators.Add(populator);

            Consume(lexer, TokenKind.LeftCurly);

            while (lexer.Token.Kind == TokenKind.Keyword)
            {
                var keyword = Consume(lexer, TokenKind.Keyword);

                switch (keyword)
                {
                    case "path":
                        populator.Path = ConsumeFile(lexer);
                        OptionallyConsume(lexer, TokenKind.Semicolon);
                        break;

                    default:
                        throw new ParserException($"Unexpected populator keyword: {keyword}.", lexer.Token);
                }
            }

            Consume(lexer, TokenKind.RightCurly);
        }


        private void ParseSamplePopulator(ProjectModel model, ProjectLexer lexer)
        {
            var populator = new SampleProjectModelPopulator
            {
                Type = ProjectModelPopulatorType.Sample,
                Id = Consume(lexer, TokenKind.Identifier)
            };

            model.Populators.Add(populator);

            Consume(lexer, TokenKind.LeftCurly);

            while (lexer.Token.Kind == TokenKind.Keyword)
            {
                var keyword = Consume(lexer, TokenKind.Keyword);

                switch (keyword)
                {
                    case "files":
                        populator.Files.AddRange(ConsumeWildcardFiles(lexer));
                        OptionallyConsume(lexer, TokenKind.Semicolon);
                        break;

                    case "zip-mask":
                        populator.ZipMask = Consume(lexer, TokenKind.String);
                        OptionallyConsume(lexer, TokenKind.Semicolon);
                        break;
                }
            }

            Consume(lexer, TokenKind.RightCurly);
        }


        private string ConsumeFile(ProjectLexer lexer)
        {
            var fragment = Consume(lexer, TokenKind.String);

            return Path.Combine(Path.GetDirectoryName(lexer.FilePath), fragment);
        }


        private IEnumerable<SampleInputFile> ConsumeWildcardFiles(ProjectLexer lexer)
        {
            var mask = Consume(lexer, TokenKind.String);

            var dirPath = Path.Combine(Path.GetDirectoryName(lexer.FilePath), Path.GetDirectoryName(mask));
            var directory = new DirectoryInfo(dirPath);
            var filemask = Path.GetFileName(mask);

            foreach (var info in directory.GetFiles(filemask).OrderBy(x => x.Name))
            {
                yield return new SampleInputFile
                {
                    Path = info.FullName,
                    LastWriteUtc = info.LastWriteTimeUtc
                };
            }
        }


        private void OptionallyConsume(ProjectLexer lexer, TokenKind kind)
        {
            if (lexer.Token.Kind == kind)
            {
                Advance(lexer);
            }
        }
    }
}
