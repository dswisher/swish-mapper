
using System;
using System.Linq;

using Microsoft.Extensions.Logging;
using SwishMapper.Models.Data;

namespace SwishMapper.Work
{
    public class EntityMerger : IEntityMerger
    {
        private readonly ILogger logger;

        public EntityMerger(ILogger<EntityMerger> logger)
        {
            this.logger = logger;
        }


        public int Merge(DataEntity targetEntity, DataEntity sourceEntity)
        {
            var mismatchCount = 0;

            mismatchCount += MergeProperty(targetEntity, sourceEntity, "DataType", x => x.DataType, (x, y) => x.DataType = y);
            mismatchCount += MergeProperty(targetEntity, sourceEntity, "Comment", x => x.Comment, (x, y) => x.Comment = y);

            return mismatchCount;
        }


        private int MergeProperty(DataEntity targetEntity, DataEntity sourceEntity, string propertyName,
                Func<DataEntity, string> getter, Action<DataEntity, string> setter)
        {
            var source = getter(sourceEntity);
            var target = getter(targetEntity);

            // If we don't have data, we're done.
            if (string.IsNullOrEmpty(source))
            {
                return 0;
            }

            // If the source or target is empty, we can't have a conflict
            if (string.IsNullOrEmpty(target))
            {
                setter(targetEntity, source);
            }
            else if (source != target)
            {
                var modelId = sourceEntity.Parent.Id;

                var sourceSource = sourceEntity.Sources.First()?.ShortName;

                logger.LogWarning("Merge conflict from {Source} on {Model}, {Entity}, {PropName} has current value '{Old}', proposed value is '{New}'.",
                        sourceSource, modelId, sourceEntity.Name, propertyName, target, source);

                return 1;
            }

            return 0;
        }


        private int MergeProperty(DataEntity targetEntity, DataEntity sourceEntity, string propertyName,
                Func<DataEntity, DataType> getter, Action<DataEntity, DataType> setter)
        {
            var source = getter(sourceEntity);
            var target = getter(targetEntity);

            // If we don't have data, we're done.
            if (source == null)
            {
                return 0;
            }

            // If the target is empty, we can't have a conflict
            if (target == null)
            {
                setter(targetEntity, source);
            }
            else if (source != target)
            {
                // If the types are strings and the target lacks maxLength, accept the length.
                if ((source.Type == PrimitiveType.String) && (target.Type == PrimitiveType.String) && !target.MaxLength.HasValue)
                {
                    setter(targetEntity, source);
                    return 0;
                }

                // Looks like a conflict...report it.
                var modelId = sourceEntity.Parent.Id;

                var sourceSource = sourceEntity.Sources.First()?.ShortName;

                logger.LogWarning("Merge conflict from {Source} on {Model}, {Entity}, {PropName} has current value '{Old}', proposed value is '{New}'.",
                        sourceSource, modelId, sourceEntity.Name, propertyName, target, source);

                return 1;
            }

            return 0;
        }
    }
}
