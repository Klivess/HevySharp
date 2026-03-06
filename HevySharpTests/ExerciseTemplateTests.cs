using HevySharp;
using HevySharp.Schemas;

namespace HevySharpTests;

public class ExerciseTemplateTests
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
    public async Task GetExerciseTemplates_DeserializesPaginatedResponse()
    {
        handler.SetOkResponse("/v1/exercise_templates?page=", """
        {
            "page": 1,
            "page_count": 1,
            "exercise_templates": [
                {
                    "id": "ex_squat",
                    "title": "Barbell Squat",
                    "muscle_group": "legs",
                    "equipment_category": "barbell",
                    "is_custom": false
                }
            ]
        }
        """);

        var result = await api.GetExerciseTemplates();

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.ExerciseTemplates, Has.Count.EqualTo(1));

        var template = result.ExerciseTemplates![0];
        Assert.That(template.Id, Is.EqualTo("ex_squat"));
        Assert.That(template.Title, Is.EqualTo("Barbell Squat"));
        Assert.That(template.MuscleGroup, Is.EqualTo("legs"));
        Assert.That(template.EquipmentCategory, Is.EqualTo("barbell"));
        Assert.That(template.IsCustom, Is.False);
    }

    [Test]
    public async Task GetExerciseTemplate_DeserializesTemplate()
    {
        handler.SetOkResponse("/v1/exercise_templates/ex_squat", """
        {
            "id": "ex_squat",
            "title": "Barbell Squat",
            "muscle_group": "legs",
            "equipment_category": "barbell",
            "is_custom": false
        }
        """);

        var template = await api.GetExerciseTemplate("ex_squat");

        Assert.That(template, Is.Not.Null);
        Assert.That(template!.Id, Is.EqualTo("ex_squat"));
        Assert.That(template.Title, Is.EqualTo("Barbell Squat"));
    }

    [Test]
    public async Task CreateExerciseTemplate_WrapsBodyInExerciseTemplateKey()
    {
        handler.SetOkResponse("/v1/exercise_templates", """
        {
            "id": "ex_custom1",
            "title": "Zercher Squat",
            "muscle_group": "legs",
            "equipment_category": "barbell",
            "is_custom": true
        }
        """);

        var template = new HevyExerciseTemplate
        {
            Title = "Zercher Squat",
            MuscleGroup = "legs",
            EquipmentCategory = "barbell"
        };

        await api.CreateExerciseTemplate(template);

        var body = await handler.GetLastRequestBody();
        Assert.That(body, Is.Not.Null);
        Assert.That(body, Does.Contain("\"exercise_template\""));
        Assert.That(body, Does.Contain("\"title\""));
    }
}
