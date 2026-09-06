---
name: revit-extensible-storage
description: >
  Persist add-in data inside the Revit document with Extensible Storage, laid out as a data class, a schema definition, and a typed context per stored subject.
  USE FOR: storing custom data on a document or on an element, defining or evolving a schema, choosing where the data lives, and finding the elements that carry it.
  DO NOT USE FOR: reading or writing values the user sees as element parameters (use revit-element-and-parameter-access).
license: MIT
---

# Revit Extensible Storage

Extensible Storage keeps add-in data inside the `.rvt` file.
A `Schema` describes the shape, an `Entity` holds the values, and any `Element` carries one entity per schema.
The layout below mirrors EF Core: a schema definition plays the part of an entity configuration, a context plays the part of a `DbContext`, and a plain class carries the values.

`Nice3point.Revit.Extensions` wraps the entity read and write pair as `SaveEntity`/`LoadEntity` on `Element`, and the schema filter as `WithExtensibleStorage` on the collector.

## When to use

- Storing add-in data that has no parameter equivalent, per document or per element.
- Adding a field to a schema that already shipped, or reading data written by an earlier version.
- Finding the elements that carry the add-in data.

## When not to use

- The user must see or edit the value in the Revit UI — model it as a project or shared parameter.
- The payload runs past a few kB per element or a few MB per file — keep it in an external database and store only the key.

## Workflow

### Step 1: Lay out the subject

The data class, the schema definition, and the context change in one edit.

```text
ProjectData/
  ProjectData.cs // the values
  ProjectDataConfiguration.cs // the schema definition
  ProjectDataContext.cs // access to the stored values
```

| EF Core                       | Extensible Storage                             |
|-------------------------------|------------------------------------------------|
| `IEntityTypeConfiguration<T>` | `*Configuration` class driving `SchemaBuilder` |
| `DbContext`                   | `*Context` class owning the schema and carrier |
| entity type                   | plain data class                               |
| `SaveChanges()`               | `Transaction.Commit()`                         |
| `DbSet<T>` query              | `collector.WithExtensibleStorage(guid)`        |
| migration                     | a new schema GUID                              |

### Step 2: Describe the data as a plain class

```csharp
namespace RevitAddIn.ProjectData;

public sealed class ProjectData
{
    public string Number { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public double GrossArea { get; set; }
}
```

### Step 3: Build the schema once per session

`Schema.Lookup` returns the schema already registered in the session, and `Finish` throws once that identity exists.

```csharp
using Autodesk.Revit.DB.ExtensibleStorage;

namespace RevitAddIn.ProjectData;

public static class ProjectDataConfiguration
{
    public const string Number = "Number";
    public const string Name = "Name";
    public const string GrossArea = "GrossArea";

    public static readonly Guid Identity = new("0E73AF93-E7F3-42E6-9BA5-AFC1CA23D42B");

    public static Schema Create()
    {
        var schema = Schema.Lookup(Identity);
        if (schema is not null) return schema;

        var builder = new SchemaBuilder(Identity);
        builder.SetSchemaName("AcmeProjectData");
        builder.SetDocumentation("Project data owned by the Acme add-in");
        builder.SetVendorId("ACME");
        builder.SetReadAccessLevel(AccessLevel.Public);
        builder.SetWriteAccessLevel(AccessLevel.Vendor);

        builder.AddSimpleField(Number, typeof(string));
        builder.AddSimpleField(Name, typeof(string));
        builder.AddSimpleField(GrossArea, typeof(double)).SetSpec(SpecTypeId.Area);

        return builder.Finish();
    }
}
```

A `double`, `float`, `XYZ`, or `UV` field declares its spec with `SetSpec`, and every read and write of it passes a compatible unit.
Schema and field names must read as C++ identifiers — ASCII letters, digits after the first character, and underscore.
Build the schema when the data is first touched, not during add-in startup.
Registering a schema at startup slows document open and save.

### Step 4: Wrap the carrier in a context

A document-wide record belongs on a `DataStorage` element, an invisible element that exists to hold entities.
One `DataStorage` element carries one subject.
In a workshared model, editing a subject borrows only the storage element of that subject.
Data that describes a single element belongs on that element instead — take it as a constructor argument and skip the lookup.

Reads need no transaction, writes do.
The caller owns the transaction; the context opens none of its own.

