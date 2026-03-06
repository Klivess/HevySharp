using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using HevySharp.Schemas;

namespace HevySharp;

public class HevyAPI
{
    private const string BaseUrl = "https://api.hevyapp.com";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly HttpClient client;
    public bool IsAuthorised { get; private set; }

    public HevyAPI()
    {
        client = new HttpClient();
    }

    public HevyAPI(HttpClient httpClient)
    {
        client = httpClient;
    }

    //Authentication

    public async Task<bool> AuthoriseHevy(string apiKey)
    {
        client.DefaultRequestHeaders.Remove("api-key");
        client.DefaultRequestHeaders.Add("api-key", apiKey);

        var response = await client.GetAsync($"{BaseUrl}/v1/user/info");
        IsAuthorised = response.IsSuccessStatusCode;
        return IsAuthorised;
    }

    //User

    public async Task<HevyUserInfo?> GetUserInfo()
    {
        EnsureAuthorised();
        var response = await client.GetAsync($"{BaseUrl}/v1/user/info");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<HevyUserInfo>(json, JsonOptions);
    }

    //Workouts

    public async Task<PaginatedWorkoutResponse?> GetWorkouts(int page = 1, int pageSize = 5)
    {
        EnsureAuthorised();
        var response = await client.GetAsync($"{BaseUrl}/v1/workouts?page={page}&pageSize={pageSize}");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<PaginatedWorkoutResponse>(json, JsonOptions);
    }

    public async Task<HevyWorkout?> GetWorkout(string workoutId)
    {
        EnsureAuthorised();
        var response = await client.GetAsync($"{BaseUrl}/v1/workouts/{workoutId}");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<HevyWorkout>(json, JsonOptions);
    }

    public async Task<HevyWorkout?> CreateWorkout(HevyWorkout workout)
    {
        EnsureAuthorised();
        SanitizeWorkout(workout);
        var payload = JsonSerializer.Serialize(new { workout }, JsonOptions);
        var content = new StringContent(payload, Encoding.UTF8, "application/json");
        var response = await client.PostAsync($"{BaseUrl}/v1/workouts", content);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<HevyWorkout>(json, JsonOptions);
    }

    public async Task<HevyWorkout?> UpdateWorkout(string workoutId, HevyWorkout workout)
    {
        EnsureAuthorised();
        SanitizeWorkout(workout);
        // Omit read-only fields for PUT
        workout.Id = null;
        var payload = JsonSerializer.Serialize(new { workout }, JsonOptions);
        var content = new StringContent(payload, Encoding.UTF8, "application/json");
        var response = await client.PutAsync($"{BaseUrl}/v1/workouts/{workoutId}", content);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<HevyWorkout>(json, JsonOptions);
    }

    public async Task<int> GetWorkoutCount()
    {
        EnsureAuthorised();
        var response = await client.GetAsync($"{BaseUrl}/v1/workouts/count");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<WorkoutCountResponse>(json, JsonOptions);
        return result?.Count ?? 0;
    }

    public async Task<PaginatedWorkoutEventResponse?> GetWorkoutEvents(int page = 1, int pageSize = 5)
    {
        EnsureAuthorised();
        var response = await client.GetAsync($"{BaseUrl}/v1/workouts/events?page={page}&pageSize={pageSize}");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<PaginatedWorkoutEventResponse>(json, JsonOptions);
    }

    // Routines

    public async Task<PaginatedRoutineResponse?> GetRoutines(int page = 1, int pageSize = 5)
    {
        EnsureAuthorised();
        var response = await client.GetAsync($"{BaseUrl}/v1/routines?page={page}&pageSize={pageSize}");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<PaginatedRoutineResponse>(json, JsonOptions);
    }

    public async Task<HevyRoutine?> GetRoutine(string routineId)
    {
        EnsureAuthorised();
        var response = await client.GetAsync($"{BaseUrl}/v1/routines/{routineId}");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<HevyRoutine>(json, JsonOptions);
    }

    public async Task<HevyRoutine?> CreateRoutine(HevyRoutine routine)
    {
        EnsureAuthorised();
        SanitizeRoutine(routine);
        var payload = JsonSerializer.Serialize(new { routine }, JsonOptions);
        var content = new StringContent(payload, Encoding.UTF8, "application/json");
        var response = await client.PostAsync($"{BaseUrl}/v1/routines", content);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<HevyRoutine>(json, JsonOptions);
    }

