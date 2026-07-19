---
name: revit-ribbon
description: >
  Build an Autodesk Revit ribbon with the Nice3point.Revit.Extensions fluent panel and button API instead of raw RibbonPanel and PushButtonData calls.
  USE FOR: creating ribbon panels, push/pulldown/split buttons, stacked rows, icons, tooltips, shortcuts, and availability controllers in an application's OnStartup.
  DO NOT USE FOR: right-click context menu entries (use revit-context-menu).
license: MIT
---

# Revit Ribbon

The `Nice3point.Revit.Extensions` ribbon API wraps the verbose, stringly-typed raw API (`RibbonPanel.AddItem(new PushButtonData(...))`, manual `BitmapImage` URIs, manual tab creation) into a fluent, generic, theme-aware chain.
`CreatePanel` creates the tab if needed and returns an existing panel by name; calling it repeatedly is safe.

## When to use

- Building an add-in's ribbon in `ExternalApplication.OnStartup`.
- Reviewing ribbon code that constructs `PushButtonData` and `BitmapImage` by hand.

## When not to use

- Adding right-click menu entries — use `revit-context-menu`.

## Workflow

### Step 1: Create a panel

```csharp
var panel = application.CreatePanel("Analysis", "MyAddin");
```

`application` is the `UIControlledApplication` from `OnStartup`.
The overload without a tab name adds the panel to the add-ins tab.

### Step 2: Add buttons bound to command classes

```csharp
panel.AddPushButton<AnalyzeCommand>("Analyze")
    .SetImage("/MyAddin;component/Resources/Icons/Analyze16.png")
    .SetLargeImage("/MyAddin;component/Resources/Icons/Analyze32.png");
```

`AddPushButton<TCommand>` wires the command by type.
Group related commands under `AddPullDownButton("…")` or `AddSplitButton("…")` and add their children with the pulldown's own `AddPushButton<TCommand>`; `AddRadioButtonGroup`, `AddComboBox`, and `AddTextBox` add the other input controls.

```csharp
var pulldown = panel.AddPullDownButton("Tools");
pulldown.AddPushButton<AnalyzeCommand>("Analyze");
pulldown.AddPushButton<ExportCommand>("Export");
```

### Step 3: Stack small items vertically

`AddStackPanel` packs one to three small items into a vertical stack (overflow flows into a new column).
It carries its own `AddPushButton`/`AddPullDownButton`/`AddSplitButton`/`AddComboBox`/`AddTextBox`, plus `AddLabel` for a caption.

```csharp
var stack = panel.AddStackPanel();
stack.AddPushButton<AnalyzeCommand>("Analyze");
stack.AddLabel("Mode:");
stack.AddComboBox();
```

On the stack panel the pulldown and split overloads take `(buttonText, internalName)` — the reverse of `RibbonPanel.AddPullDownButton(internalName, buttonText)`.

### Step 4: Set icons, tooltips, availability, and shortcuts

`SetImage`/`SetLargeImage` take a pack URI; when the file name contains `light`/`dark` the icon auto-swaps for the Revit theme (2024+).
Add `SetToolTip`, `SetLongDescription` (which accepts `<p>` paragraphs), and `SetAvailabilityController<T>` to grey the button out when its command cannot run.
Bind a keyboard shortcut with `AddShortcuts` (or `TryAddShortcuts`, which skips on conflict).

```csharp
panel.AddPushButton<AnalyzeCommand>("Analyze")
    .SetToolTip("Analyze the active model")
    .SetLongDescription("<p>Runs the analysis and writes a report.</p>")
    .SetAvailabilityController<DocumentAvailableController>()
    .AddShortcuts("RA");
```

### Step 5: Tint the panel (optional)

Tint the panel body, its title bar, or its slide-out area with `SetBackground`, `SetTitleBarBackground`, and `SetSlideOutPanelBackground` — each takes a color name, a `Color`, or a WPF `Brush`.

### Step 6: Verify

Launch Revit and confirm the panel and buttons appear on the tab, icons render in both light and dark themes, and each button invokes its command.

## Validation

- [ ] Panels come from `CreatePanel`, not hand-built tabs.
- [ ] Buttons use `AddPushButton<TCommand>`, not raw `PushButtonData`.
- [ ] Icons are set with `SetImage`/`SetLargeImage`.
- [ ] The ribbon is built in `OnStartup`.

## Common Pitfalls

| Pitfall                                           | Correct approach                                             |
|---------------------------------------------------|--------------------------------------------------------------|
| `panel.AddItem(new PushButtonData(...))`          | `panel.AddPushButton<TCommand>("…")`.                        |
| `new BitmapImage(new Uri("pack://…"))` per button | `.SetLargeImage("/Addin;component/…png")`.                   |
| Two panels of the same name created twice         | `CreatePanel` returns the existing panel; call it directly.  |
| `CreatePanel`/`AddPushButton` not found           | The `Nice3point.Revit.Extensions` package is not referenced. |
