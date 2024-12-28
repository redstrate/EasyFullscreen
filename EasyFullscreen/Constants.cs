using FFXIVClientStructs.FFXIV.Client.System.Framework;

namespace EasyFullscreen;

public class Constants
{
    // FIXME: For some reason this disappeared from the FFXIVClientStruct signature list...?
    public const string SetWindowStateSignature = "E9 ?? ?? ?? ?? 83 FA ?? 75 ?? 44 8B 4C 24";

    public unsafe delegate void SetWindowModeDelegate(EnvironmentManager* a1, int mode);
}