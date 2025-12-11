using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin.Services;
using System;
using System.Collections.Generic;
using VoicedNaelLines.Interop.Structs;

namespace VoicedNaelLines.Interop;

public unsafe class VfxSpawn : IDisposable
{
    private record struct QueueItem(string path, IGameObject gameObject);

    private GameFunctions gameFunctions;
    private IFramework framework;
    private readonly List<ActorVfx> spawnedVfxes = [];
    private readonly List<QueueItem> vfxSpawnQueue = [];
    private bool despawnVfxes = false;

    public VfxSpawn(GameFunctions gameFunctions, IFramework framework)
    {
        this.gameFunctions = gameFunctions;
        this.framework = framework;

        framework.Update += OnFrameworkUpdate;
    }

    public void Dispose()
    {
        DespawnAllVfxes();
        OnFrameworkUpdate(framework);
        framework.Update -= OnFrameworkUpdate;
    }

    private void OnFrameworkUpdate(IFramework framework)
    {

        foreach (var queueItem in vfxSpawnQueue)
        {
            if (queueItem.gameObject.IsValid())
            {
                var vfx = new ActorVfx(gameFunctions, queueItem.path);
                vfx.Create(queueItem.gameObject.Address, queueItem.gameObject.Address);
                spawnedVfxes.Add(vfx);
            }
        }
        vfxSpawnQueue.Clear();
        if (despawnVfxes)
        {
            DespawnAllVfxesFramework();
        }
    }

    public void DespawnAllVfxes() {
        despawnVfxes = true;
    }

    private void DespawnAllVfxesFramework()
    {
        foreach(var vfx in spawnedVfxes)
        {
            vfx.Remove();
        }
        spawnedVfxes.Clear();
        despawnVfxes = false;
    }

    public void QueueActorVfx(string path, IGameObject target)
    {
        vfxSpawnQueue.Add(new QueueItem(path, target));
    }

    public void InteropRemoved(IntPtr data)
    {
        if (!GetVfx(data, out var vfx)) { return; }

        spawnedVfxes.Remove((ActorVfx)vfx);

        if (vfx is ActorVfx actorVfx)
        {
            actorVfx.Vfx = null;
        }
    }

    public bool GetVfx(nint data, out BaseVfx vfx)
    {
        vfx = null!;
        if (data == IntPtr.Zero || spawnedVfxes.Count == 0) { return false; }

        vfx = spawnedVfxes.Find((vfx) => vfx.GetVfxPointer() == data)!;
        return vfx != null;

    }
}
