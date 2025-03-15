using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Capabilities;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.UserMessages;
using CounterStrikeSharp.API.Modules.Utils;
using NativeVoteAPI.API;

namespace NativeVoteAPI;

public class NativeVoteApi: BasePlugin, INativeVoteApi
{
    public override string ModuleName => "NativeVoteAPI";
    public override string ModuleVersion => "0.0.1";
    public override string ModuleAuthor => "tuna";

    public INativeVoteApi ApiInstance = null!;
    private static PluginCapability<INativeVoteApi> ApiCapability { get; } = new("nativevote:api");

    private VoteManager? _voteManager;
    
    public override void Load(bool hotReload)
    {
        _voteManager = new VoteManager(this);
        _voteManager.Load();

        ApiInstance = this;
        Capabilities.RegisterPluginCapability(ApiCapability, () => ApiInstance);
        
    }

    public override void Unload(bool hotReload)
    {
        _voteManager?.Unload();
    }

    public void InvokeVoteFailEvent(YesNoVoteInfo? voteInfo = null)
    {
        OnVoteFail?.Invoke(voteInfo);
    }
    
    public void InvokeVotePassEvent(YesNoVoteInfo? voteInfo = null)
    {
        OnVotePass?.Invoke(voteInfo);
    }
    
    
    public event Action<YesNoVoteInfo?>? OnVoteFail;
    public event Action<YesNoVoteInfo?>? OnVotePass;
    
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