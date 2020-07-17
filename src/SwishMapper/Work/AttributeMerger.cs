
using System;
using System.Linq;

using Microsoft.Extensions.Logging;
using SwishMapper.Models.Data;

namespace SwishMapper.Work
{
    public class AttributeMerger : IAttributeMerger
    {
        private readonly ILogger logger;

        public AttributeMerger(ILogger<AttributeMerger> logger)
        {
            this.logger = logger;
        }


        public int Merge(DataAttribute targetAttribute, DataAttribute sourceAttribute)
        {
            var mismatchCount = 0;

            mismatchCount += MergeProperty(targetAttribute, sourceAttribute, "DataType", x => x.DataType, (x, y) => x.DataType = y);
            mismatchCount += MergeProperty(targetAttribute, sourceAttribute, "MinOccurs", x => x.MinOccurs, (x, y) => x.MinOccurs = y);
            mismatchCount += MergeProperty(targetAttribute, sourceAttribute, "MaxOccurs", x => x.MaxOccurs, (x, y) => x.MaxOccurs = y);
            mismatchCount += MergeProperty(targetAttribute, sourceAttribute, "Comment", x => x.Comment, (x, y) => x.Comment = y);

            if (sourceAttribute.IsXmlAttribute)
            {
                targetAttribute.IsXmlAttribute = true;
            }

            targetAttribute.Samples.AddRange(sourceAttribute.Samples);

            return mismatchCount;
        }


        private int MergeProperty(DataAttribute targetAttribute, DataAttribute sourceAttribute, string propertyName,
                Func<DataAttribute, string> getter, Action<DataAttribute, string> setter)
        {
            var source = getter(sourceAttribute);
            var target = getter(targetAttribute);

            // If we don't have data, we're done.
            if (string.IsNullOrEmpty(source))
            {
                return 0;
            }

            // If the source or target is empty, we can't have a conflict
            if (string.IsNullOrEmpty(target))
            {
                setter(targetAttribute, source);
            }
            else if (source != target)
            {
                var modelId = sourceAttribute.Parent.Parent.Id;
                var entityName = sourceAttribute.Parent.Name;

                var sourceSource = sourceAttribute.Sources.First()?.ShortName;

                logger.LogWarning("Merge conflict from {Source} on {Model}, {Entity}.{Attribute}, {PropName} has current value '{Old}', proposed value is '{New}'.",
                        sourceSource, modelId, entityName, sourceAttribute.Name, propertyName, target, source);

                return 1;
            }

            return 0;
        }


        private int MergeProperty(DataAttribute targetAttribute, DataAttribute sourceAttribute, string propertyName,
                Func<DataAttribute, DataType> getter, Action<DataAttribute, DataType> setter)
        {
            var source = getter(sourceAttribute);
            var target = getter(targetAttribute);

            // If we don't have data, we're done.
            if (source == null)
            {
                return 0;
            }

            // If the target is empty, we can't have a conflict
            if (target == null)
            {
                setter(targetAttribute, source);
            }
            else if (source != target)
            {
                // If the types are strings and the target lacks maxLength, accept the length.
                if ((source.Type == PrimitiveType.String) && (target.Type == PrimitiveType.String) && !target.MaxLength.HasValue)
                {
                    setter(targetAttribute, source);
                    return 0;
                }

                // Looks like a conflict...report it.
                var modelId = sourceAttribute.Parent.Parent.Id;
                var entityName = sourceAttribute.Parent.Name;

                var sourceSource = sourceAttribute.Sources.First()?.ShortName;

                logger.LogWarning("Merge conflict from {Source} on {Model}, {Entity}.{Attribute}, {PropName} has current value '{Old}', proposed value is '{New}'.",
                        sourceSource, modelId, entityName, sourceAttribute.Name, propertyName, target, source);

                return 1;
            }

            return 0;
        }
    }
}
