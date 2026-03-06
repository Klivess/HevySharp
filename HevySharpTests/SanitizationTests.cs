using HevySharp;
using HevySharp.Schemas;

namespace HevySharpTests;

public class SanitizationTests
{
    private MockHttpMessageHandler handler = null!;
    private HevyAPI api = null!;

    [TearDown]
    public void TearDown() => handler?.Dispose();

    [SetUp]
    public async Task SetUp()
    {
        handler = new MockHttpMessageHandler();
        handler.SetOkResponse("/v1/user/info", """{"username":"test"}""");
        handler.SetOkResponse("/v1/workouts", """{"id":"wk_1","title":"test"}""");
        handler.SetOkResponse("/v1/routines", """{"id":"rt_1","title":"test"}""");
        handler.SetOkResponse("/v1/routine_folders", """{"id":"fld_1","title":"test"}""");
        handler.SetOkResponse("/v1/exercise_templates", """{"id":"ex_1","title":"test"}""");
        api = new HevyAPI(new HttpClient(handler));
        await api.AuthoriseHevy("key");
    }

    [Test]
    public async Task CreateWorkout_SanitizesAtSymbol_InTitleAndDescription()
    {
        var workout = new HevyWorkout
        {
            Title = "Workout @gym",
            Description = "Contact @trainer",
            StartTime = "2026-03-05T18:00:00Z",
            EndTime = "2026-03-05T19:30:00Z"
        };

        await api.CreateWorkout(workout);

        var body = await handler.GetLastRequestBody();
        Assert.That(body, Does.Not.Contain("@"));
        Assert.That(body, Does.Contain("Workout atgym"));
        Assert.That(body, Does.Contain("Contact attrainer"));
    }

    [Test]
    public async Task CreateWorkout_SanitizesAtSymbol_InExerciseNotes()
    {
        var workout = new HevyWorkout
        {
            Title = "Push",
            StartTime = "2026-03-05T18:00:00Z",
            EndTime = "2026-03-05T19:30:00Z",
            Exercises =
            [
                new HevyExercise
                {
                    ExerciseTemplateId = "ex_1",
                    Notes = "Ask @coach for form check"
                }
            ]
        };

        await api.CreateWorkout(workout);

        var body = await handler.GetLastRequestBody();
        Assert.That(body, Does.Not.Contain("@"));
        Assert.That(body, Does.Contain("Ask atcoach for form check"));
    }

    [Test]
    public async Task UpdateWorkout_SanitizesAtSymbol()
    {
        handler.SetOkResponse("/v1/workouts/wk_1", """{"id":"wk_1","title":"test"}""");

        var workout = new HevyWorkout
        {
            Title = "@morning session",
            StartTime = "2026-03-05T18:00:00Z",
            EndTime = "2026-03-05T19:30:00Z"
        };

        await api.UpdateWorkout("wk_1", workout);

        var body = await handler.GetLastRequestBody();
        Assert.That(body, Does.Not.Contain("@"));
        Assert.That(body, Does.Contain("atmorning session"));
    }

    [Test]
    public async Task CreateRoutine_SanitizesAtSymbol_InTitleAndNotes()
    {
        var routine = new HevyRoutine
        {
            Title = "Routine @home",
            Notes = "Notify @partner",
            Exercises = []
        };

        await api.CreateRoutine(routine);

        var body = await handler.GetLastRequestBody();
        Assert.That(body, Does.Not.Contain("@"));
        Assert.That(body, Does.Contain("Routine athome"));
        Assert.That(body, Does.Contain("Notify atpartner"));
    }

    [Test]
    public async Task CreateRoutineFolder_SanitizesAtSymbol()
    {
        var folder = new HevyRoutineFolder { Title = "Folder @work" };

        await api.CreateRoutineFolder(folder);

        var body = await handler.GetLastRequestBody();
        Assert.That(body, Does.Not.Contain("@"));
        Assert.That(body, Does.Contain("Folder atwork"));
    }

    [Test]
    public async Task CreateExerciseTemplate_SanitizesAtSymbol()
    {
        var template = new HevyExerciseTemplate
        {
            Title = "Press @smith machine",
            MuscleGroup = "chest",
            EquipmentCategory = "machine"
        };

        await api.CreateExerciseTemplate(template);

        var body = await handler.GetLastRequestBody();
        Assert.That(body, Does.Not.Contain("@"));
        Assert.That(body, Does.Contain("Press atsmith machine"));
    }
}
