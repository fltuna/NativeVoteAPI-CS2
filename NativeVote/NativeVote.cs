using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Capabilities;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.UserMessages;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.Logging;
using NativeVoteAPI;
using NativeVoteAPI.API;

namespace NativeVote;

public class NativeVoteApi: BasePlugin, INativeVoteApi
{
    public override string ModuleName => "NativeVoteAPI";
    public override string ModuleVersion => "0.2.3";
    public override string ModuleAuthor => "tuna";

    public static INativeVoteApi ApiInstance = null!;

    private VoteManager? _voteManager;
    
    public override void Load(bool hotReload)
    {
        _voteManager = new VoteManager(this);
        _voteManager.Load();

        ApiInstance = this;

        if (!hotReload)
        {
            Capabilities.RegisterPluginCapability(INativeVoteApi.Capability, () => ApiInstance);
        }
        else
        {
            Logger.LogWarning("NativeVote API is not supported for hot-reload! Some problem(s) may occur.");
        }
        Logger.LogInformation("NativeVote API loaded");
    }

    public override void Unload(bool hotReload)
    {
        _voteManager?.Unload();
        Logger.LogInformation("NativeVote API unloaded");
    }

    public void InvokeVoteFailEvent(YesNoVoteInfo? voteInfo = null)
    {
        OnVoteFail?.Invoke(voteInfo);
    }
    
    public void InvokeVotePassEvent(YesNoVoteInfo? voteInfo = null)
    {
        OnVotePass?.Invoke(voteInfo);
    }

    public void InvokeVoteCancelEvent(YesNoVoteInfo? voteInfo = null)
    {
        OnVoteCancel?.Invoke(voteInfo);
    }

    public void InvokePlayerCastVoteEvent(CCSPlayerController client, VoteOption voteOption, YesNoVoteInfo? voteInfo = null)
    {
        OnPlayerCastVote?.Invoke(voteInfo, client, voteOption);
    }
    
    
    public event Action<YesNoVoteInfo?>? OnVoteFail;
    public event Action<YesNoVoteInfo?>? OnVotePass;
    public event Action<YesNoVoteInfo?>? OnVoteCancel;
    public event Action<YesNoVoteInfo?, CCSPlayerController, VoteOption>? OnPlayerCastVote;

    public NativeVoteState InitiateVote(NativeVoteInfo vote)
    {
        if (_voteManager == null)
            throw new InvalidOperationException("Failed to initialize the voteManager!");
        
        return _voteManager.InitiateVote(vote);
    }

    public NativeVoteState CancelVote()
    {
        if (_voteManager == null)
            throw new InvalidOperationException("Failed to initialize the voteManager!");
        
        return _voteManager.CancelVote();
    }

    public NativeVoteState GetCurrentVoteState()
    {
        if (_voteManager == null)
            throw new InvalidOperationException("Failed to initialize the voteManager!");

        return _voteManager.GetCurrentVoteState();
    }

    public YesNoVoteInfo? GetCurrentVote()
    {
        if (_voteManager == null)
            throw new InvalidOperationException("Failed to initialize the voteManager!");
        
        
        return _voteManager.GetCurrentVoteInfo();
    }
}