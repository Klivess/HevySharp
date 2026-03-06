using HevySharp;
using HevySharp.Schemas;

namespace HevySharpTests;

public class RoutineTests
{
    private MockHttpMessageHandler handler = null!;
    private HevyAPI api = null!;

    [TearDown]
    public void TearDown() => handler?.Dispose();

    private const string RoutineJson = """
    {
        "id": "rt_456def",
        "folder_id": "fld_789ghi",
        "title": "Upper Body Power",
        "notes": "Focus on explosive movements",
        "updated_at": "2026-03-01T12:00:00Z",
        "exercises": [
            {
                "exercise_template_id": "ex_overhead_press",
                "rest_seconds": 120,
                "sets": [
                    { "type": "normal", "weight_kg": null, "reps": 5 }
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
    public async Task GetRoutines_DeserializesPaginatedResponse()
    {
        handler.SetOkResponse("/v1/routines?page=", $$"""
        {
            "page": 1,
            "page_count": 2,
            "routines": [{{RoutineJson}}]
        }
        """);

        var result = await api.GetRoutines();

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Page, Is.EqualTo(1));
        Assert.That(result.Routines, Has.Count.EqualTo(1));
        Assert.That(result.Routines![0].Title, Is.EqualTo("Upper Body Power"));
    }

    [Test]
    public async Task GetRoutine_DeserializesFullRoutine()
    {
        handler.SetOkResponse("/v1/routines/rt_456def", RoutineJson);

        var routine = await api.GetRoutine("rt_456def");

        Assert.That(routine, Is.Not.Null);
        Assert.That(routine!.Id, Is.EqualTo("rt_456def"));
        Assert.That(routine.FolderId, Is.EqualTo("fld_789ghi"));
        Assert.That(routine.Title, Is.EqualTo("Upper Body Power"));
        Assert.That(routine.Notes, Is.EqualTo("Focus on explosive movements"));
        Assert.That(routine.UpdatedAt, Is.EqualTo("2026-03-01T12:00:00Z"));
        Assert.That(routine.Exercises, Has.Count.EqualTo(1));

        var exercise = routine.Exercises![0];
        Assert.That(exercise.ExerciseTemplateId, Is.EqualTo("ex_overhead_press"));
        Assert.That(exercise.RestSeconds, Is.EqualTo(120));
        Assert.That(exercise.Sets, Has.Count.EqualTo(1));
        Assert.That(exercise.Sets![0].WeightKg, Is.Null);
        Assert.That(exercise.Sets[0].Reps, Is.EqualTo(5));
    }

    [Test]
    public async Task CreateRoutine_WrapsBodyInRoutineKey()
    {
        handler.SetOkResponse("/v1/routines", RoutineJson);

        var routine = new HevyRoutine
        {
            Title = "Upper Body Power",
            FolderId = "fld_789ghi",
            Exercises = []
        };

        await api.CreateRoutine(routine);

        var body = await handler.GetLastRequestBody();
        Assert.That(body, Is.Not.Null);
        Assert.That(body, Does.Contain("\"routine\""));
        Assert.That(body, Does.Contain("\"title\""));
    }

    [Test]
    public async Task UpdateRoutine_OmitsReadOnlyFields()
    {
        handler.SetOkResponse("/v1/routines/rt_456def", RoutineJson);

        var routine = new HevyRoutine
        {
            Id = "rt_456def",
            Title = "Updated Routine",
            UpdatedAt = "2026-03-01T12:00:00Z"
        };

        await api.UpdateRoutine("rt_456def", routine);

        var body = await handler.GetLastRequestBody();
        Assert.That(body, Is.Not.Null);
        Assert.That(body, Does.Not.Contain("\"id\""));
        Assert.That(body, Does.Not.Contain("\"updated_at\""));
        Assert.That(body, Does.Contain("\"routine\""));
    }
}
