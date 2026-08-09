# Schema lifecycle

Identity, versioning, access, and the cost of the data you store.

## Identity lives in the GUID

A schema is registered in the memory of the running Revit instance and shared by every open document.
Saving a document saves the schemas of the entities it holds, and opening that document reintroduces them into memory.

Two schemas conflict when they share a GUID but differ in any of:

- the number of fields, or any field definition;
- the schema name;
- the vendor id or the application GUID;
- the read or write access level.

The conflict surfaces when a document written by one definition meets a session holding another — on open, and on synchronize in a workshared model.
Copying a sample and keeping its GUID, or editing fields without minting a new GUID, is what produces it.

## Evolving a shipped schema

A published schema is immutable.
Adding a field means a new GUID and a new definition class; the previous one stays in the code base as long as documents in the wild still carry it.

```csharp
public static class ProjectDataConfigurationV1
{
    public static readonly Guid Identity = new("0E73AF93-E7F3-42E6-9BA5-AFC1CA23D42B");
    // fields as they shipped
}
```

The context reads the old schema when the new one holds nothing, and writes only the new one.

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
In a workshared model such a handler borrows elements behind the user's back.
Erase the old schema from a document only once its data has been carried over.

## Access levels

Read and write levels are set independently, each one `Public`, `Vendor`, or `Application`.
`Vendor` requires `SetVendorId`, `Application` requires `SetApplicationGUID`, and `Finish` rejects a restricted level whose identifier is missing.

A vendor id is 4 to 253 characters of letters, digits, and a small set of punctuation, matched case-insensitively.
`SchemaBuilder.VendorIdIsValid(id)` checks one before use.

Public read with vendor write is the usual choice: any add-in may read the data, only yours may change it.

Write access is verified when the entity is stored, not when a field is set.
The failure appears at `SetEntity`:

> Writing of Entities of this Schema is not allowed to the current add-in.

It means the running add-in's vendor id does not match the schema's — commonly a Design Automation bundle registered under a different id.
`schema.ReadAccessGranted()` and `schema.WriteAccessGranted()` answer the same question before the write.

## Worksharing and element lifetime

An entity synchronizes like a parameter value: editing it borrows its host element, and other users see the change after reload.

Splitting an element leaves the entity on both halves; copying an element copies its entities.

## Size and reach

Every add-in in the session draws on the same budget for one file.

Split data into fields, arrays, and maps.
One large serialized string slows save, open, and synchronize.
Many `ElementId` values in a single entity are the expensive case.

Extensible Storage does not travel to the Autodesk viewer.
The SVF conversion pipeline loads no add-ins and reads no schema.
