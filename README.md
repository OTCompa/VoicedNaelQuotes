# Voiced Nael Quotes
Voiced Nael Quotes aims to add voice acting to Nael quotes in The Unending Coil of Bahamut, a raid in FINAL FANTASY XIV.

## Main Points
- Adds voice acting to Nael quotes 
- Use different voice packs to your liking (currently only tik-tok and teto. LF VAs!)
- Directional in-world audio with fall-off

## How It Works
The directional audio utilizes a custom VFX (no visuals) with a sound attached to simulate in-world audio. File replacements are done
through a process based on how Penumbra swaps files on the fly.   

## How To Use
This plugin uses the sound effect volume in your game client. Please ensure sound effects are audible in-game.  
Languages other than English are not currently supported but is planned.  
A separate option to support a different in-game audio source is also planned (Possibly won't be directional?).
### Getting Started
- Type `/xlsettings` in the chatbox or open up Dalamud's settings menu
- Open the "Experimental" tab and scroll down to the "Custom Plugin Repositories" section

`https://raw.githubusercontent.com/OTCompa/frey-s-dalamud-plugins/refs/heads/main/plogon.json`
- Paste the link above into the bottom-most textbox of the section and click the "+" button to the right
- Click on the save button on the bottom right corner of the window
- Type `/xlplugins` in the chatbox or open up Dalamud's plugin installer menu
- Search for `Voiced Nael Quotes` and install.

### Building
1. Open up `VoicedNaelQuotes.sln` in your C# editor of choice (likely [Visual Studio 2022](https://visualstudio.microsoft.com) or [JetBrains Rider](https://www.jetbrains.com/rider/)).
2. Build the solution. By default, this will build a `Debug` build, but you can switch to `Release` in your IDE.
3. The resulting plugin can be found at `VoicedNaelQuotes/bin/x64/Debug/VoicedNaelQuotes.dll` (or `Release` if appropriate.)

## Credits
Voiced Nael Quotes heavily uses code from different projects and would not be possible if these did not exist.
Huge thanks to:
- [RaidsRewritten](https://github.com/Ricimon/FFXIV-RaidsRewritten)
- [VfxEditor](https://github.com/0ceal0t/Dalamud-VFXEditor)
- [Penumbra](https://github.com/xivdev/Penumbra)
- [BigNaelQuotes](https://github.com/hunter2actual/BigNaelQuotes)