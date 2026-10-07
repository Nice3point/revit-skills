# Schema lifecycle

Identity, versioning, access, and the cost of the stored data.

## Identity is the GUID

A schema is registered in the memory of the running Revit instance and shared by every open document.
Saving a document saves the schemas of the entities it contains, and opening that document registers them in memory again.

Two schemas conflict when they share a GUID but differ in any of:

- the number of fields, or any field definition;
- the schema name;
- the vendor id or the application GUID;
- the read or write access level.

The conflict occurs when a session that has registered one definition opens a document written with another, and on synchronize in a workshared model.
Copying a sample without changing its GUID, or editing fields without creating a new GUID, causes the conflict.

## Evolving a shipped schema

A published schema is immutable.
Adding a field requires a new GUID and a new definition class.
The previous class remains in the code base while existing documents still contain its schema.

```csharp
public static class ProjectDataConfigurationV1
{
    public static readonly Guid Identity = new("0E73AF93-E7F3-42E6-9BA5-AFC1CA23D42B");
    // fields as they shipped
}
```

The context reads the old schema when the new one contains no data, and writes only the new one.

```csharp
public ProjectData Load()
{
    var storage = FindStorage();
    if (storage is null) return new ProjectData();

    var current = storage.LoadEntity<string>(_schema, ProjectDataConfiguration.Number);
    if (current is not null) return ReadCurrent(storage);

    return ReadLegacy(storage);
}
```

Migrate on an explicit user action or on first write, never inside a `DocumentOpened` or `DocumentSaved` handler.
In a workshared model such a handler borrows elements without a user action.
Erase the old schema from a document only after its data has been migrated.

## Access levels

Read and write levels are set independently, each one `Public`, `Vendor`, or `Application`.
`Vendor` requires `SetVendorId`, `Application` requires `SetApplicationGUID`, and `Finish` rejects a restricted level whose identifier is missing.

A vendor id is 4 to 253 characters of letters, digits, and a small set of punctuation, matched case-insensitively.
`SchemaBuilder.VendorIdIsValid(id)` checks one before use.

Public read with vendor write is the usual choice: any add-in may read the data, and only an add-in of the schema's vendor may change it.

Write access is verified when the entity is stored, not when a field is set.
The failure appears at `SetEntity`:

> Writing of Entities of this Schema is not allowed to the current add-in.

It means the running add-in's vendor id does not match the schema's — commonly a Design Automation bundle registered under a different id.
`schema.ReadAccessGranted()` and `schema.WriteAccessGranted()` check the same access before the write.

## Worksharing and element lifetime

An entity synchronizes like a parameter value: editing it borrows its host element, and other users see the change after reload.

Splitting an element copies the entity to both parts, and copying an element copies its entities.

## Size and scope

Every add-in in the session shares the same storage budget for one file.

Split data into fields, arrays, and maps.
One large serialized string slows save, open, and synchronize.
Many `ElementId` values in a single entity are the expensive case.

The Autodesk viewer does not display Extensible Storage data.
The SVF conversion pipeline loads no add-ins and reads no schema.
