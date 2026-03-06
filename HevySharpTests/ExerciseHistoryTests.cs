using HevySharp;

namespace HevySharpTests;

public class ExerciseHistoryTests
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
        api = new HevyAPI(new HttpClient(handler));
        await api.AuthoriseHevy("key");
    }

    [Test]
    public async Task GetExerciseHistory_DeserializesResponse()
    {
        handler.SetOkResponse("/v1/exercise_history/ex_bench_press", """
        {
            "exercise_template_id": "ex_bench_press",
            "history": [
                {
                    "workout_id": "wk_123abc",
                    "date": "2026-03-05T18:00:00Z",
                    "sets": [
                        { "type": "normal", "weight_kg": 100, "reps": 8 }
                    ]
                }
            ]
        }
        """);

        var result = await api.GetExerciseHistory("ex_bench_press");

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.ExerciseTemplateId, Is.EqualTo("ex_bench_press"));
        Assert.That(result.History, Has.Count.EqualTo(1));

        var entry = result.History![0];
        Assert.That(entry.WorkoutId, Is.EqualTo("wk_123abc"));
        Assert.That(entry.Date, Is.EqualTo("2026-03-05T18:00:00Z"));
        Assert.That(entry.Sets, Has.Count.EqualTo(1));
        Assert.That(entry.Sets![0].Type, Is.EqualTo("normal"));
        Assert.That(entry.Sets[0].WeightKg, Is.EqualTo(100));
        Assert.That(entry.Sets[0].Reps, Is.EqualTo(8));
    }
}
