using System.Dynamic;
using Asp.Versioning;
using DevHabit.Api.Database;
using DevHabit.Api.DTOs.Common;
using DevHabit.Api.DTOs.Habits;
using DevHabit.Api.Entities;
using DevHabit.Api.Services;
using DevHabit.Api.Services.Sorting;
using FluentValidation;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.Api.Controllers;

[ApiController]
// [Route("v{version:apiVersion}/habits")]
[Route("habits")]
[ApiVersion(1.0)]
// [ApiVersion(2.0)]
public sealed class HabitsController(ApplicationDbContext dbContext, LinkService linkService) : ControllerBase
{
    // [HttpGet]
    // public async Task<ActionResult<HabitsCollectionDto>> GetHabits()
    // {
    //     List<HabitDto> habits = await dbContext
    //         .Habits
    //         .Select(HabitQueries.ProjectToDto())
    //         .ToListAsync();
    //
    //
    //     var habitsCollectionDto = new HabitsCollectionDto
    //     {
    //         Data = habits
    //     };
    //
    //     return Ok(habitsCollectionDto);
    // }

    [HttpGet]
    // [Produces(MediaTypeNames.Application.Json, CustomMediaTypeNames.Application.HateoasJson)]
    public async Task<IActionResult> GetHabits(
        // [FromQuery(Name = "q")] string? search,
        // HabitType? type,
        // HabitStatus? status
        [FromQuery] HabitsQueryParameters query,
        SortMappingProvider sortMappingProvider,
        DataShapingService dataShapingService)
    {
        if (!sortMappingProvider.ValidateMappings<HabitDto, Habit>(query.Sort))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: $"The provided sort parameter isn't valid: '{query.Sort}'");
        }

        if (!dataShapingService.Validate<HabitDto>(query.Fields))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: $"The provided data shaping fields aren't valid: '{query.Fields}'");
        }

        // search ??= search?.Trim().ToLower();
        query.Search ??= query.Search?.Trim().ToLower();

        // IQueryable<Habit> query = dbContext.Habits;
        //
        // if (!string.IsNullOrWhiteSpace(search))
        // {
        //     query = query.Where(h => h.Name.ToLower().Contains(search) ||
        //         h.Description != null && h.Description.ToLower().Contains(search));
        // }

        // List<HabitDto> habits = await dbContext
        //     .Habits
        //     .Where(h => search == null ||
        //         h.Name.ToLower().Contains(search) ||
        //         h.Description != null && h.Description.ToLower().Contains(search))
        //     .Where(h => type == null || h.Type == type)
        //     .Where(h => status == null || h.Status == status)
        //     .Select(HabitQueries.ProjectToDto())
        //     .ToListAsync();

        // List<HabitDto> habits = await dbContext
        //     .Habits
        //     .Where(h => query.Search == null ||
        //         h.Name.ToLower().Contains(query.Search) ||
        //         h.Description != null && h.Description.ToLower().Contains(query.Search))
        //     .Where(h => query.Type == null || h.Type == query.Type)
        //     .Where(h => query.Status == null || h.Status == query.Status)
        //     .Select(HabitQueries.ProjectToDto())
        //     .ToListAsync();

        // Better approach
        // List<HabitDto> habits = await dbContext
        //     .Habits
        //     .Where(h => query.Search == null ||
        //         EF.Functions.ILike(h.Name, $"%{query.Search}%") ||
        //         h.Description != null && EF.Functions.ILike(h.Description, $"%{query.Search}%"))
        //     .Where(h => query.Type == null || h.Type == query.Type)
        //     // .OrderByDescending(h => h.Name)
        //     // .ThenByDescending(h => h.Description)
        //     .Where(h => query.Status == null || h.Status == query.Status)
        //     .Select(HabitQueries.ProjectToDto())
        //     .ToListAsync();

        // Expression<Func<Habit, object>> orderBy = query.Sort switch
        // {
        //     "name" => h => h.Name,
        //     "description" => h => h.Description,
        //     "type" => h => h.Type,
        //     "status" => h => h.Status,
        //     _ => h => h.Name
        // };

        SortMapping[] sortMappings = sortMappingProvider.GetMappings<HabitDto, Habit>();

        // List<HabitDto> habits = await dbContext
        //     .Habits
        //     .Where(h => query.Search == null ||
        //         h.Name.ToLower().Contains(query.Search) ||
        //         h.Description != null && h.Description.ToLower().Contains(query.Search))
        //     .Where(h => query.Type == null || h.Type == query.Type)
        //     // .OrderBy(orderBy)
        //     .Where(h => query.Status == null || h.Status == query.Status)
        //     // .OrderBy("Name ASC, Description DESC, EndDate DESC")
        //     .ApplySort(query.Sort, sortMappings)
        //     .Select(HabitQueries.ProjectToDto())
        //     .ToListAsync();
        // List<HabitDto> habits = await dbContext
        //     .Habits
        //     .Where(h => query.Search == null ||
        //         EF.Functions.ILike(h.Name, $"%{query.Search}%") ||
        //         h.Description != null && EF.Functions.ILike(h.Description, $"%{query.Search}%"))
        //     .Where(h => query.Type == null || h.Type == query.Type)
        //     .Where(h => query.Status == null || h.Status == query.Status)
        //     .ApplySort(query.Sort, sortMappings)
        //     .Select(HabitQueries.ProjectToDto())
        //     .ToListAsync();

        // IQueryable<Habit> habitsQuery = dbContext
        //     .Habits
        //     .Where(h => query.Search == null ||
        //         h.Name.ToLower().Contains(query.Search) ||
        //         h.Description != null && h.Description.ToLower().Contains(query.Search))
        //     .Where(h => query.Type == null || h.Type == query.Type)
        //     .Where(h => query.Status == null || h.Status == query.Status);

        // int totalCount = await habitsQuery.CountAsync();

        // List<HabitDto> habits = await habitsQuery
        //     .ApplySort(query.Sort, sortMappings)
        //     .Skip((query.Page - 1) * query.PageSize)
        //     .Take(query.PageSize)
        //     .Select(HabitQueries.ProjectToDto())
        //     .ToListAsync();

        IQueryable<HabitDto> habitsQuery = dbContext
            .Habits
            .Where(h => query.Search == null ||
                EF.Functions.ILike(h.Name, $"%{query.Search}%") ||
                h.Description != null && EF.Functions.ILike(h.Description, $"%{query.Search}%"))
            .Where(h => query.Type == null || h.Type == query.Type)
            .Where(h => query.Status == null || h.Status == query.Status)
            .ApplySort(query.Sort, sortMappings)
            .Select(HabitQueries.ProjectToDto());

        // var paginationResult = new PaginationResult<HabitDto>
        // {
        //     Items = habits
        // };

        // var paginationResult = await PaginationResult<HabitDto>.CreateAsync(habitsQuery, query.Page, query.PageSize);

        int totalCount = await habitsQuery.CountAsync();

        List<HabitDto> habits = await habitsQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        bool includeLinks = query.Accept == CustomMediaTypeNames.Application.HateoasJson;
        var paginationResult = new PaginationResult<ExpandoObject>
        {
            // Items = habits,
            Items = dataShapingService.ShapeCollectionData(
                habits,
                query.Fields,
                includeLinks ? h => CreateLinksForHabit(h.Id, query.Fields) : null),
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount
        };
        if (includeLinks)
        {
            paginationResult.Links = CreateLinksForHabits(
                query,
                paginationResult.HasNextPage,
                paginationResult.HasPreviousPage);
        }

        return Ok(paginationResult);
    }

    [HttpGet("{id}")]
    [MapToApiVersion(1.0)]
    public async Task<IActionResult> GetHabit(
        string id,
        string? fields,
        [FromHeader(Name = "Accept")] string? accept,
        DataShapingService dataShapingService)
    {
        if (!dataShapingService.Validate<HabitWithTagsDto>(fields))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: $"The provided data shaping fields aren't valid: '{fields}'");
        }

        HabitWithTagsDto? habit = await dbContext
            .Habits
            .Where(h => h.Id == id)
            .Select(HabitQueries.ProjectToDtoWithTags())
            .FirstOrDefaultAsync();

        if (habit is null)
        {
            return NotFound();
        }

        ExpandoObject shapedHabitDto = dataShapingService.ShapeData(habit, fields);

        if (accept == CustomMediaTypeNames.Application.HateoasJson)
        {
            List<LinkDto> links = CreateLinksForHabit(id, fields);

            shapedHabitDto.TryAdd("links", links);
        }

        return Ok(shapedHabitDto);
    }

    [HttpGet("{id}")]
    [MapToApiVersion(2.0)]
    public async Task<IActionResult> GetHabitV2(
        string id,
        string? fields,
        [FromHeader(Name = "Accept")] string? accept,
        DataShapingService dataShapingService)
    {
        if (!dataShapingService.Validate<HabitWithTagsDtoV2>(fields))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: $"The provided data shaping fields aren't valid: '{fields}'");
        }

        HabitWithTagsDtoV2? habit = await dbContext
            .Habits
            .Where(h => h.Id == id)
            .Select(HabitQueries.ProjectToDtoWithTagsV2())
            .FirstOrDefaultAsync();

        if (habit is null)
        {
            return NotFound();
        }

        ExpandoObject shapedHabitDto = dataShapingService.ShapeData(habit, fields);

        if (accept == CustomMediaTypeNames.Application.HateoasJson)
        {
            List<LinkDto> links = CreateLinksForHabit(id, fields);

            shapedHabitDto.TryAdd("links", links);
        }

        return Ok(shapedHabitDto);
    }

    [HttpPost]
    public async Task<ActionResult<HabitDto>> CreateHabit(
        CreateHabitDto createHabitDto,
        IValidator<CreateHabitDto> validator)
    {
        // ValidationResult validationResult = await validator.ValidateAsync(createHabitDto);
        //
        // if (!validationResult.IsValid)
        // {
        //     return BadRequest(validationResult.ToDictionary());
        // }
        await validator.ValidateAndThrowAsync(createHabitDto);

        Habit habit = createHabitDto.ToEntity();

        dbContext.Habits.Add(habit);

        await dbContext.SaveChangesAsync();

        HabitDto habitDto = habit.ToDto();
        habitDto.Links = CreateLinksForHabit(habit.Id, null);

        return CreatedAtAction(nameof(GetHabit), new { id = habit.Id }, habitDto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateHabit(string id, UpdateHabitDto updateHabitDto)
    {
        Habit? habit = await dbContext.Habits.FirstOrDefaultAsync(h => h.Id == id);

        if (habit is null)
        {
            return NotFound();
        }

        habit.UpdateFromDto(updateHabitDto);

        await dbContext.SaveChangesAsync();

        return NoContent();
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult> PatchHabit(string id, JsonPatchDocument<HabitDto> patchDocument)
    {
        Habit? habit = await dbContext.Habits.FirstOrDefaultAsync(h => h.Id == id);

        if (habit is null)
        {
            return NotFound();
        }

        HabitDto habitDto = habit.ToDto();

        patchDocument.ApplyTo(habitDto, ModelState);

        // if (!ModelState.IsValid)
        // {
        //     return ValidationProblem(ModelState);
        // }

        if (!TryValidateModel(habitDto))
        {
            return ValidationProblem(ModelState);
        }

        habit.Name = habitDto.Name;
        habit.Description = habitDto.Description;
        habit.UpdatedAtUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteHabit(string id)
    {
        Habit? habit = await dbContext.Habits.FirstOrDefaultAsync(h => h.Id == id);

        if (habit is null)
        {
            return NotFound();
        }

        // if (habit is null)
        // {
        //     return StatusCode(StatusCodes.Status410Gone);
        // }

        dbContext.Habits.Remove(habit);

        await dbContext.SaveChangesAsync();

        return NoContent();
    }

    private List<LinkDto> CreateLinksForHabits(
        HabitsQueryParameters parameters,
        bool hasNextPage,
        bool hasPreviousPage)
    {
        List<LinkDto> links =
        [
            linkService.Create(nameof(GetHabits), "self", HttpMethods.Get, new
            {
                page = parameters.Page,
                pageSize = parameters.PageSize,
                fields = parameters.Fields,
                q = parameters.Search,
                sort = parameters.Sort,
                type = parameters.Type,
                status = parameters.Status
            }),
            linkService.Create(nameof(CreateHabit), "create", HttpMethods.Post)
        ];

        if (hasNextPage)
        {
            links.Add(linkService.Create(nameof(GetHabits), "next-page", HttpMethods.Get, new
            {
                page = parameters.Page + 1,
                pageSize = parameters.PageSize,
                fields = parameters.Fields,
                q = parameters.Search,
                sort = parameters.Sort,
                type = parameters.Type,
                status = parameters.Status
            }));
        }

        if (hasPreviousPage)
        {
            links.Add(linkService.Create(nameof(GetHabits), "previous-page", HttpMethods.Get, new
            {
                page = parameters.Page - 1,
                pageSize = parameters.PageSize,
                fields = parameters.Fields,
                q = parameters.Search,
                sort = parameters.Sort,
                type = parameters.Type,
                status = parameters.Status
            }));
        }

        return links;
    }

    private List<LinkDto> CreateLinksForHabit(string id, string? fields)
    {
        List<LinkDto> links =
        [
            linkService.Create(nameof(GetHabit), "self", HttpMethods.Get, new { id, fields }),
            linkService.Create(nameof(UpdateHabit), "update", HttpMethods.Put, new { id }),
            linkService.Create(nameof(PatchHabit), "partial-update", HttpMethods.Patch, new { id }),
            linkService.Create(nameof(DeleteHabit), "delete", HttpMethods.Delete, new { id }),
            linkService.Create(
                nameof(HabitTagsController.UpsertHabitTags),
                "upsert-tags",
                HttpMethods.Put,
                new { habitId = id },
                HabitTagsController.Name)
        ];

        return links;
    }
}
