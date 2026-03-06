using System.Text.Json.Serialization;

namespace HevySharp.Schemas;

public class PaginatedWorkoutResponse
{
    [JsonPropertyName("page")]
    public int Page { get; set; }

    [JsonPropertyName("page_count")]
    public int PageCount { get; set; }

    [JsonPropertyName("workouts")]
    public List<HevyWorkout>? Workouts { get; set; }
}

public class PaginatedRoutineResponse
{
    [JsonPropertyName("page")]
    public int Page { get; set; }

    [JsonPropertyName("page_count")]
    public int PageCount { get; set; }

    [JsonPropertyName("routines")]
    public List<HevyRoutine>? Routines { get; set; }
}

public class PaginatedExerciseTemplateResponse
{
    [JsonPropertyName("page")]
    public int Page { get; set; }

    [JsonPropertyName("page_count")]
    public int PageCount { get; set; }

    [JsonPropertyName("exercise_templates")]
    public List<HevyExerciseTemplate>? ExerciseTemplates { get; set; }
}

public class WorkoutCountResponse
{
    [JsonPropertyName("count")]
    public int Count { get; set; }
}

public class PaginatedWorkoutEventResponse
{
    [JsonPropertyName("page")]
    public int Page { get; set; }

    [JsonPropertyName("page_count")]
    public int PageCount { get; set; }

    [JsonPropertyName("events")]
    public List<HevyWorkoutEvent>? Events { get; set; }
}

public class HevyWorkoutEvent
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("workout")]
    public HevyWorkout? Workout { get; set; }
}

public class PaginatedRoutineFolderResponse
{
    [JsonPropertyName("page")]
    public int Page { get; set; }

    [JsonPropertyName("page_count")]
    public int PageCount { get; set; }

    [JsonPropertyName("routine_folders")]
    public List<HevyRoutineFolder>? RoutineFolders { get; set; }
}
