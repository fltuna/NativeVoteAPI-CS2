using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using NativeVoteAPI;
using NativeVoteAPI.API;

namespace NativeVoteAPITest;

public class NativeVoteApiTest: BasePlugin
{
    public override string ModuleName => "NativeVoteAPI test";
    public override string ModuleVersion => "0.0.1";

    // Declare the nativeVoteApi variable like this
    private static INativeVoteApi? _nativeVoteApi;
    
    public override void Load(bool hotReload)
    {
    }

    // You should initialize plugin capability on OnAllPluginsLoaded 
    public override void OnAllPluginsLoaded(bool hotReload)
    {
        // When NativeVoteAPI is not loaded, it will throw the exception ->
        try
        {
            _nativeVoteApi = INativeVoteApi.Capability.Get();
        }
        catch (Exception e)
        {
            // Ignored because plugin will stop loading.
        }

        // -> And plugin should stop loading or implement some feature to block the NativeVoteAPI from using.
        if (_nativeVoteApi == null)
        {
            Server.PrintToConsole("NativeVote API is not available.");
            return;
        }
        
        // To register OnVotePass listener
        _nativeVoteApi.OnVotePass += info =>
        {
            Server.PrintToChatAll($"{info!.VoteInfo.voteIdentifier}");
            Server.PrintToChatAll("Vote passed!!!");
        };
        
        // To register OnVoteFail listener
        _nativeVoteApi.OnVoteFail += info =>
        {
            Server.PrintToChatAll($"{info!.VoteInfo.voteIdentifier}");
            Server.PrintToChatAll("Vote failed!!!");
        };
        
        // To register OnVoteCancel listener
        _nativeVoteApi.OnVoteCancel += info =>
        {
            Server.PrintToChatAll($"{info!.VoteInfo.voteIdentifier}");
            Server.PrintToChatAll("Vote cancelled!!!");
        };
        
        this.AddCommand("css_tsvote", "Initiate test vote", CmdInitiateVote);
        this.AddCommand("css_tcvote", "Cancel test vote", CmdCancelVote);
    }

    public override void Unload(bool hotReload)
    {
        this.RemoveCommand("css_tsvote", CmdInitiateVote);
        this.RemoveCommand("css_tcvote", CmdCancelVote);
    }


    private void CmdInitiateVote(CCSPlayerController? client, CommandInfo info)
    {
        if (_nativeVoteApi == null)
            return;
        
        if(client == null)
            return;
        
        // Requires to creating a NativeVoteInfo
        var potentialClients = Utilities.GetPlayers().Where(p => p is { IsBot: false, IsHLTV: false }).ToList();
        var potentialClientsIndex = potentialClients.Select(p => p.Index).ToList();
        
        
        // You can only use builtin SFUI_vote texts in display string.
        // for instance, you can use these strings:
        // #SFUI_vote_passed_nextlevel_extend -> can be use the details string (custom string)
        // #SFUI_Vote_None -> blank vote
        //
        // Or you can use custom file for fully customizable text. See README.md
        //
        string displayString = "#SFUI_Vote_None";
        string detailsString = "";
        
        // You can set vote identifier to check your vote in OnVotePass, OnVoteFail, OnVoteCancel.
        string voteIdentifier = "TEST_VOTE!";
        
        // arguments information is provided in code document, see NativeVoteInfo.cs
        NativeVoteInfo nInfo = new NativeVoteInfo(voteIdentifier, displayString ,detailsString, potentialClientsIndex, VoteThresholdType.AbsoluteValue, 0.5F, 5.0F);

        // When vote successfully initiated, it will return InitializeAccepted
        NativeVoteState state = _nativeVoteApi.InitiateVote(nInfo);


        switch (state)
        {
            case NativeVoteState.InitializeAccepted:
                client.PrintToChat("Starting vote.");
                break;
            
            default:
                client.PrintToChat("Vote is already in progress!");
                break;
        }
    }


    private void CmdCancelVote(CCSPlayerController? client, CommandInfo info)
    {
        if (_nativeVoteApi == null)
            return;
        
        if(client == null)
            return;
        
        // When vote successfully to initiating cancel, it will return Cancelling
        NativeVoteState state = _nativeVoteApi.CancelVote();
        
        
        switch (state)
        {
            case NativeVoteState.Cancelling:
                client.PrintToChat("Cancelling the vote.");
                break;
            
            case NativeVoteState.NoActiveVote:
                client.PrintToChat("There is no active vote!");
                break;
                
            default:
                client.PrintToChat("Something went wrong!");
                break;
        }
    }
}