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


    public static INativeVoteApi? nativeVoteApi;

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
        try
        {
            GetAPI();
        }
        catch (Exception e)
        {
        }

        if (nativeVoteApi == null)
        {
            Server.PrintToConsole("NativeVote API is not available.");
            return;
        }
        
        nativeVoteApi.OnVotePass += info =>
        {
            Server.PrintToChatAll($"{info!.VoteInfo.voteIdentifier}");
            Server.PrintToChatAll("Vote passed!!!");
        };
        
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
        
        var potentialClients = Utilities.GetPlayers().Where(p => p is { IsBot: false, IsHLTV: false }).ToList();
        var potentialClientsIndex = potentialClients.Select(p => p.Index).ToList();
        NativeVoteInfo nInfo = new NativeVoteInfo("TEST_VOTE!", "#SFUI_vote_passed_nextlevel_extend" ,"Details STR", potentialClientsIndex, VoteThresholdType.AbsoluteValue, 0.5F, 5.0F);

        nativeVoteApi.InitiateVote(nInfo);
    }
}