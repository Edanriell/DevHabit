using DevHabit.Api.DTOs.Entries;
using DevHabit.Api.DTOs.Habits;
using DevHabit.Api.DTOs.HabitTags;
using DevHabit.Api.DTOs.Tags;
using DevHabit.Api.Entities;

namespace DevHabit.IntegrationTests.Infrastructure;

public static class TestData
{
    public static class Habits
    {
        public static CreateHabitDto CreateReadingHabit()
        {
            return new CreateHabitDto
            {
                Name = "Read Books",
                Type = HabitType.Measurable,
                Frequency = new FrequencyDto
                {
                    Type = FrequencyType.Daily,
                    TimesPerPeriod = 1
                },
                Target = new TargetDto
                {
                    Value = 30,
                    Unit = "pages"
                }
            };
        }

        public static CreateHabitDto CreateExerciseHabit()
        {
            return new CreateHabitDto
            {
                Name = "Exercise",
                Type = HabitType.Measurable,
                Frequency = new FrequencyDto
                {
                    Type = FrequencyType.Daily,
                    TimesPerPeriod = 1
                },
                Target = new TargetDto
                {
                    Value = 30,
                    Unit = "minutes"
                }
            };
        }
    }

    public static class Tags
    {
        public static CreateTagDto CreateImportantTag()
        {
            return new CreateTagDto
            {
                Name = "Important"
            };
        }

        public static CreateTagDto CreateProductivityTag()
        {
            return new CreateTagDto
            {
                Name = "Productivity"
            };
        }
    }

    public static class HabitTags
    {
        public static UpsertHabitTagsDto CreateUpsertDto(params string[] tagIds)
        {
            return new UpsertHabitTagsDto
            {
                TagIds = tagIds.ToList()
            };
        }
    }

    public static class Entries
    {
        public static CreateEntryDto CreateEntry(string habitId, int value = 10, string? note = null)
        {
            return new CreateEntryDto
            {
                HabitId = habitId,
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
                Value = value,
                Notes = note
            };
        }

        public static CreateEntryDto CreateEntryForDate(
            string habitId, DateOnly date, int value = 10, string? note = null)
        {
            return new CreateEntryDto
            {
                HabitId = habitId,
                Date = date,
                Value = value,
                Notes = note
            };
        }

        public static UpdateEntryDto CreateUpdateEntry(int value = 20, string? note = "Updated entry")
        {
            return new UpdateEntryDto
            {
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
                Value = value,
                Notes = note
            };
        }

        public static CreateEntryBatchDto CreateBatch(string habitId, params (DateOnly Date, int Value)[] entries)
        {
            return new CreateEntryBatchDto
            {
                Entries = entries.Select(e => new CreateEntryDto
                {
                    HabitId = habitId,
                    Date = e.Date,
                    Value = e.Value
                }).ToList()
            };
        }
    }
}
