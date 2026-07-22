---
name: revit-api-option-handlers
description: >
  Supply small Autodesk Revit API callback interfaces with Nice3point.Revit.Toolkit ready-made handlers, not hand-rolled classes.
  USE FOR: passing IFamilyLoadOptions, IDuplicateTypeNamesHandler, or ISaveSharedCoordinatesCallback to a Revit API call.
  DO NOT USE FOR: constraining what a user can pick in an interactive selection, or building a dockable pane's UI element.
license: MIT
---

# Revit API Option Handlers

Several Revit API calls demand a small callback interface.
`Nice3point.Revit.Toolkit` ships ready implementations with a default behavior and lambda or enum customization; you do not hand-roll a class each time.

## When to use

- A Revit API call requires `IFamilyLoadOptions`, `IDuplicateTypeNamesHandler`, or `ISaveSharedCoordinatesCallback`.

## Workflow

### Step 1: Family load options

```csharp
document.LoadFamily(fileName, new FamilyLoadOptions(), out var family); //overwrite existing types, load from the family
document.LoadFamily(fileName, new FamilyLoadOptions(false, FamilySource.Project), out var family); // keep values, load from the project
document.LoadFamily(fileName, UIDocument.GetRevitUIFamilyLoadOptions(), out var family); //reuse Revit's interactive prompt
```

The default constructor overwrites the parameter values of existing types; the `(overwrite, FamilySource)` constructor sets those explicitly, and `UIDocument.GetRevitUIFamilyLoadOptions()` defers to Revit's own dialog.

### Step 2: Duplicate type names handler

```csharp
var options = new CopyPasteOptions();
options.SetDuplicateTypeNamesHandler(new DuplicateTypeNamesHandler()); //default: UseDestinationTypes
options.SetDuplicateTypeNamesHandler(new DuplicateTypeNamesHandler(DuplicateTypeAction.Abort)); //fixed action
options.SetDuplicateTypeNamesHandler(new DuplicateTypeNamesHandler(args => DuplicateTypeAction.Abort)); // decide per call
```

The default constructor keeps the destination types; pass a `DuplicateTypeAction` for a fixed action, or a lambda to decide per call from the `args`.

### Step 3: Save shared coordinates callback

```csharp
linkType.Unload(new SaveSharedCoordinatesCallback()); //default: SaveLinks
linkType.Unload(new SaveSharedCoordinatesCallback(SaveModifiedLinksOptions.DoNotSaveLinks)); //fixed option
linkType.Unload(new SaveSharedCoordinatesCallback(link => //decide per link
{
    if (link.AttachmentType == AttachmentType.Overlay) return SaveModifiedLinksOptions.SaveLinks;
    return SaveModifiedLinksOptions.DoNotSaveLinks;
}));
```

The default constructor saves the links; pass a `SaveModifiedLinksOptions` value for a fixed choice, or a lambda to decide per link.

### Step 4: Verify

Call the API with the handler and confirm the intended behavior applies — the family loads with the chosen source, the duplicate resolves as selected, or the link unloads with the chosen coordinate option.

## Validation

- [ ] The callback uses the Toolkit handler, not a hand-rolled interface implementation.
- [ ] Customization is expressed with the lambda or enum overload, not a new class.
- [ ] The selected default behavior matches the operation's requirement.

## Common Pitfalls

| Pitfall                                                      | Correct approach                                                    |
|--------------------------------------------------------------|---------------------------------------------------------------------|
| Writing a class that implements `IDuplicateTypeNamesHandler` | Use `DuplicateTypeNamesHandler` with a lambda returning the action. |
| `FamilyLoadOptions` not found                                | The `Nice3point.Revit.Toolkit` package is not referenced.           |
