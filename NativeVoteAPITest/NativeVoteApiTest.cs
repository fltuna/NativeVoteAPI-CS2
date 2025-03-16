using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Capabilities;
using CounterStrikeSharp.API.Modules.Commands;
using NativeVoteAPI;
using NativeVoteAPI.API;

namespace NativeVoteAPITest;

public class NativeVoteApiTest: BasePlugin
{
    public override string ModuleName => "NativeVoteAPI test";
    public override string ModuleVersion => "0.0.1";

    // Declare the nativeVoteApi variable like this
    public static INativeVoteApi? nativeVoteApi;

    // This is optional, but useful property.
    private INativeVoteApi GetAPI()
    {
        if (nativeVoteApi == null)
            nativeVoteApi = INativeVoteApi.Capability.Get();

        return nativeVoteApi;
    }
    
    public override void Load(bool hotReload)
    {
    }

    public override void OnAllPluginsLoaded(bool hotReload)
    {
        // When NativeVoteAPI is not loaded, it will throw the exception ->
        try
        {
            GetAPI();
        }
        catch (Exception e)
        {
        }

        // -> And plugin should stop loading or implement some feature to block the NativeVoteAPI from using.
        if (nativeVoteApi == null)
        {
            Server.PrintToConsole("NativeVote API is not available.");
            return;
        }
        
        // To register OnVotePass listener
        nativeVoteApi.OnVotePass += info =>
        {
            Server.PrintToChatAll($"{info!.VoteInfo.voteIdentifier}");
            Server.PrintToChatAll("Vote passed!!!");
        };
        
        // To register OnVoteFail listener
        nativeVoteApi.OnVoteFail += info =>
        {
            Server.PrintToChatAll($"{info!.VoteInfo.voteIdentifier}");
            Server.PrintToChatAll("Vote failed!!!");
        };
        
        this.AddCommand("css_tsvote", "Initiate test vote", CmdInitiateVote);
    }

    public override void Unload(bool hotReload)
    {
        this.RemoveCommand("css_tsvote", CmdInitiateVote);
    }


    private void CmdInitiateVote(CCSPlayerController? client, CommandInfo info)
    {
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
        // Another arguments information is provided in code document, see NativeVoteInfo.cs
        //
        NativeVoteInfo nInfo = new NativeVoteInfo("TEST_VOTE!", "#SFUI_Vote_loadbackup" ,"Details STR あああああああああ", potentialClientsIndex, VoteThresholdType.AbsoluteValue, 0.5F, 5.0F);

        nativeVoteApi.InitiateVote(nInfo);
    }
}