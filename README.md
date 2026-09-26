# Realistic PMC Names

A lightweight server mod for SPT that replaces default PMC-style usernames with large pools of believable, faction-appropriate human identities — and gives the Flea Market its own recurring population of buyers and sellers.

## Features

- 4,000 unique U-side PMC identities
- 4,000 unique B-side PMC identities
- 8,000 PMC identities in total
- Carefully selected, unique callsigns mixed naturally into both PMC populations
- Multinational US/UK/Canadian/Australian/NZ/European/NATO-style U-side pool
- Predominantly Russian B-side pool with wider former-Soviet and regional influence
- 300 separate Flea Market identities used by simulated buyers and sellers
- Flea identities are deliberately kept separate from the PMC population
- No changes to AI, spawning, equipment, progression, health, difficulty or PMC levels
- Human-editable JSON name pools
- Server mod only; no client mod required

## "It's just a name. So what?"

A surprisingly large amount, as it turns out.

A difficult PMC encounter feels different when the dogtag reveals that the person you just fought was Daniel "Bob" Mercer rather than another disposable gamer-style username. If you ever encounter that callsign again, you know you've seen that identity before.

Realistic PMC Names does not add biographies, scripted characters or a persistence system. It simply gives encounters an identity and lets the stories create themselves.

The same idea now extends to the Flea Market. Its smaller, separate population means names can recur naturally: Scrapper might buy something from you today and be selling something tomorrow. The marketplace begins to feel like a community rather than a stream of random usernames.

> "Before Realistic PMC Names, I didn't even exist. Now I'm Giulio 'Duke' Leone. Frankly, that's an improvement."
>
> — Giulio "Duke" Leone, U-side PMC

## What's new in v1.1.0

- Expanded the U-side roster from 2,000 to 4,000 identities
- Expanded the B-side roster from 2,000 to 4,000 identities
- Reworked and expanded callsigns, with callsigns unique within each side
- Added a dedicated pool of 300 Flea Market identities
- Flea identities are used for simulated sellers and buyers
- Flea identities remain separate from the PMC name pools
- Added configuration for the dedicated Flea population

## Compatibility

Designed for the SPT 4.1.x branch.

Version 1.1.0 was developed and tested on SPT 4.1.5. The mod metadata declares `~4.1.0` compatibility.

Mods that also replace the same PMC first-name pools may conflict with Realistic PMC Names or overwrite its identities depending on load order.

## Installation

Extract the release ZIP into the root of your SPT installation.

The DLL should end up at:

`SPT_Runtime/user/mods/RealisticPMCNames/RealisticPMCNames.dll`

The installed mod folder should contain:

```text
RealisticPMCNames/
├── RealisticPMCNames.dll
├── RealisticPMCNames.deps.json
└── config/
    ├── config.json
    ├── u_names.json
    ├── b_names.json
    └── flea_names.json
```

Restart the SPT server after installing or changing configuration/name files.

## Configuration

`config/config.json` contains:

```json
{
  "useFleaVendorNames": true
}
```

With `useFleaVendorNames` set to `true` (default), simulated Flea Market buyers and sellers use the dedicated 300-name Flea population.

Set it to `false` if you prefer the Flea Market to draw from the mod's realistic PMC identity pools instead.

There is deliberately no option to restore the original gamer-style PMC usernames: replacing those is the purpose of the mod.

### Name pools

- `config/u_names.json` — 4,000 U-side PMC identities
- `config/b_names.json` — 4,000 B-side PMC identities
- `config/flea_names.json` — 300 Flea Market identities

All pools are human-editable JSON. Keep valid JSON formatting if you customise them.

## Building from source

The project targets .NET 10 and, by default, references SPT assemblies from:

`C:\SPT\SPT_Runtime`

Override the MSBuild `SptPath` property if your development installation is elsewhere.

Build with:

```powershell
dotnet build -c Release
```

The `bin` and `obj` directories are build output and do not need to be included in the source repository.

## License

MIT
