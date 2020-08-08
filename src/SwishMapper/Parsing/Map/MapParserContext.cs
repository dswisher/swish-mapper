
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
            // TODO - xyzzy - implement push
        }


        // Leaving a "with" statement
        public void Pop()
        {
            // TODO - xyzzy - implement pop
        }


        public MappedDataAttribute Resolve(CompoundIdentifier identifier)
        {
            // We must have a prefix, and that should (eventually) resolve to a model.
            if (string.IsNullOrEmpty(identifier.Prefix))
            {
                throw new ParserException(identifier, "Identifier must have a prefix.");
            }

            // TODO - xyzzy - this is overly simplistic...need to translate up through all aliases
            var model = FindModel(identifier.Prefix);

            if (model == null)
            {
                throw new ParserException(identifier, $"Could not find a model for alias '{identifier.Prefix}'.");
            }

            // Use the first part to find the starting entity
            var entity = model.FindEntity(identifier.Parts.First());
            if (entity == null)
            {
                throw new ParserException(identifier, $"Could not find root entity '{identifier.Parts.First()}'.");
            }

            // Go through the path, using reference attributes to find the next entity
            foreach (var attributeName in identifier.Parts.Skip(1).Take(identifier.Parts.Count() - 2))
            {
                var att = entity.FindAttribute(attributeName);
                if (att == null)
                {
                    throw new ParserException(identifier, $"Could not find attribute '{attributeName}' in entity '{entity.Name}'.");
                }

                if (att.DataType.Type != PrimitiveType.Ref)
                {
                    throw new ParserException(identifier, $"Attribute '{entity.Name}.{attributeName}' is not a reference type.");
                }

                entity = model.FindEntity(att.DataType.RefName);
            }

            // Find the attribute in the last entity
            var attribute = entity.FindAttribute(identifier.Parts.Last());
            if (attribute == null)
            {
                throw new ParserException(identifier, $"Could not find attribute '{identifier.Parts.Last()}' in entity '{entity.Name}'.");
            }

            // Return the result
            return new MappedDataAttribute
            {
                Attribute = attribute,
                XPath = identifier.XPath
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


        private class Scope
        {
            private readonly Dictionary<string, DataModel> modelAliases = new Dictionary<string, DataModel>();

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
        }
    }
}
