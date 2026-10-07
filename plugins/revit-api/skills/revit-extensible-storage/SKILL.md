---
name: revit-extensible-storage
description: >
  Persist add-in data inside the Revit document with Extensible Storage, laid out as a data class, a schema definition, and a typed context per stored subject.
  USE FOR: storing custom data on a document or on an element, defining or evolving a schema, choosing the element that stores the data, and finding the elements that contain it.
  DO NOT USE FOR: reading or writing values the user sees as element parameters (use revit-element-and-parameter-access).
license: MIT
---

# Revit Extensible Storage

Extensible Storage stores add-in data in the `.rvt` file.
A `Schema` describes the shape, an `Entity` contains the values, and any `Element` stores one entity per schema.
The layout below mirrors EF Core: a schema definition corresponds to an entity configuration, a context corresponds to a `DbContext`, and a plain class contains the values.

`Nice3point.Revit.Extensions` wraps the entity read and write pair as `SaveEntity`/`LoadEntity` on `Element`, and the schema filter as `WithExtensibleStorage` on the collector.

## When to use

- Storing add-in data that has no parameter equivalent, per document or per element.
- Adding a field to a schema that already shipped, or reading data written by an earlier version.
- Finding the elements that contain the add-in data.

## When not to use

- The user must see or edit the value in the Revit UI — model it as a project or shared parameter.
- The payload runs past a few kB per element or a few MB per file — store it in an external database and save only its key in the document.

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
| `DbContext`                   | `*Context` class managing the schema and store |
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

### Step 4: Wrap the storage element in a context

A document-wide record belongs on a `DataStorage` element, an invisible element designed to store entities.
One `DataStorage` element stores one subject.
In a workshared model, editing a subject borrows only the storage element of that subject.
Data that describes a single element belongs on that element instead — take it as a constructor argument and skip the lookup.

Reads need no transaction, writes do.
The caller opens the transaction, and the context opens none.

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
Create one per document, and never store it in a static field or register it as a singleton.

### Step 5: Drive it from the caller

```csharp
using var transaction = new Transaction(document, "Save project data");
transaction.Start();

var context = new ProjectDataContext(document);
context.Save(data);

transaction.Commit();
```

### Step 6: Query the elements that contain the data

`WithExtensibleStorage` applies an `ExtensibleStorageFilter`, a quick filter that rejects elements before they expand into memory.

```csharp
var annotated = document.CollectElements()
    .WithExtensibleStorage(ProjectDataConfiguration.Identity)
    .ToElements();
```

### Step 7: Remove the data when the feature is uninstalled

```csharp
element.DeleteEntity(schema); // returns false when the element has no entity of the schema
document.EraseSchemaAndAllEntities(schema); // every entity in the document
```

Both need an open transaction and write access to the schema.
The erased schema remains registered in memory for the rest of the session.

### Step 8: Verify the round trip

Save, close the document, reopen it, and read the values back.
Data missing after the reopen was never written to the file.

## Validation

- [ ] The data class, the schema definition, and the context of one subject are located in one folder.
- [ ] The schema is looked up before it is built, and the build happens on first use.
- [ ] Every `double`, `float`, `XYZ`, and `UV` field declares a spec, and every read and write of one passes a compatible unit.
- [ ] Writes run inside a transaction the caller opens, and the context opens no transaction.
- [ ] The context is created per document and stored in no static field.
- [ ] Elements that contain data are found with `WithExtensibleStorage`, not by loading everything and probing `GetEntity`.
- [ ] Values read back after a document reopen match what was written.

## Common Pitfalls

| Pitfall                                                        | Correct approach                                                                  |
|----------------------------------------------------------------|-----------------------------------------------------------------------------------|
| Adding or renaming a field in a schema that already shipped    | Create a new GUID, read the old schema, and write the new one.                    |
| `new SchemaBuilder(guid)` without `Schema.Lookup` first        | `Finish` throws once the identity is registered in the session.                   |
| A `double` field without `SetSpec`                             | Declare the spec, then pass a compatible unit on every read and write.            |
| Mutating the entity from `GetEntity` and expecting it to stick | `GetEntity` returns a copy, and `SetEntity` stores it. `SaveEntity` does both.    |
| Treating the result of `GetEntity` as null when absent         | `GetEntity` returns an invalid entity, not null — check `entity.IsValid()`.       |
| Wrapping `Transaction` in a custom unit of work                | The caller opens the transaction; the context only reads and writes.              |
| Dumping one large JSON string into a single field              | Split it into separate fields, arrays, and maps.                                  |
| Loading every element to probe `GetEntity`                     | `document.CollectElements().WithExtensibleStorage(guid)`.                         |
| `SaveEntity`/`WithExtensibleStorage` not found                 | The `Nice3point.Revit.Extensions` package is not referenced.                      |

## References

- [references/schema-fields.md](references/schema-fields.md) — **Load when:** the schema needs a numeric, array, map, or nested-entity field, or a read returns a value with the wrong type or unit.
- [references/schema-lifecycle.md](references/schema-lifecycle.md) — **Load when:** evolving a shipped schema, resolving a GUID conflict, choosing an access level, or storing data in a workshared model.