```csharp
using Autodesk.Revit.DB.ExtensibleStorage;

namespace RevitAddIn.ProjectData;

public sealed class ProjectDataContext
{
    private const string StorageName = "Acme Project Data";

    private readonly Document _document;
    private readonly Schema _schema;

    public ProjectDataContext(Document document)
    {
        _document = document;
        _schema = ProjectDataConfiguration.Create();
    }

    public ProjectData Load()
    {
        var storage = FindStorage();
        if (storage is null) return new ProjectData();

        return new ProjectData
        {
            Number = storage.LoadEntity<string>(_schema, ProjectDataConfiguration.Number) ?? string.Empty,
            Name = storage.LoadEntity<string>(_schema, ProjectDataConfiguration.Name) ?? string.Empty,
            GrossArea = storage.LoadEntity<double>(_schema, ProjectDataConfiguration.GrossArea, UnitTypeId.SquareMeters)
        };
    }

    public void Save(ProjectData data)
    {
        var storage = FindStorage();
        if (storage is null)
        {
            storage = DataStorage.Create(_document);
            storage.Name = StorageName;
        }

        storage.SaveEntity(_schema, data.Number, ProjectDataConfiguration.Number);
        storage.SaveEntity(_schema, data.Name, ProjectDataConfiguration.Name);
        storage.SaveEntity(_schema, data.GrossArea, ProjectDataConfiguration.GrossArea, UnitTypeId.SquareMeters);
    }

    private DataStorage? FindStorage()
    {
        return (DataStorage?) _document.CollectElements()
            .OfClass<DataStorage>()
            .WithExtensibleStorage(_schema.GUID)
            .FirstOrDefault();
    }
}
```

`SaveEntity` returns `false` when the schema has no such field, and `LoadEntity` returns the default when the field is missing or nothing was written yet.

The context is bound to a `Document`.
Create one per document, and keep it out of a static field and out of a singleton registration.

### Step 5: Drive it from the caller

```csharp
using var transaction = new Transaction(document, "Save project data");
transaction.Start();

var context = new ProjectDataContext(document);
context.Save(data);

transaction.Commit();
```

### Step 6: Query the elements that carry the data

`WithExtensibleStorage` applies an `ExtensibleStorageFilter`, a quick filter that rejects elements before they expand into memory.

```csharp
var annotated = document.CollectElements()
    .WithExtensibleStorage(ProjectDataConfiguration.Identity)
    .ToElements();
```

### Step 7: Remove the data when the feature is uninstalled

```csharp
element.DeleteEntity(schema); // returns false when the element held nothing
document.EraseSchemaAndAllEntities(schema); // every entity in the document
```

Both need an open transaction and write access to the schema.
The erased schema stays registered in memory for the rest of the session.

### Step 8: Verify the round trip

Save, close the document, reopen it, and read the values back.
Data that survives only inside one session was never written to the file.

## Validation

- [ ] The data class, the schema definition, and the context of one subject sit together.
- [ ] The schema is looked up before it is built, and the build happens on first use.
- [ ] Every `double`, `float`, `XYZ`, and `UV` field declares a spec, and every read and write of one passes a compatible unit.
- [ ] Writes run inside a transaction the caller owns; the context adds no transaction of its own.
- [ ] The context is created per document and held by no static field.
- [ ] Elements carrying data are found with `WithExtensibleStorage`, not by loading everything and probing `GetEntity`.
- [ ] Values read back after a document reopen match what was written.

## Common Pitfalls

| Pitfall                                                        | Correct approach                                                                  |
|----------------------------------------------------------------|-----------------------------------------------------------------------------------|
| Adding or renaming a field in a schema that already shipped    | Mint a new GUID; read the old schema, write the new one.                          |
| `new SchemaBuilder(guid)` without `Schema.Lookup` first        | `Finish` throws once the identity is registered in the session.                   |
| A `double` field without `SetSpec`                             | Declare the spec, then pass a compatible unit on every read and write.            |
| Mutating the entity from `GetEntity` and expecting it to stick | `GetEntity` hands back a copy; `SetEntity` stores it. `SaveEntity` does both.     |
| Treating the result of `GetEntity` as null when absent         | An absent entity comes back invalid, not null — check `entity.IsValid()`.         |
| Wrapping `Transaction` in a custom unit of work                | The caller opens the transaction; the context only reads and writes.              |
| Dumping one large JSON string into a single field              | Split it into separate fields, arrays, and maps.                                  |
| Loading every element to probe `GetEntity`                     | `document.CollectElements().WithExtensibleStorage(guid)`.                         |
| `SaveEntity`/`WithExtensibleStorage` not found                 | The `Nice3point.Revit.Extensions` package is not referenced.                      |

## References

- [references/schema-fields.md](references/schema-fields.md) — **Load when:** the schema needs a numeric, array, map, or nested-entity field, or a value comes back with the wrong type or unit.
- [references/schema-lifecycle.md](references/schema-lifecycle.md) — **Load when:** evolving a shipped schema, resolving a GUID conflict, choosing an access level, or storing data in a workshared model.
