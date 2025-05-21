using BepInEx;
using BepInEx.Logging;

namespace Paulov.UnityBepInExNetworking;

[BepInPlugin("Paulov.UnityBepInExNetworking", "Paulov.UnityBepInExNetworking", "2025.4.30")]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    public static DataProcessing DataProcessing { get; private set; }

    private void Awake()
    {
        Logger = base.Logger;
        gameObject.AddComponent<DataProcessing>();
    }
}
