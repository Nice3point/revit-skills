# Schema fields

Field types, units, containers, and nested entities.
A schema holds at most 256 fields, and a name runs 1 to 247 characters.
`SchemaBuilder.AcceptableName(name)` answers for a name built at runtime.

## Simple fields

`AddSimpleField(name, type)` accepts `bool`, `byte`, `short`, `int`, `long`, `float`, `double`, `string`, `Guid`, `ElementId`, `XYZ`, `UV`, and `Entity`.

```csharp
builder.AddSimpleField("Manufacturer", typeof(string));
builder.AddSimpleField("Verified", typeof(bool));
builder.AddSimpleField("HostId", typeof(ElementId));
```

An `ElementId` field tracks the model: deleting the referenced element resets the stored value to `ElementId.InvalidElementId`, and the value is not carried into elements produced by copy, paste, or an array.
Store a `Guid` — `element.UniqueId` — when the reference must survive those operations.

## Fields with units

`float`, `double`, `XYZ`, and `UV` values convert on the way in and out.
`Finish` rejects a schema whose numeric field has no spec.

```csharp
builder.AddSimpleField("Thickness", typeof(double)).SetSpec(SpecTypeId.Length);
builder.AddSimpleField("Ratio", typeof(double)).SetSpec(SpecTypeId.Number); // unitless
```

```csharp
wall.SaveEntity(schema, 0.5, "Thickness", UnitTypeId.Meters);
wall.SaveEntity(schema, 0.75, "Ratio", UnitTypeId.General);

var thickness = wall.LoadEntity<double>(schema, "Thickness", UnitTypeId.Meters);
```

An incompatible unit fails the call.
`field.GetSpecTypeId()` reports the declared spec, `field.CompatibleUnit(unitTypeId)` tests a unit before use, and `fieldBuilder.NeedsUnits()` tells whether the type demands one at all.

The unit overloads of `SaveEntity` and `LoadEntity` require `Nice3point.Revit.Extensions` built for Revit 2021 or newer.

## Arrays

`AddArrayField(name, valueType)` accepts the same value types as a simple field, and the value travels as `IList<T>`.

```csharp
builder.AddArrayField("Revisions", typeof(string));
```

```csharp
IList<string> revisions = ["A", "B"];
element.SaveEntity(schema, revisions, "Revisions");

var stored = element.LoadEntity<IList<string>>(schema, "Revisions");
```

Declare the local as `IList<T>`.
The generic argument is inferred from the static type, and Revit rejects a `List<T>` as a type mismatch.

## Maps

`AddMapField(name, keyType, valueType)` stores an ordered key-value map, and the value travels as `IDictionary<TKey, TValue>`.

Keys accept `bool`, `byte`, `short`, `int`, `long`, `string`, `Guid`, and `ElementId`.
Floating-point and entity keys are unsupported.
Round-off makes floating-point comparison unstable, and an entity carries no comparison operator.
Values accept everything a simple field accepts.

```csharp
builder.AddMapField("Attributes", typeof(string), typeof(string));
```

```csharp
IDictionary<string, string> attributes = new Dictionary<string, string>
{
    ["vendor"] = "ACME"
};

element.SaveEntity(schema, attributes, "Attributes");

var stored = element.LoadEntity<IDictionary<string, string>>(schema, "Attributes");
```

To key by a measured value, put the values in an array field and key by index.

## Nested entities

A field of type `Entity` holds an entity of another schema, named by `SetSubSchemaGUID`.
Give the nested schema its own definition class, the same as any other schema.

```csharp
builder.AddSimpleField("Supplier", typeof(Entity)).SetSubSchemaGUID(SupplierConfiguration.Identity);
```

```csharp
var supplierSchema = SupplierConfiguration.Create();

var supplier = new Entity(supplierSchema);
supplier.Set(supplierSchema.GetField(SupplierConfiguration.Name), "ACME");

element.SaveEntity(schema, supplier, "Supplier");
```

The nested schema carries its own access levels, and `field.SubEntityReadAccessGranted()` and `field.SubEntityWriteAccessGranted()` report whether the current add-in may reach through.
An invalid entity written into such a field deletes the nested value.

## Inspecting a schema at runtime

- `schema.ListFields()` — every field of the schema;
- `schema.GetField(name)` — one field, or `null`;
- `field.ValueType`, `field.KeyType`, `field.ContainerType` — the declared shape;
- `field.SubSchema`, `field.SubSchemaGUID` — the nested schema;
- `element.GetEntitySchemaGuids()` — every schema that stored data on this element, including other vendors';
- `Schema.ListSchemas()` — every schema registered in the session.

`Schema`, `Field`, and `Entity` expose more of their definition than the members above; reach for whichever one the task needs.
Use them when reading data whose schema another add-in owns, or when writing a diagnostic that dumps what a document carries.