    public async Task<HevyRoutine?> UpdateRoutine(string routineId, HevyRoutine routine)
    {
        EnsureAuthorised();
        SanitizeRoutine(routine);
        // Omit read-only fields for PUT
        routine.Id = null;
        routine.UpdatedAt = null;
        var payload = JsonSerializer.Serialize(new { routine }, JsonOptions);
        var content = new StringContent(payload, Encoding.UTF8, "application/json");
        var response = await client.PutAsync($"{BaseUrl}/v1/routines/{routineId}", content);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<HevyRoutine>(json, JsonOptions);
    }

    //Routine Folders

    public async Task<PaginatedRoutineFolderResponse?> GetRoutineFolders(int page = 1, int pageSize = 5)
    {
        EnsureAuthorised();
        var response = await client.GetAsync($"{BaseUrl}/v1/routine_folders?page={page}&pageSize={pageSize}");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<PaginatedRoutineFolderResponse>(json, JsonOptions);
    }

    public async Task<HevyRoutineFolder?> GetRoutineFolder(string folderId)
    {
        EnsureAuthorised();
        var response = await client.GetAsync($"{BaseUrl}/v1/routine_folders/{folderId}");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<HevyRoutineFolder>(json, JsonOptions);
    }

    public async Task<HevyRoutineFolder?> CreateRoutineFolder(HevyRoutineFolder routineFolder)
    {
        EnsureAuthorised();
        routineFolder.Title = routineFolder.Title?.Replace("@", "at");
        var payload = JsonSerializer.Serialize(new { routine_folder = routineFolder }, JsonOptions);
        var content = new StringContent(payload, Encoding.UTF8, "application/json");
        var response = await client.PostAsync($"{BaseUrl}/v1/routine_folders", content);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<HevyRoutineFolder>(json, JsonOptions);
    }

    //Exercise Templates

    public async Task<PaginatedExerciseTemplateResponse?> GetExerciseTemplates(int page = 1, int pageSize = 5)
    {
        EnsureAuthorised();
        var response = await client.GetAsync($"{BaseUrl}/v1/exercise_templates?page={page}&pageSize={pageSize}");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<PaginatedExerciseTemplateResponse>(json, JsonOptions);
    }

    public async Task<HevyExerciseTemplate?> GetExerciseTemplate(string exerciseTemplateId)
    {
        EnsureAuthorised();
        var response = await client.GetAsync($"{BaseUrl}/v1/exercise_templates/{exerciseTemplateId}");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<HevyExerciseTemplate>(json, JsonOptions);
    }

    public async Task<HevyExerciseTemplate?> CreateExerciseTemplate(HevyExerciseTemplate exerciseTemplate)
    {
        EnsureAuthorised();
        exerciseTemplate.Title = exerciseTemplate.Title?.Replace("@", "at");
        var payload = JsonSerializer.Serialize(new { exercise_template = exerciseTemplate }, JsonOptions);
        var content = new StringContent(payload, Encoding.UTF8, "application/json");
        var response = await client.PostAsync($"{BaseUrl}/v1/exercise_templates", content);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<HevyExerciseTemplate>(json, JsonOptions);
    }

    // Exercise History

    public async Task<HevyExerciseHistory?> GetExerciseHistory(string exerciseTemplateId, int page = 1, int pageSize = 5)
    {
        EnsureAuthorised();
        var response = await client.GetAsync($"{BaseUrl}/v1/exercise_history/{exerciseTemplateId}?page={page}&pageSize={pageSize}");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<HevyExerciseHistory>(json, JsonOptions);
    }

    //Helpers

    private void EnsureAuthorised()
    {
        if (!IsAuthorised)
            throw new InvalidOperationException("Not authorised. Call AuthoriseHevy first.");
    }

    private static void SanitizeWorkout(HevyWorkout workout)
    {
        workout.Title = workout.Title?.Replace("@", "at");
        workout.Description = workout.Description?.Replace("@", "at");
        SanitizeExercises(workout.Exercises);
    }

    private static void SanitizeRoutine(HevyRoutine routine)
    {
        routine.Title = routine.Title?.Replace("@", "at");
        routine.Notes = routine.Notes?.Replace("@", "at");
        SanitizeExercises(routine.Exercises);
    }

    private static void SanitizeExercises(List<HevyExercise>? exercises)
    {
        if (exercises is null) return;
        foreach (var exercise in exercises)
        {
            exercise.Notes = exercise.Notes?.Replace("@", "at");
        }
    }
}
