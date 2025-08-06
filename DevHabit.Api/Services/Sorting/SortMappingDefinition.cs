namespace DevHabit.Api.Services.Sorting;

#pragma warning disable S2326
public sealed class SortMappingDefinition<TSource, TDestination> : ISortMappingDefinition
#pragma warning restore S2326
{
    public required SortMapping[] Mappings { get; set; }
}
