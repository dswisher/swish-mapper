
using System.Collections.Generic;
using System.Linq;

using SwishMapper.Models.Data;

namespace SwishMapper.Parsing.Map
{
    public class MapParserContext
    {
        private readonly Stack<Scope> scopes = new Stack<Scope>();

        public MapParserContext(IEnumerable<DataModel> models)
        {
            Models = models;
            MapList = new ExpressiveMapList();

            // Should always have at least one scope
            scopes.Push(new Scope());
        }

        public IEnumerable<DataModel> Models { get; private set; }
        public ExpressiveMapList MapList { get; private set; }

        private Scope CurrentScope { get { return scopes.Peek(); } }


        // Adding a model alias to the current scope
        public void AddModelAlias(string alias, DataModel model)
        {
            CurrentScope.AddModelAlias(alias, model);
        }


        // Setting up a "with" statement - creating a new scope
        public void Push(string alias, CompoundIdentifier expansion)
        {
            // Create a new scope and push it on the stack
            var scope = new Scope();

            scopes.Push(scope);

            // Add the alias
            scope.AddAlias(alias, expansion);
        }


        // Leaving a "with" statement
        public void Pop()
        {
            // Pop a scope off the stack
            scopes.Pop();
        }


        public MappedDataAttribute Resolve(CompoundIdentifier identifier)
        {
            // We must have a prefix, and that should (eventually) resolve to a model.
            if (string.IsNullOrEmpty(identifier.Prefix))
            {
                throw new ParserException(identifier, "Identifier must have a prefix.");
            }

            // Expand the identifier by translating all its prefixes up the scope stack
            var expanded = Expand(identifier);

            // Use the expanded identifier to find the model
            var model = FindModel(expanded.Prefix);

            if (model == null)
            {
                throw new ParserException(expanded, $"Could not find a model for alias '{expanded.Prefix}'.");
            }

            // Use the first part to find the starting entity
            var entity = model.FindEntity(expanded.Parts.First());
            if (entity == null)
            {
                throw new ParserException(expanded, $"Could not find root entity '{expanded.Parts.First()}'.");
            }

            // Go through the path, using reference attributes to find the next entity
            foreach (var attributeName in expanded.Parts.Skip(1).Take(expanded.Parts.Count() - 2))
            {
                var att = entity.FindAttribute(attributeName);
                if (att == null)
                {
                    throw new ParserException(expanded, $"Could not find attribute '{attributeName}' in entity '{entity.Name}'.");
                }

                if (att.DataType.Type != PrimitiveType.Ref)
                {
                    throw new ParserException(expanded, $"Attribute '{entity.Name}.{attributeName}' is not a reference type.");
                }

                entity = model.FindEntity(att.DataType.RefName);
            }

            // Find the attribute in the last entity
            var attribute = entity.FindAttribute(expanded.Parts.Last());
            if (attribute == null)
            {
                throw new ParserException(expanded, $"Could not find attribute '{expanded.Parts.Last()}' in entity '{entity.Name}'.");
            }

            // Return the result
            return new MappedDataAttribute
            {
                Attribute = attribute,
                XPath = expanded.XPath
            };
        }


        private DataModel FindModel(string alias)
        {
            foreach (var scope in scopes)
            {
                var model = scope.FindModel(alias);

                if (model != null)
                {
                    return model;
                }
            }

            return null;
        }


        private CompoundIdentifier Expand(CompoundIdentifier ident)
        {
            // Keep expanding...
            List<CompoundIdentifier> expansions = new List<CompoundIdentifier>();

            expansions.Add(ident);

            foreach (var scope in scopes)
            {
                if (scope.HasAlias(ident.Prefix))
                {
                    ident = scope.GetAlias(ident.Prefix);

                    expansions.Add(ident);
                }
            }

            // Put everything back together into one...
            var result = new CompoundIdentifier
            {
                Prefix = expansions.Last().Prefix
            };

            for (var i = expansions.Count - 1; i >= 0; i--)
            {
                foreach (var part in expansions[i].Parts)
                {
                    result.Parts.Add(part);
                }
            }

            return result;
        }


        private class Scope
        {
            private readonly Dictionary<string, DataModel> modelAliases = new Dictionary<string, DataModel>();
            private readonly Dictionary<string, CompoundIdentifier> aliases = new Dictionary<string, CompoundIdentifier>();

            public DataModel FindModel(string alias)
            {
                if (modelAliases.ContainsKey(alias))
                {
                    return modelAliases[alias];
                }
                else
                {
                    return null;
                }
            }


            public void AddModelAlias(string alias, DataModel model)
            {
                modelAliases.Add(alias, model);
            }


            public void AddAlias(string alias, CompoundIdentifier expansion)
            {
                aliases.Add(alias, expansion);
            }


            public bool HasAlias(string alias)
            {
                return aliases.ContainsKey(alias);
            }


            public CompoundIdentifier GetAlias(string alias)
            {
                return aliases[alias];
            }
        }
    }
}
