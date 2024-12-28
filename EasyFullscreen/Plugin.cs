using Dalamud.Game.Config;
using Dalamud.Hooking;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI;

namespace EasyFullscreen;

public sealed class Plugin : IDalamudPlugin
{
    public Plugin()
    {
        Hooking.InitializeFromAttributes(this);
        _setWindowStateHook?.Enable();
        Framework.Update += OnUpdate;
    }

    [PluginService] private static IFramework Framework { get; set; } = null!;

    [PluginService] private static IGameConfig GameConfig { get; set; } = null!;

    [PluginService] private static IGameInteropProvider Hooking { get; set; } = null!;

    public void Dispose()
    {
        _setWindowStateHook?.Dispose();
        Framework.Update -= OnUpdate;
    }

    [Signature(Constants.SetWindowStateSignature, DetourName = nameof(SetWindowMode))]
    private readonly Hook<Constants.SetWindowModeDelegate>? _setWindowStateHook = null!;

    private unsafe void SetWindowMode(EnvironmentManager* a1, int param2)
    {
        _setWindowStateHook?.Original(a1, param2);
    }

    enum ScreenMode
    {
        Windowed = 0,
        BorderlessWindowed = 2
    }

    private unsafe void OnUpdate(IFramework framework)
    {
        if (UIInputData.Instance()->IsKeyPressed(SeVirtualKey.F11))
        {
            var environmentManager =
                FFXIVClientStructs.FFXIV.Client.System.Framework.Framework.Instance()->EnvironmentManager;
            if (GameConfig.System.GetUInt("ScreenMode") == (uint)ScreenMode.BorderlessWindowed)
            {
                _setWindowStateHook?.Original(environmentManager, (int)ScreenMode.Windowed);
                GameConfig.Set(SystemConfigOption.ScreenMode, (uint)ScreenMode.Windowed);
            }
            else
            {
                _setWindowStateHook?.Original(environmentManager, (int)ScreenMode.BorderlessWindowed);
                GameConfig.Set(SystemConfigOption.ScreenMode, (int)ScreenMode.BorderlessWindowed);
            }
        }
    }
}