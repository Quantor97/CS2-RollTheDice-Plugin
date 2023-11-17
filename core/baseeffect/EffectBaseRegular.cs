
using CounterStrikeSharp.API.Core;
using Preach.CS2.Plugins.RollTheDiceV2.Core;
using Preach.CS2.Plugins.RollTheDiceV2.Utilities;

namespace Preach.CS2.Plugins.RollTheDiceV2.Core.BaseEffect;

public abstract class EffectBaseRegular : EffectBase, IEffectRegular
{
    public abstract void OnApply(CCSPlayerController? playerController);

    public virtual void PrintDescription(CCSPlayerController? playerController, string translationKey, params string[] args)
    {
        playerController!.LogChat(GetEffectPrefix() + Description.__(translationKey, args));
    }
}