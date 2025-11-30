# Specyfikacja: Master App — Moduł Treningowy

Dokument zawiera kompletne materiały do przekazania Copilotowi / developerowi: model danych (ERD), schemat SQL dla PostgreSQL, instrukcje Database-First (scaffold), modele EF Core przykładowe (w C#), propozycję REST API, oraz gotowy **Service-Driven Prompt / SLA** dla Copilota, tak aby wygenerował projekt: backend w C# (.NET 8+), front-end w Blazor (WebAssembly lub Server - decyzja do wyboru), i bazę PostgreSQL.

> **Założenia**
> - Single-user na start (brak auth). W przyszłości można dodać użytkowników i JWT.
> - Baza relacyjna PostgreSQL z kolumnami JSONB tam, gdzie potrzeba elastyczności.
> - Podejście *Database-First* (stworzymy SQL schema + seed, potem `dotnet ef dbcontext scaffold` aby wygenerować modele EF Core).
> - API REST z kontrolerami CRUD.

---

# 1. ERD — opis logiczny (tabele i relacje)

Tabele główne:

1. **exercise** — definicja ćwiczenia
   - id (PK) : uuid
   - name : text
   - description : text
   - category : text
   - exercise_type : text (enum-like: Strength/Time/Distance/Mixed/Custom)
   - additional_settings : jsonb (opcjonalne)
   - created_at, updated_at

2. **tag**
   - id (PK) : uuid
   - name : text (unikalne)

3. **exercise_tag** (many-to-many)
   - exercise_id (FK -> exercise.id)
   - tag_id (FK -> tag.id)
   - PRIMARY KEY (exercise_id, tag_id)

4. **exercise_link**
   - id (PK) : uuid
   - exercise_id (FK -> exercise.id)
   - url : text
   - description : text (opcjonalne)

5. **workout_template**
   - id (PK) : uuid
   - name : text
   - description : text
   - category : text
   - created_at, updated_at

6. **template_exercise**
   - id (PK) : uuid
   - workout_template_id (FK -> workout_template.id)
   - exercise_id (FK -> exercise.id)
   - order_index : integer

7. **template_set**
   - id (PK) : uuid
   - template_exercise_id (FK -> template_exercise.id)
   - set_number : integer
   - metrics : jsonb  -- { reps, weight, time_seconds, distance_m, rest_seconds, tempo }

8. **workout_session**
   - id (PK) : uuid
   - workout_template_id (FK -> workout_template.id, nullable)
   - session_date : timestamptz
   - notes : text
   - is_completed : boolean
   - created_at, updated_at

9. **session_exercise**
   - id (PK) : uuid
   - workout_session_id (FK -> workout_session.id)
   - exercise_id (FK -> exercise.id)
   - order_index : integer

10. **workout_set**
   - id (PK) : uuid
   - session_exercise_id (FK -> session_exercise.id)
   - set_number : integer
   - completed : boolean
   - metrics : jsonb -- { reps, weight, time_seconds, distance_m, rest_seconds, tempo }
   - notes : text (opcjonalne)


---

# 2. SQL: CREATE TABLE (PostgreSQL)

Poniżej pełne przykładowe definicje SQL gotowe do uruchomienia w PostgreSQL 14+.

```sql
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE exercise (
  id uuid PRIMARY KEY DEFAULT uuid_generate_v4(),
  name text NOT NULL,
  description text,
  category text,
  exercise_type text NOT NULL,
  additional_settings jsonb,
  created_at timestamptz NOT NULL DEFAULT now(),
  updated_at timestamptz NOT NULL DEFAULT now()
);

CREATE TABLE tag (
  id uuid PRIMARY KEY DEFAULT uuid_generate_v4(),
  name text NOT NULL UNIQUE
);

CREATE TABLE exercise_tag (
  exercise_id uuid NOT NULL REFERENCES exercise(id) ON DELETE CASCADE,
  tag_id uuid NOT NULL REFERENCES tag(id) ON DELETE CASCADE,
  PRIMARY KEY (exercise_id, tag_id)
);

CREATE TABLE exercise_link (
  id uuid PRIMARY KEY DEFAULT uuid_generate_v4(),
  exercise_id uuid NOT NULL REFERENCES exercise(id) ON DELETE CASCADE,
  url text NOT NULL,
  description text
);

CREATE TABLE workout_template (
  id uuid PRIMARY KEY DEFAULT uuid_generate_v4(),
  name text NOT NULL,
  description text,
  category text,
  created_at timestamptz NOT NULL DEFAULT now(),
  updated_at timestamptz NOT NULL DEFAULT now()
);

CREATE TABLE template_exercise (
  id uuid PRIMARY KEY DEFAULT uuid_generate_v4(),
  workout_template_id uuid NOT NULL REFERENCES workout_template(id) ON DELETE CASCADE,
  exercise_id uuid NOT NULL REFERENCES exercise(id) ON DELETE RESTRICT,
  order_index integer NOT NULL DEFAULT 0
);

CREATE TABLE template_set (
  id uuid PRIMARY KEY DEFAULT uuid_generate_v4(),
  template_exercise_id uuid NOT NULL REFERENCES template_exercise(id) ON DELETE CASCADE,
  set_number integer NOT NULL,
  metrics jsonb NOT NULL DEFAULT '{}'
);

CREATE TABLE workout_session (
  id uuid PRIMARY KEY DEFAULT uuid_generate_v4(),
  workout_template_id uuid REFERENCES workout_template(id) ON DELETE SET NULL,
  session_date timestamptz NOT NULL,
  notes text,
  is_completed boolean NOT NULL DEFAULT false,
  created_at timestamptz NOT NULL DEFAULT now(),
  updated_at timestamptz NOT NULL DEFAULT now()
);

CREATE TABLE session_exercise (
  id uuid PRIMARY KEY DEFAULT uuid_generate_v4(),
  workout_session_id uuid NOT NULL REFERENCES workout_session(id) ON DELETE CASCADE,
  exercise_id uuid NOT NULL REFERENCES exercise(id) ON DELETE RESTRICT,
  order_index integer NOT NULL DEFAULT 0
);

CREATE TABLE workout_set (
  id uuid PRIMARY KEY DEFAULT uuid_generate_v4(),
  session_exercise_id uuid NOT NULL REFERENCES session_exercise(id) ON DELETE CASCADE,
  set_number integer NOT NULL,
  completed boolean NOT NULL DEFAULT false,
  metrics jsonb NOT NULL DEFAULT '{}',
  notes text
);

-- Indeksy pomocnicze
CREATE INDEX idx_workout_session_date ON workout_session(session_date);
CREATE INDEX idx_exercise_type ON exercise(exercise_type);
```

> **Uwaga:** `metrics` JSONB powinno trzymać klucze:
> - reps (integer)
> - weight (numeric)
> - time_seconds (integer)
> - distance_m (numeric)
> - rest_seconds (integer)
> - tempo (text, np. "3-1-2")

---

# 3. Code-First: workflow (instrukcja dla Copilot / deva)

1. W projekcie .NET: zainstaluj pakiety:
   - Microsoft.EntityFrameworkCore
   - Npgsql.EntityFrameworkCore.PostgreSQL
   - Microsoft.EntityFrameworkCore.Design
   - Microsoft.EntityFrameworkCore.Tools
2. Utwórz modele EF Core (patrz sekcja 4).
3. Skonfiguruj `AppDbContext` z odpowiednimi konfigracjami (wartości domyślne, konwersje JSONB).
4. Uruchom migracje:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

5. Dodaj przykładowe seed data w metodzie `OnModelCreating` lub osobnej klasie Seeder.

---

# 4. Modele C# (przykład — po scaffold można dostosować)

Poniżej przykładowe klasy DTO/Model w stylu EF Core (przykładowe, proste):

```csharp
using System;
using System.Collections.Generic;

public class Exercise {
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public string ExerciseType { get; set; }
    public string AdditionalSettings { get; set; } // raw JSON or use JsonDocument
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public ICollection<ExerciseTag> ExerciseTags { get; set; }
    public ICollection<ExerciseLink> Links { get; set; }
}

public class Tag {
    public Guid Id { get; set; }
    public string Name { get; set; }
    public ICollection<ExerciseTag> ExerciseTags { get; set; }
}

public class ExerciseTag {
    public Guid ExerciseId { get; set; }
    public Exercise Exercise { get; set; }
    public Guid TagId { get; set; }
    public Tag Tag { get; set; }
}

public class ExerciseLink {
    public Guid Id { get; set; }
    public Guid ExerciseId { get; set; }
    public string Url { get; set; }
    public string Description { get; set; }
}

public class WorkoutTemplate {
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public ICollection<TemplateExercise> TemplateExercises { get; set; }
}

public class TemplateExercise {
    public Guid Id { get; set; }
    public Guid WorkoutTemplateId { get; set; }
    public WorkoutTemplate WorkoutTemplate { get; set; }
    public Guid ExerciseId { get; set; }
    public Exercise Exercise { get; set; }
    public int OrderIndex { get; set; }
    public ICollection<TemplateSet> TemplateSets { get; set; }
}

public class TemplateSet {
    public Guid Id { get; set; }
    public Guid TemplateExerciseId { get; set; }
    public int SetNumber { get; set; }
    public string Metrics { get; set; } // JSON
}

public class WorkoutSession {
    public Guid Id { get; set; }
    public Guid? WorkoutTemplateId { get; set; }
    public WorkoutTemplate WorkoutTemplate { get; set; }
    public DateTimeOffset SessionDate { get; set; }
    public string Notes { get; set; }
    public bool IsCompleted { get; set; }
    public ICollection<SessionExercise> SessionExercises { get; set; }
}

public class SessionExercise {
    public Guid Id { get; set; }
    public Guid WorkoutSessionId { get; set; }
    public WorkoutSession WorkoutSession { get; set; }
    public Guid ExerciseId { get; set; }
    public Exercise Exercise { get; set; }
    public int OrderIndex { get; set; }
    public ICollection<WorkoutSet> WorkoutSets { get; set; }
}

public class WorkoutSet {
    public Guid Id { get; set; }
    public Guid SessionExerciseId { get; set; }
    public int SetNumber { get; set; }
    public bool Completed { get; set; }
    public string Metrics { get; set; } // JSON
    public string Notes { get; set; }
}
```

> **Wskazówka**: dla JSON przechowuj jako `JsonDocument` lub `Dictionary<string, object>` z konwerterem EF Core `ValueConverter` albo użyj `NpgsqlTypes.NpgsqlJsonb`.

---

# 5. API — endpointy i DTO

Proponowane route'y (wszystkie pod `/api`):

## Exercise
- `GET /api/exercises` — lista (filtrowanie po tagach, category, name)
- `GET /api/exercises/{id}` — szczegóły (wraz z links, tags)
- `POST /api/exercises` — tworzenie (body zawiera name, description, category, exerciseType, tags[], links[])
- `PUT /api/exercises/{id}` — aktualizacja
- `DELETE /api/exercises/{id}` — usunięcie

## Tag
- `GET /api/tags`
- `POST /api/tags`

## WorkoutTemplate
- `GET /api/templates`
- `GET /api/templates/{id}`
- `POST /api/templates` — body zawiera templateExercises (exerciseId, orderIndex, templateSets)
- `PUT /api/templates/{id}`
- `DELETE /api/templates/{id}`

## WorkoutSession
- `GET /api/sessions` — listowanie sesji (data range)
- `GET /api/sessions/{id}`
- `POST /api/sessions` — tworzy sesję (może przyjąć `templateId` aby skopiować struktury)
- `PUT /api/sessions/{id}` — modyfikacja notes/is_completed
- `DELETE /api/sessions/{id}`

## SessionExercise / WorkoutSet (część sesji)
- `POST /api/sessions/{sessionId}/exercises` — dodać exercise do sesji (z opcjonalnymi kopiami templateSets)
- `PUT /api/sessions/{sessionId}/exercises/{exerciseId}` — aktualizacja order
- `POST /api/sessions/{sessionId}/exercises/{sessionExerciseId}/sets` — dodać set (body -> metrics JSON)
- `PUT /api/sessions/{sessionId}/exercises/{sessionExerciseId}/sets/{setId}` — aktualizować set (np. completed + metrics)
- `DELETE /api/sessions/{sessionId}/exercises/{sessionExerciseId}/sets/{setId}`

---

# 6. Przykładowe DTO (JSON)

**CreateTemplate**
```json
{
  "name": "Push/Pull",
  "description": "Upper body focus",
  "category": "upper",
  "exercises": [
    {
      "exerciseId": "...",
      "orderIndex": 0,
      "sets": [
        {"setNumber": 1, "metrics": {"reps":10, "weight":30, "rest_seconds":90, "tempo":"3-1-2"}},
        {"setNumber": 2, "metrics": {"reps":8, "weight":35}}
      ]
    }
  ]
}
```

**CreateSession (kopiowanie template)**
```json
{
  "workoutTemplateId": "...",
  "sessionDate": "2025-11-30T18:00:00Z",
  "notes": "Dzień nóg"
}
```

**AddSet**
```json
{
  "setNumber": 1,
  "metrics": { "reps": 10, "weight": 30, "rest_seconds": 90 }
}
```

---

# 7. Seed data — przykłady

```sql
-- przykładowy tag i exercise
INSERT INTO tag (id, name) VALUES (uuid_generate_v4(), 'push');
INSERT INTO exercise (id, name, description, category, exercise_type) VALUES (uuid_generate_v4(), 'Push Press', 'Overhead push press', 'shoulders', 'Strength');
```

---

# 8. Frontend (Blazor) — krótka propozycja struktury

- Projekt Blazor WebAssembly (lub Server):
  - Strony: `Templates`, `Sessions`, `Exercises`, `Dashboard`.
  - Komponenty: `ExerciseCard`, `TemplateEditor`, `SessionEditor`, `SetEditor`.
  - HttpClient skonfigurowany do `/api`.
  - Store lokalny (np. prosty service/in-memory) do trzymania bieżącej sesji.
  - Obsługa offline: możliwość buforowania sesji lokalnie (później sync).

UX flow:
- Użytkownik tworzy / edytuje szablon (TemplateEditor)
- Użytkownik z listy wybiera szablon i tworzy sesję (SessionEditor kopiujący template)
- W trakcie sesji klika konkretne ćwiczenie — rozwija się SetEditor, wypełnia serie
- Po skończonej sesji oznacza `IsCompleted`

---

# 9. Testy i walidacja

- Walidacja DTO (FluentValidation lub DataAnnotations)
- Testy integracyjne dla kontrolerów (in-memory DB lub testcontainer + PostgreSQL)

---

# 10. Gotowy SLA / Service-Driven Prompt dla Copilot

Poniżej kompletny prompt, który możesz wkleić do Copilota (jedno polecenie). Zawiera kroki, wymagania techniczne i co finalnie chcesz uzyskać.

```
GOAL: Wygeneruj kompletny backend .NET 8 (WebAPI) + migracje EF Core dla modułu "Workout/Exercise" zgodnie z poniższymi wymaganiami. Projekt ma być code-first: najpierw stwórz modele EF Core, następnie wygeneruj migracje. Na końcu przygotuj kontrolery REST.

TECH STACK:
- Backend: .NET 8 (C#), WebAPI
- ORM: EF Core + Npgsql (Code-First z migracjami)
- DB: PostgreSQL (z użyciem jsonb dla metryk)

REQUIREMENTS (data model & behaviour):
1. Use Code-First approach with EF Core migrations instead of raw SQL.
2. Exercise supports: name, description, category, exercise_type (Strength/Time/Distance/Mixed/Custom), tags (separate table), external links (url + description) and additional_settings jsonb.
3. WorkoutTemplate: list of template_exercises (with order) and template_sets (predefined sets with metrics: reps, weight, time_seconds, distance_m, rest_seconds, tempo).
4. WorkoutSession can be created from a template — this should copy structure (template_exercises + template_sets) into session_exercises + workout_sets so user can mark sets completed and adjust metrics.
5. workout_set.metrics is jsonb and may contain: reps, weight, time_seconds, distance_m, rest_seconds, tempo.
6. Single-user initial scope (no authentication). Keep code ready to add auth later.
7. Provide OpenAPI/Swagger for the API.

DELIVERABLES (repo layout & key files):
- Docker setup with PostgreSQL
- Backend project with:
  - `Models/` (EF Core models with proper configurations)
  - `Data/AppDbContext` configured for Npgsql and JSONB handling
  - `Controllers/` with controllers for Exercises, Templates, Sessions
  - DTOs and mapping (AutoMapper or manual mapping)
  - `Program.cs` with swagger, DB connection string configuration (use environment variables)
  - EF Core migrations for database schema

TESTING & QA:
- Add minimal integration tests for API endpoints with in-memory or testcontainer DB.

NOTES:
- Use Code-First: create models first, then generate migrations with `dotnet ef migrations add InitialCreate`
- Provide example HTTP requests (curl) for creating a template, creating a session from template, adding sets, marking set completed.

END GOAL: a working application where I can run DB (via Docker), apply migrations, run backend, and perform full CRUD for exercises, templates and sessions.
```

---

# 11. Dalsze uwagi / rozszerzenia (opcjonalne przyszłe funkcje)

- multi-user i auth (JWT, Identity)
- sync offline / local-first
- eksport CSV/JSON z progresu
- prosty analytics (e.g. wykresy progresu dla danego exercise)
- integracja z wearable (import tętna/dystansu)

---

# 12. Co dalej

Jeżeli chcesz, mogę teraz:
- wygenerować *konkretny* projekt początkowy (pliki `Program.cs`, przykładowy kontroler, przykładowy Blazor page) — generuję kod w kolejnym kroku,
- albo wprowadzić zmiany w modelu (np. dodać users/roles),
- albo przygotować gotowy `docker-compose.yml` (Postgres + Backend + Frontend).

Napisz, którą z opcji chcesz teraz zrealizować.
