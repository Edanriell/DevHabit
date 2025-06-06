using DevHabit.Api.Database;
using Microsoft.AspNetCore.Mvc;

namespace DevHabit.Api.Controllers;

public sealed class HabitTagsController(ApplicationDbContext dbContext) : ControllerBase
{
    [HttpPut]
    public async Task<ActionResult> UpsertHabitTags(string habitId, string[] tagIds)
    {
        return Ok();
    }

    [HttpDelete("{tagId}")]
    public async Task<ActionResult> RemoveTagFromHabit(string id, string tagId)
    {
        return Ok();
    }
}
