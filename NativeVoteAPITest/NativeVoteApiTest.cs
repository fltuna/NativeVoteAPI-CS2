using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using NativeVoteAPI;
using NativeVoteAPI.API;

namespace NativeVoteAPITest;

public class NativeVoteApiTest: BasePlugin
{
    public override string ModuleName => "NativeVoteAPI test";
    public override string ModuleVersion => "0.3.0";

    // Declare the nativeVoteApi variable like this
    private static INativeVoteApi? _nativeVoteApi;


    private const string VoteIdentifier = "NativeVoteAPITest";
    
    public override void Load(bool hotReload)
    {
    }

    // You should initialize plugin capability on OnAllPluginsLoaded 
    public override void OnAllPluginsLoaded(bool hotReload)
    {
        // When NativeVoteAPI is not loaded, it will throw the exception -
        try
        {
            _nativeVoteApi = INativeVoteApi.Capability.Get();
        }
        catch (Exception e)
        {}

        // - And plugin should stop loading or implement some feature to block the use of NativeVoteAPI.
        if (_nativeVoteApi == null)
        {
            Server.PrintToConsole("NativeVote API is not available.");
            return;
        }
        
        // To register OnVotePass listener
        _nativeVoteApi.OnVotePass += info =>
        {
            if(info!.VoteInfo.voteIdentifier != VoteIdentifier)
                return;

            Server.PrintToChatAll($"{info!.VoteInfo.voteIdentifier}");
            Server.PrintToChatAll("Vote passed!!!");
        };
        
        // To register OnVoteFail listener
        _nativeVoteApi.OnVoteFail += info =>
        {
            if(info!.VoteInfo.voteIdentifier != VoteIdentifier)
                return;

            Server.PrintToChatAll($"{info!.VoteInfo.voteIdentifier}");
            Server.PrintToChatAll("Vote failed!!!");
        };
        
        // To register OnVoteCancel listener
        _nativeVoteApi.OnVoteCancel += info =>
        {
            if(info!.VoteInfo.voteIdentifier != VoteIdentifier)
                return;

            Server.PrintToChatAll($"{info!.VoteInfo.voteIdentifier}");
            Server.PrintToChatAll("Vote cancelled!!!");
        };

        // To register OnPlayerCastVote listener
        _nativeVoteApi.OnPlayerCastVote += (info, player, voteOption) =>
        {
            if(info!.VoteInfo.voteIdentifier != VoteIdentifier)
                return;

            Server.PrintToChatAll($"identifier: {info?.VoteInfo.voteIdentifier}, voteOption: {voteOption}, player: {player.PlayerName}");
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
        
        
        // You can only use built-in SFUI_vote texts in the display string.
        // For instance, you can use these strings:
        // #SFUI_vote_passed_nextlevel_extend -> Allows embedding a details string (custom string).
        // #SFUI_Vote_None -> Blank vote
        //
        // Or you can use custom file for fully customizable text. See README.md
        //
        
        // Dirty hack for custom vote string
        string devider = "----------------------------------------------------------------------------------";
        string devider2 = "||||||||||||||||||||||||||||||||||||||||||||";
        
        string displayString = "#SFUI_vote_passed_nextlevel_extend";
        string detailsString = $"{devider2} {devider} The text we wanted to show {devider} {devider2}";

        if (info.ArgCount >= 3)
        {
            if (info.ArgByIndex(1) == "c")
            {
                displayString = "#SFUI_vote_custom_vote_default";
                detailsString = info.ArgByIndex(2);
            }
            else if (info.ArgByIndex(1) == "i")
            {
                displayString = "#SFUI_vote_custom_vote_image";
                detailsString = info.ArgByIndex(2);
            }
        }
        
        // You can set vote identifier to check your vote in OnVotePass, OnVoteFail, OnVoteCancel.
        string voteIdentifier = "TestVoteIdentifier";
        
        // Arguments information is provided in code document, see NativeVoteInfo.cs
        NativeVoteInfo nInfo = new NativeVoteInfo(voteIdentifier, displayString ,detailsString, potentialClientsIndex, VoteThresholdType.Percentage, 0.5F, 20.0F, initiator: client.Slot);

        // When the vote is successfully initiated, it will return InitializeAccepted.
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
        
        // When the vote is successfully initiated to cancel, it will return Cancelling
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