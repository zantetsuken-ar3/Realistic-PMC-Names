# Realistic PMC Names

A lightweight server mod for SPT that replaces default PMC-style usernames with large pools of believable, faction-appropriate human identities.

## Features
- 2,000 unique U\*\*\* identities
- 2,000 unique B\*\*\* identities
- Roughly one in four identities includes a callsign or nickname
- Multinational US/UK/Canadian/Australian/NZ/European/NATO-style U\*\*\* pool
- Predominantly Russian B\*\*\* pool with wider former-Soviet and regional influence
- No changes to AI, spawning, equipment, progression, health, difficulty or PMC levels
- Human-editable JSON name pools
- Server mod only; no client mod required

> "Before Realistic PMC Names, I didn't even exist. Now I'm Giulio 'Duke' Leone. Frankly, that's an improvement."
>
> — Giulio "Duke" Leone, U\*\*\*

## Compatibility
Designed for the SPT 4.1.x branch. Version 1.0.0 was developed and tested on SPT 4.1.5. The mod metadata declares `~4.1.0` compatibility.

## Installation
Release users simply extract the release ZIP into the root of their SPT installation. The DLL should end up at `user/mods/RealisticPMCNames/RealisticPMCNames.dll`.

## Configuration
The supplied pools are `config/u_names.json` and `config/b_names.json`. They can be edited with a normal text editor. Restart the SPT server after changes.

Avoid enabling another mod that replaces the same U\*\*\*/B\*\*\* name pools after this mod loads.

## Building from source
The project targets .NET 10 and, by default, references SPT assemblies from `C:\SPT\SPT_Runtime`. Override the MSBuild `SptPath` property if your development installation is elsewhere.

Build with `dotnet build -c Release`.

## License
MIT
