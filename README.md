# FlexConfig
[![Nuget](https://img.shields.io/nuget/v/FlexConfig)](https://www.nuget.org/packages/FlexConfig/)

A dynamic configuration library to use with .net projects. Avoid creating oversized configuration objects with dozens or hundreds of properties. Set and get properties dynamically using keys instead! The library uses JSON for storing configurations and can support primitives and user-defined objects alike. Brought to you by [SheepGoMeh](https://github.com/SheepGoMeh) and [kalilistic](https://github.com/kalilistic).

### Example

```csharp

// create config obj
var config = new Configuration("config.json");

// load existing config file at path
config.Load();

// set property
config.Set("isEnabled", false);

// save config file
config.Save();

// retrieve property
var isEnabled = config.Get<bool>("isEnabled");

```
