// Adapted from https://github.com/0ceal0t/Dalamud-VFXEditor/blob/main/VFXEditor/Interop/ResourceLoader.Vfx.cs
// ac4aab8
using System;
using Dalamud.Hooking;

namespace VoicedNaelLines.Interop;

public unsafe partial class ResourceLoader
{
    public delegate IntPtr ActorVfxRemoveDelegate(IntPtr vfx, char a2);

    public ActorVfxRemoveDelegate ActorVfxRemove;
    public Hook<ActorVfxRemoveDelegate> ActorVfxRemoveHook { get; private set; }

    private IntPtr ActorVfxRemoveDetour(IntPtr vfx, char a2)
    {
        this.vfxSpawn.InteropRemoved(vfx);
        return ActorVfxRemoveHook.Original(vfx, a2);
    }
}
