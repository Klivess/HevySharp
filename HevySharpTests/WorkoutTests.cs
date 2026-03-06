using HevySharp;
using HevySharp.Schemas;

namespace HevySharpTests;

public class WorkoutTests
{
    private MockHttpMessageHandler handler = null!;
    private HevyAPI api = null!;

    [TearDown]
    public void TearDown() => handler?.Dispose();

    private const string WorkoutJson = """
    {
        "id": "wk_123abc",
        "title": "Push Day",
        "description": "Felt strong today",
        "start_time": "2026-03-05T18:00:00Z",
        "end_time": "2026-03-05T19:30:00Z",
        "exercises": [
            {
                "exercise_template_id": "ex_bench_press",
                "notes": "Smooth reps",
                "superset_id": null,
                "sets": [
                    { "type": "warmup", "weight_kg": 60, "reps": 10 },
                    { "type": "normal", "weight_kg": 100, "reps": 8 },
                    { "type": "failure", "weight_kg": 100, "reps": 6 }
                ]
            }
        ]
    }
    """;

    [SetUp]
    public async Task SetUp()
    {
        handler = new MockHttpMessageHandler();
        handler.SetOkResponse("/v1/user/info", """{"username":"test"}""");
        api = new HevyAPI(new HttpClient(handler));
        await api.AuthoriseHevy("key");
    }

    [Test]
    public async Task GetWorkouts_DeserializesPaginatedResponse()
    {
        handler.SetOkResponse("/v1/workouts?page=", $$"""
        {
            "page": 1,
            "page_count": 3,
            "workouts": [{{WorkoutJson}}]
        }
        """);

        var result = await api.GetWorkouts(1, 5);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Page, Is.EqualTo(1));
        Assert.That(result.PageCount, Is.EqualTo(3));
        Assert.That(result.Workouts, Has.Count.EqualTo(1));
        Assert.That(result.Workouts![0].Title, Is.EqualTo("Push Day"));
    }

    [Test]
    public async Task GetWorkout_DeserializesFullWorkout()
    {
        handler.SetOkResponse("/v1/workouts/wk_123abc", WorkoutJson);

        var workout = await api.GetWorkout("wk_123abc");

        Assert.That(workout, Is.Not.Null);
        Assert.That(workout!.Id, Is.EqualTo("wk_123abc"));
        Assert.That(workout.Title, Is.EqualTo("Push Day"));
        Assert.That(workout.Description, Is.EqualTo("Felt strong today"));
        Assert.That(workout.StartTime, Is.EqualTo("2026-03-05T18:00:00Z"));
        Assert.That(workout.EndTime, Is.EqualTo("2026-03-05T19:30:00Z"));
        Assert.That(workout.Exercises, Has.Count.EqualTo(1));

        var exercise = workout.Exercises![0];
        Assert.That(exercise.ExerciseTemplateId, Is.EqualTo("ex_bench_press"));
        Assert.That(exercise.Notes, Is.EqualTo("Smooth reps"));
        Assert.That(exercise.Sets, Has.Count.EqualTo(3));
        Assert.That(exercise.Sets![0].Type, Is.EqualTo("warmup"));
        Assert.That(exercise.Sets[0].WeightKg, Is.EqualTo(60));
        Assert.That(exercise.Sets[0].Reps, Is.EqualTo(10));
        Assert.That(exercise.Sets[2].Type, Is.EqualTo("failure"));
    }

    [Test]
    public async Task CreateWorkout_WrapsBodyInWorkoutKey()
    {
        handler.SetOkResponse("/v1/workouts", WorkoutJson);

        var workout = new HevyWorkout
        {
            Title = "Push Day",
            StartTime = "2026-03-05T18:00:00Z",
            EndTime = "2026-03-05T19:30:00Z",
            Exercises = []
        };

        await api.CreateWorkout(workout);

        var body = await handler.GetLastRequestBody();
        Assert.That(body, Is.Not.Null);
        Assert.That(body, Does.Contain("\"workout\""));
        Assert.That(body, Does.Contain("\"title\""));
    }

    [Test]
    public async Task UpdateWorkout_OmitsIdFromPayload()
    {
        handler.SetOkResponse("/v1/workouts/wk_123abc", WorkoutJson);

        var workout = new HevyWorkout
        {
            Id = "wk_123abc",
            Title = "Updated Push Day",
            StartTime = "2026-03-05T18:00:00Z",
            EndTime = "2026-03-05T19:30:00Z"
        };

        await api.UpdateWorkout("wk_123abc", workout);

        var body = await handler.GetLastRequestBody();
        Assert.That(body, Is.Not.Null);
        Assert.That(body, Does.Not.Contain("\"id\""));
        Assert.That(body, Does.Contain("\"workout\""));
    }

    [Test]
    public async Task GetWorkoutCount_ReturnsCount()
    {
        handler.SetOkResponse("/v1/workouts/count", """{"count": 245}""");

        var count = await api.GetWorkoutCount();

        Assert.That(count, Is.EqualTo(245));
    }

    [Test]
    public async Task GetWorkoutEvents_DeserializesPaginatedResponse()
    {
        handler.SetOkResponse("/v1/workouts/events", """
        {
            "page": 1,
            "page_count": 1,
            "events": [
                {
                    "id": "evt_1",
                    "type": "updated",
                    "workout": { "id": "wk_123abc", "title": "Push Day" }
                }
            ]
        }
        """);

        var result = await api.GetWorkoutEvents();

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Events, Has.Count.EqualTo(1));
        Assert.That(result.Events![0].Type, Is.EqualTo("updated"));
        Assert.That(result.Events[0].Workout?.Id, Is.EqualTo("wk_123abc"));
    }
}
