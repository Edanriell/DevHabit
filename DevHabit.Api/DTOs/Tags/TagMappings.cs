namespace DevHabit.Api.DTOs.Tags;

internal static class TagMappings
{
    public static TagDto ToDto(this Tag tag)
    {
        return new TagDto
        {
            Id = tag.Id,
            Name = tag.Name,
            Description = tag.Description,
            CreatedAtUtc = tag.CreatedAtUtc,
            UpdatedAtUtc = tag.UpdatedAtUtc
        };
    }

    public static Tag ToEntity(this CreateTagDto dto)
    {
        Tag habit = new()
        {
            Id = $"t_{Guid.CreateVersion7()}",
            Name = dto.Name,
            Description = dto.Description,
            CreatedAtUtc = DateTime.UtcNow
        };

        return habit;
    }
}
