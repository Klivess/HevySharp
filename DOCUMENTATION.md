# HevySharp — Full API Reference

Complete reference for every class, method, and model in the HevySharp library.

---

## Table of Contents

- [HevyAPI Client](#hevyapi-client)
  - [Constructors](#constructors)
  - [Authentication](#authentication)
  - [User](#user)
  - [Workouts](#workouts)
  - [Routines](#routines)
  - [Routine Folders](#routine-folders)
  - [Exercise Templates](#exercise-templates)
  - [Exercise History](#exercise-history)
- [Models (Schemas)](#models-schemas)
  - [HevyWorkout](#hevyworkout)
  - [HevyRoutine](#hevyroutine)
  - [HevyRoutineFolder](#hevyroutinefolder)
  - [HevyExercise](#hevyexercise)
  - [HevySet](#hevyset)
  - [HevyExerciseTemplate](#hevyexercisetemplate)
  - [HevyExerciseHistory](#hevyexercisehistory)
  - [HevyExerciseHistoryEntry](#hevyexercisehistoryentry)
  - [HevyUserInfo](#hevyuserinfo)
  - [HevyWorkoutEvent](#hevyworkoutevent)
- [Paginated Responses](#paginated-responses)
  - [PaginatedWorkoutResponse](#paginatedworkoutresponse)
  - [PaginatedRoutineResponse](#paginatedroutineresponse)
  - [PaginatedRoutineFolderResponse](#paginatedroutinefolderresponse)
  - [PaginatedExerciseTemplateResponse](#paginatedexercisetemplateresponse)
  - [PaginatedWorkoutEventResponse](#paginatedworkouteventresponse)
  - [WorkoutCountResponse](#workoutcountresponse)
- [Enum Values Reference](#enum-values-reference)
- [Automatic Behaviours](#automatic-behaviours)
- [Error Handling](#error-handling)
- [Testing and Dependency Injection](#testing-and-dependency-injection)

---

## HevyAPI Client

**Namespace:** `HevySharp`

The main entry point for interacting with the Hevy API.

### Constructors

```csharp
// Creates a new HevyAPI instance with a default HttpClient
public HevyAPI()

// Creates a new HevyAPI instance with a provided HttpClient (for DI or testing)
public HevyAPI(HttpClient httpClient)
```

### Properties

| Property | Type | Description |
|---|---|---|
| `IsAuthorised` | `bool` | `true` after a successful call to `AuthoriseHevy`. Read-only. |

---

### Authentication

#### `AuthoriseHevy`

```csharp
public async Task<bool> AuthoriseHevy(string apiKey)
```

Validates the API key by calling `GET /v1/user/info`. On success, sets `IsAuthorised = true` and attaches the key to all subsequent requests.

| Parameter | Type | Description |
|---|---|---|
| `apiKey` | `string` | Your Hevy developer API key |

**Returns:** `true` if the key is valid, `false` otherwise.

> All other methods throw `InvalidOperationException` if called before a successful authorisation.

---

### User

#### `GetUserInfo`

```csharp
public async Task<HevyUserInfo?> GetUserInfo()
```

Retrieves the current user's account info.

**API endpoint:** `GET /v1/user/info`

**Returns:** `HevyUserInfo` with username, account creation date, and preferred units.

**Example:**

```csharp
var user = await api.GetUserInfo();
Console.WriteLine($"{user!.Username} uses {user.WeightUnit}");
```

---

### Workouts

#### `GetWorkouts`

```csharp
public async Task<PaginatedWorkoutResponse?> GetWorkouts(int page = 1, int pageSize = 5)
```

Gets a paginated list of workouts ordered newest to oldest.

| Parameter | Type | Default | Description |
|---|---|---|---|
| `page` | `int` | `1` | Page number (1-based) |
| `pageSize` | `int` | `5` | Items per page (max 10) |

**API endpoint:** `GET /v1/workouts?page={page}&pageSize={pageSize}`

**Returns:** `PaginatedWorkoutResponse` containing `Page`, `PageCount`, and `Workouts` list.

---

#### `GetWorkout`

```csharp
public async Task<HevyWorkout?> GetWorkout(string workoutId)
```

Gets a single workout by ID with all exercises and sets.

| Parameter | Type | Description |
|---|---|---|
| `workoutId` | `string` | The workout ID (e.g. `"wk_123abc"`) |

**API endpoint:** `GET /v1/workouts/{workoutId}`

**Returns:** `HevyWorkout`

---

#### `CreateWorkout`

```csharp
public async Task<HevyWorkout?> CreateWorkout(HevyWorkout workout)
```

Creates a new workout. The library automatically:
- Wraps the body in `{"workout": {...}}`
- Sanitizes `@` in title, description, and exercise notes

| Parameter | Type | Description |
|---|---|---|
| `workout` | `HevyWorkout` | Workout to create. `Id` is ignored. |

**API endpoint:** `POST /v1/workouts`

**Returns:** The created `HevyWorkout` (with server-assigned `Id`).

**Example:**

```csharp
var workout = new HevyWorkout
{
    Title     = "Push Day",
    StartTime = "2026-03-05T18:00:00Z",
    EndTime   = "2026-03-05T19:30:00Z",
    Exercises =
    [
        new HevyExercise
        {
            ExerciseTemplateId = "ex_bench_press",
            Notes = "Smooth reps",
            Sets =
            [
                new HevySet { Type = "warmup", WeightKg = 60,  Reps = 10 },
                new HevySet { Type = "normal", WeightKg = 100, Reps = 8  }
            ]
        }
    ]
};
var created = await api.CreateWorkout(workout);
```

---

#### `UpdateWorkout`

```csharp
public async Task<HevyWorkout?> UpdateWorkout(string workoutId, HevyWorkout workout)
```

Updates an existing workout. The library automatically:
- Wraps the body in `{"workout": {...}}`
- Nulls out the `Id` field so it is omitted from the payload
- Sanitizes `@` in text fields

| Parameter | Type | Description |
|---|---|---|
| `workoutId` | `string` | ID of the workout to update |
| `workout` | `HevyWorkout` | Updated workout data |

**API endpoint:** `PUT /v1/workouts/{workoutId}`

**Returns:** The updated `HevyWorkout`.

---

#### `GetWorkoutCount`

```csharp
public async Task<int> GetWorkoutCount()
```

Gets the total number of workouts on the account.

**API endpoint:** `GET /v1/workouts/count`

**Returns:** `int` — the workout count.

---

#### `GetWorkoutEvents`

```csharp
public async Task<PaginatedWorkoutEventResponse?> GetWorkoutEvents(int page = 1, int pageSize = 5)
```

Retrieves a paginated list of workout events (updates or deletes) useful for keeping a local cache in sync.

| Parameter | Type | Default | Description |
|---|---|---|---|
| `page` | `int` | `1` | Page number |
| `pageSize` | `int` | `5` | Items per page (max 10) |

**API endpoint:** `GET /v1/workouts/events?page={page}&pageSize={pageSize}`

**Returns:** `PaginatedWorkoutEventResponse` containing `HevyWorkoutEvent` items (each with a `Type` of `"updated"` or `"deleted"` and an optional `Workout`).

---

### Routines

#### `GetRoutines`

```csharp
public async Task<PaginatedRoutineResponse?> GetRoutines(int page = 1, int pageSize = 5)
```

Gets a paginated list of saved routines.

| Parameter | Type | Default | Description |
|---|---|---|---|
| `page` | `int` | `1` | Page number |
| `pageSize` | `int` | `5` | Items per page (max 10) |

**API endpoint:** `GET /v1/routines?page={page}&pageSize={pageSize}`

---

#### `GetRoutine`

```csharp
public async Task<HevyRoutine?> GetRoutine(string routineId)
```

Gets a single routine by ID.

**API endpoint:** `GET /v1/routines/{routineId}`

---

#### `CreateRoutine`

```csharp
public async Task<HevyRoutine?> CreateRoutine(HevyRoutine routine)
```

Creates a new routine. Body is wrapped in `{"routine": {...}}`. Text fields are sanitized.

**API endpoint:** `POST /v1/routines`

**Example:**

```csharp
var routine = new HevyRoutine
{
    Title    = "Upper Body Power",
    FolderId = "fld_789ghi",
    Notes    = "Focus on explosive movements",
    Exercises =
    [
        new HevyExercise
        {
            ExerciseTemplateId = "ex_overhead_press",
            RestSeconds = 120,
            Sets = [ new HevySet { Type = "normal", Reps = 5 } ]
        }
    ]
};
var created = await api.CreateRoutine(routine);
```

---

#### `UpdateRoutine`

```csharp
public async Task<HevyRoutine?> UpdateRoutine(string routineId, HevyRoutine routine)
```

Updates an existing routine. Automatically omits `Id` and `UpdatedAt` from the payload.

**API endpoint:** `PUT /v1/routines/{routineId}`

---

### Routine Folders

#### `GetRoutineFolders`

```csharp
public async Task<PaginatedRoutineFolderResponse?> GetRoutineFolders(int page = 1, int pageSize = 5)
```

Gets a paginated list of routine folders.

**API endpoint:** `GET /v1/routine_folders?page={page}&pageSize={pageSize}`

---

#### `GetRoutineFolder`

```csharp
public async Task<HevyRoutineFolder?> GetRoutineFolder(string folderId)
```

Gets a single routine folder by ID.

**API endpoint:** `GET /v1/routine_folders/{folderId}`

---

#### `CreateRoutineFolder`

```csharp
public async Task<HevyRoutineFolder?> CreateRoutineFolder(HevyRoutineFolder routineFolder)
```

Creates a new routine folder. New folders are inserted at index 0, pushing existing ones down. Body is wrapped in `{"routine_folder": {...}}`.

**API endpoint:** `POST /v1/routine_folders`

**Example:**

```csharp
var folder = await api.CreateRoutineFolder(
    new HevyRoutineFolder { Title = "Powerlifting Programs" });
```

---

### Exercise Templates

#### `GetExerciseTemplates`

```csharp
public async Task<PaginatedExerciseTemplateResponse?> GetExerciseTemplates(int page = 1, int pageSize = 5)
```

Gets a paginated list of all default and custom exercise templates.

**API endpoint:** `GET /v1/exercise_templates?page={page}&pageSize={pageSize}`

---

#### `GetExerciseTemplate`

```csharp
public async Task<HevyExerciseTemplate?> GetExerciseTemplate(string exerciseTemplateId)
```

Gets a single exercise template by ID.

**API endpoint:** `GET /v1/exercise_templates/{exerciseTemplateId}`

---

#### `CreateExerciseTemplate`

```csharp
public async Task<HevyExerciseTemplate?> CreateExerciseTemplate(HevyExerciseTemplate exerciseTemplate)
```

Creates a new custom exercise template. Body is wrapped in `{"exercise_template": {...}}`.

**API endpoint:** `POST /v1/exercise_templates`

**Example:**

```csharp
var template = await api.CreateExerciseTemplate(new HevyExerciseTemplate
{
    Title             = "Zercher Squat",
    MuscleGroup       = "legs",
    EquipmentCategory = "barbell"
});
```

---

### Exercise History

#### `GetExerciseHistory`

```csharp
public async Task<HevyExerciseHistory?> GetExerciseHistory(string exerciseTemplateId, int page = 1, int pageSize = 5)
```

Retrieves the historical performance data for a specific exercise over time.

| Parameter | Type | Default | Description |
|---|---|---|---|
| `exerciseTemplateId` | `string` | — | Exercise template ID to look up |
| `page` | `int` | `1` | Page number |
| `pageSize` | `int` | `5` | Items per page (max 10) |

**API endpoint:** `GET /v1/exercise_history/{exerciseTemplateId}?page={page}&pageSize={pageSize}`

**Returns:** `HevyExerciseHistory` containing a list of `HevyExerciseHistoryEntry` items, each with a workout ID, date, and sets.

---

## Models (Schemas)

All models are in the `HevySharp.Schemas` namespace. Properties use `System.Text.Json` `[JsonPropertyName]` attributes to map to the API's `snake_case` JSON fields.

### HevyWorkout

| Property | Type | JSON Key | Description |
|---|---|---|---|
| `Id` | `string?` | `id` | Workout ID (read-only, server-assigned) |
| `Title` | `string?` | `title` | Workout name |
| `Description` | `string?` | `description` | Optional description/notes |
| `StartTime` | `string?` | `start_time` | ISO 8601 start timestamp |
| `EndTime` | `string?` | `end_time` | ISO 8601 end timestamp |
| `Exercises` | `List<HevyExercise>?` | `exercises` | Exercises performed |

---

### HevyRoutine

| Property | Type | JSON Key | Description |
|---|---|---|---|
| `Id` | `string?` | `id` | Routine ID (read-only) |
| `FolderId` | `string?` | `folder_id` | Parent folder ID |
| `Title` | `string?` | `title` | Routine name |
| `Notes` | `string?` | `notes` | Optional notes |
| `UpdatedAt` | `string?` | `updated_at` | Last update timestamp (read-only) |
| `Exercises` | `List<HevyExercise>?` | `exercises` | Routine exercises |

---

### HevyRoutineFolder

| Property | Type | JSON Key | Description |
|---|---|---|---|
| `Id` | `string?` | `id` | Folder ID (read-only) |
| `Title` | `string?` | `title` | Folder name |

---

### HevyExercise

A single exercise entry within a workout or routine.

| Property | Type | JSON Key | Description |
|---|---|---|---|
| `ExerciseTemplateId` | `string?` | `exercise_template_id` | Reference to the exercise template |
| `Notes` | `string?` | `notes` | Optional notes (workouts) |
| `SupersetId` | `string?` | `superset_id` | Superset grouping ID (workouts) |
| `RestSeconds` | `int?` | `rest_seconds` | Rest period in seconds (routines) |
| `Sets` | `List<HevySet>?` | `sets` | Sets performed or planned |

> `Notes` and `SupersetId` are typically used in workouts. `RestSeconds` is typically used in routines. All are nullable and omitted from JSON when null.

---

### HevySet

A single set within an exercise.

| Property | Type | JSON Key | Description |
|---|---|---|---|
| `Type` | `string?` | `type` | Set type (see [Enum Values](#enum-values-reference)) |
| `WeightKg` | `double?` | `weight_kg` | Weight in kilograms (null for bodyweight) |
| `Reps` | `int?` | `reps` | Number of repetitions |

---

### HevyExerciseTemplate

| Property | Type | JSON Key | Description |
|---|---|---|---|
| `Id` | `string?` | `id` | Template ID (read-only for defaults) |
| `Title` | `string?` | `title` | Exercise name |
| `MuscleGroup` | `string?` | `muscle_group` | Primary muscle group (see [Enum Values](#enum-values-reference)) |
| `EquipmentCategory` | `string?` | `equipment_category` | Equipment type (see [Enum Values](#enum-values-reference)) |
| `IsCustom` | `bool?` | `is_custom` | `true` if user-created |

---

### HevyExerciseHistory

| Property | Type | JSON Key | Description |
|---|---|---|---|
| `ExerciseTemplateId` | `string?` | `exercise_template_id` | The exercise this history is for |
| `History` | `List<HevyExerciseHistoryEntry>?` | `history` | List of historical entries |

---

### HevyExerciseHistoryEntry

| Property | Type | JSON Key | Description |
|---|---|---|---|
| `WorkoutId` | `string?` | `workout_id` | Workout that contained this exercise |
| `Date` | `string?` | `date` | ISO 8601 date of the workout |
| `Sets` | `List<HevySet>?` | `sets` | Sets performed |

---

### HevyUserInfo

| Property | Type | JSON Key | Description |
|---|---|---|---|
| `Username` | `string?` | `username` | Hevy username |
| `CreatedAt` | `string?` | `created_at` | Account creation date |
| `WeightUnit` | `string?` | `weight_unit` | Preferred weight unit (`"kg"` or `"lbs"`) |
| `DistanceUnit` | `string?` | `distance_unit` | Preferred distance unit (`"km"` or `"mi"`) |

---

### HevyWorkoutEvent

| Property | Type | JSON Key | Description |
|---|---|---|---|
| `Id` | `string?` | `id` | Event ID |
| `Type` | `string?` | `type` | `"updated"` or `"deleted"` |
| `Workout` | `HevyWorkout?` | `workout` | The workout data (null for deletes) |

---

## Paginated Responses

All list endpoints return paginated responses. Each shares a common structure:

| Property | Type | Description |
|---|---|---|
| `Page` | `int` | Current page number |
| `PageCount` | `int` | Total number of pages |

### PaginatedWorkoutResponse

Additional property: `Workouts` (`List<HevyWorkout>?`)

### PaginatedRoutineResponse

Additional property: `Routines` (`List<HevyRoutine>?`)

### PaginatedRoutineFolderResponse

Additional property: `RoutineFolders` (`List<HevyRoutineFolder>?`)

### PaginatedExerciseTemplateResponse

Additional property: `ExerciseTemplates` (`List<HevyExerciseTemplate>?`)

### PaginatedWorkoutEventResponse

Additional property: `Events` (`List<HevyWorkoutEvent>?`)

### WorkoutCountResponse

| Property | Type | Description |
|---|---|---|
| `Count` | `int` | Total number of workouts |

---

## Enum Values Reference

These are the valid string values accepted by the Hevy API for categorised fields.

### Set Type (`HevySet.Type`)

| Value | Description |
|---|---|
| `"normal"` | Standard working set |
| `"warmup"` | Warm-up set |
| `"drop"` | Drop set |
| `"failure"` | Set taken to failure |

### Muscle Group (`HevyExerciseTemplate.MuscleGroup`)

| Value |
|---|
| `"chest"` |
| `"back"` |
| `"legs"` |
| `"arms"` |
| `"shoulders"` |
| `"core"` |
| `"full_body"` |
| `"cardio"` |

### Equipment Category (`HevyExerciseTemplate.EquipmentCategory`)

| Value |
|---|
| `"barbell"` |
| `"dumbbell"` |
| `"machine"` |
| `"cable"` |
| `"bodyweight"` |
| `"kettlebell"` |
| `"none"` |

---

## Automatic Behaviours

HevySharp handles several Hevy API quirks behind the scenes so you don't have to:

### 1. JSON Body Wrapping

The Hevy API requires POST and PUT bodies to be wrapped in their resource name. HevySharp does this automatically:

| Method | JSON Wrapper Key |
|---|---|
| `CreateWorkout` / `UpdateWorkout` | `"workout"` |
| `CreateRoutine` / `UpdateRoutine` | `"routine"` |
| `CreateRoutineFolder` | `"routine_folder"` |
| `CreateExerciseTemplate` | `"exercise_template"` |

### 2. `@` Symbol Sanitization

The Hevy API silently fails when any text field contains the `@` character. Before every POST or PUT, HevySharp replaces `@` with `at` in:

- Workout: `Title`, `Description`, and exercise `Notes`
- Routine: `Title`, `Notes`, and exercise `Notes`
- Routine Folder: `Title`
- Exercise Template: `Title`

### 3. Read-Only Field Stripping

PUT endpoints reject payloads that include read-only fields. Before sending an update, HevySharp sets these to `null` (which are then omitted from JSON via `JsonIgnoreCondition.WhenWritingNull`):

| Method | Fields Stripped |
|---|---|
| `UpdateWorkout` | `Id` |
| `UpdateRoutine` | `Id`, `UpdatedAt` |

---

## Error Handling

- **Not authorised:** All methods (except `AuthoriseHevy`) throw `InvalidOperationException` if `IsAuthorised` is `false`.
- **HTTP errors:** Methods call `response.EnsureSuccessStatusCode()` which throws `HttpRequestException` on non-2xx responses. Catch this to handle API errors:

```csharp
try
{
    var workout = await api.GetWorkout("invalid_id");
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"API error: {ex.StatusCode} - {ex.Message}");
}
```

---

## Testing and Dependency Injection

HevySharp accepts an `HttpClient` in its constructor, making it straightforward to inject a mocked handler for unit testing:

```csharp
// Create a mock handler
var handler = new MockHttpMessageHandler();
handler.SetOkResponse("/v1/user/info", """{"username":"test"}""");
handler.SetOkResponse("/v1/workouts/count", """{"count":42}""");

// Inject into HevyAPI
var api = new HevyAPI(new HttpClient(handler));
await api.AuthoriseHevy("test-key");

// Test as usual
var count = await api.GetWorkoutCount();
Assert.That(count, Is.EqualTo(42));
```

The test project (`HevySharpTests`) includes a complete `MockHttpMessageHandler` and 29 tests covering all endpoints, body wrapping, sanitization, and read-only field stripping.
