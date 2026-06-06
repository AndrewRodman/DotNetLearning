using TaskApi.Models;

namespace TaskApi.Tests;

public class TaskItemTests
{
    [Fact]
    public void NewTask_IsNotComplete_ByDefault()
    {
        var task = new TaskItem { Title = "Learn EF Core" };

        Assert.False(task.IsComplete);
        Assert.Equal("Learn EF Core", task.Title);
    }
}