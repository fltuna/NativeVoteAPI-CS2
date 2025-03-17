using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.UserMessages;
using CounterStrikeSharp.API.Modules.Utils;
using NativeVoteAPI;
using NativeVoteAPI.API;
using Timer = CounterStrikeSharp.API.Modules.Timers.Timer;

namespace NativeVote;

class VoteManager(NativeVoteApi plugin)
{
    NativeVoteApi _plugin = plugin;
    
    private CVoteController? VoteController => Utilities.FindAllEntitiesByDesignerName<CVoteController>("vote_controller").Last();

    private YesNoVoteInfo? _currentVote = null;
    
    private NativeVoteState _currentVoteState = NativeVoteState.NoActiveVote;
    
    private bool _voteInProgress = false;
    
    Timer? endVoteTimer;
    
    
    public void Load()
    {
        _plugin.RegisterEventHandler<EventVoteCast>(OnVoteCast);
        
    }

    public void Unload()
    {
        _plugin.DeregisterEventHandler<EventVoteCast>(OnVoteCast);
    }

    private HookResult OnVoteCast(EventVoteCast @event, GameEventInfo info)
    {
        Server.PrintToChatAll("VoteCast");
        if (!_voteInProgress)
            return HookResult.Continue;

        if (VoteController == null)
            return HookResult.Continue;

        CCSPlayerController? client = @event.Userid;
        
        if (client == null)
            return HookResult.Continue;

        if (_currentVote == null)
            return HookResult.Continue;

        VoteOption voteOption = (VoteOption)@event.VoteOption;

        if (voteOption == VoteOption.VOTE_OPTION1)
        {
            ++_currentVote.YesVotes;
            _currentVote.ClientVoteInfo[client.Index] = VoteType.Yes;
        }
        else if (voteOption == VoteOption.VOTE_OPTION2)
        {
            ++_currentVote.NoVotes;
            _currentVote.ClientVoteInfo[client.Index] = VoteType.No;
        }
        
        RefreshVotes();
        
        if (_currentVote.TotalVotes >= _currentVote.VoteInfo.PotentialClients.Count)
        {
            EndVote();
        }

        if (_currentVote == null)
        {
            Server.PrintToChatAll("Current vote null");
        }
        
        _plugin.InvokePlayerCastVoteEvent(client, voteOption, _currentVote);
        
        return HookResult.Continue;
    }
    

    private void SendVoteStartUm(RecipientFilter filter)
    {
        if(_currentVote == null)
            return;
        
        UserMessage um = UserMessage.FromPartialName("VoteStart");
        um.SetInt("team", _currentVote.VoteInfo.TargetTeam);
        // Set this to 99 to make looks execute from server
        um.SetInt("player_slot", _currentVote.VoteInfo.VoteInitiator);
        um.SetInt("vote_type", -1);
            
        // We can only use in-game vote related texts since valve fixed a XSS exploit.
        // #SFUI_vote_passed_nextlevel_extend
        um.SetString("disp_str", _currentVote.VoteInfo.DisplayString);
        um.SetString("details_str", _currentVote.VoteInfo.DetailsString);
        um.SetString("other_team_str", "#SFUI_otherteam_vote_unimplemented");
        um.SetBool("is_yes_no_vote", true);
            
        um.Send(filter);
    }

    private void SendVoteFailUmAll()
    {
        foreach (CCSPlayerController cl in Utilities.GetPlayers())
        {
            if(cl.IsBot || cl.IsHLTV)
                continue;
            
            SendVoteFailUm(cl);
        }
    }
    
    private void SendVoteFailUm(CCSPlayerController client)
    {
        RecipientFilter filter = new RecipientFilter();
        filter.Add(client);
        UserMessage um = UserMessage.FromPartialName("VoteFailed");
        um.SetInt("reason", 0);
        
        um.Send(filter);
    }

    
    private void SendVotePassUmAll()
    {
        foreach (CCSPlayerController cl in Utilities.GetPlayers())
        {
            if(cl.IsBot || cl.IsHLTV)
                continue;
            
            SendVotePassUm(cl);
        }
    }
    
    private void SendVotePassUm(CCSPlayerController client)
    {
        RecipientFilter filter = new RecipientFilter();
        filter.Add(client);
        UserMessage um = UserMessage.FromPartialName("VotePass");
        um.SetInt("team", -1);
        um.SetInt("vote_type", 2);
        um.SetString("disp_str", "#SFUI_vote_passed");
        um.SetString("details_str", "");
        
        um.Send(filter);
    }

    public NativeVoteState InitiateVote(NativeVoteInfo vote)
    {
        if(_voteInProgress || _currentVoteState != NativeVoteState.NoActiveVote)
            return _currentVoteState;

        if (VoteController == null)
            return _currentVoteState;
        
        _voteInProgress = true;
        _currentVoteState = NativeVoteState.Initializing;
        

        VoteController.PotentialVotes = vote.PotentialClients.Count;
        //VoteController.IsYesNoVote = true;
        VoteController.ActiveIssueIndex = 2;
        //VoteController.OnlyTeamToVote = -1;

        for (int i = VoteController.VoteOptionCount.Length-1; i >= 0; i--)
        {
            VoteController.VoteOptionCount[i] = 0;
        }
        
        

        _currentVote = new YesNoVoteInfo(vote);

        RefreshVotes();
        _plugin.AddTimer(0.2F, () =>
        {
            var filter = new RecipientFilter();
            foreach (CCSPlayerController cl in Utilities.GetPlayers())
            {
                if(cl.IsBot || cl.IsHLTV)
                    continue;
            
                filter.Add(cl);
            }
            
            SendVoteStartUm(filter);

            endVoteTimer = _plugin.AddTimer(_currentVote.VoteInfo.VoteDuration, EndVote);
        });

        _currentVoteState = NativeVoteState.Voting;
        return NativeVoteState.InitializeAccepted;
    }

    public NativeVoteState CancelVote()
    {
        if (!_voteInProgress || _currentVoteState == NativeVoteState.NoActiveVote)
            return _currentVoteState;

        endVoteTimer?.Kill();
        _currentVoteState = NativeVoteState.Cancelling;
        
        SendVoteFailUmAll();
        _plugin.InvokeVoteCancelEvent(_currentVote);
        DelayedVoteFinishUpdate();

        return NativeVoteState.Cancelling;
    }

    public NativeVoteState GetCurrentVoteState()
    {
        return _currentVoteState;
    }

    public YesNoVoteInfo? GetCurrentVoteInfo()
    {
        return _currentVote;
    }
    
    private void EndVote()
    {
        if(!_voteInProgress)
            return;

        endVoteTimer?.Kill();
        _currentVoteState = NativeVoteState.Finalizing;
        
        if (_currentVote == null)
        {
            SendVoteFailUmAll();
            _plugin.InvokeVoteFailEvent();
            DelayedVoteFinishUpdate();
            return;
        }
        
        float threthold = _currentVote.VoteInfo.VoteThreshold;
        VoteThresholdType type = _currentVote.VoteInfo.ThresholdType;

        int totalYesVotes = _currentVote.YesVotes;
        int totalVotes = _currentVote.TotalVotes;
        
        bool isVotePassed = false;

        if (totalVotes == 0)
        {
            SendVoteFailUmAll();
            _plugin.InvokeVoteFailEvent(_currentVote);
            DelayedVoteFinishUpdate();
            return;
        }
        
        
        if (type == VoteThresholdType.Percentage)
        {
            isVotePassed = (float)totalYesVotes / totalVotes >= threthold;
        }
        else
        {
            isVotePassed = totalYesVotes >= threthold;
        }

        if (isVotePassed)
        {
            SendVotePassUmAll();
            _plugin.InvokeVotePassEvent(_currentVote);
        }
        else
        {
            SendVoteFailUmAll();
            _plugin.InvokeVoteFailEvent(_currentVote);
        }

        DelayedVoteFinishUpdate();
    }
    
    private void RefreshVotes()
    {
        if(_currentVote == null)
            return;
        
        if (VoteController == null)
            return;
        
        var @event = new EventVoteChanged(true)
        {
            VoteOption1 = VoteController.VoteOptionCount[0],
            VoteOption2 = VoteController.VoteOptionCount[1],
            VoteOption3 = VoteController.VoteOptionCount[2],
            VoteOption4 = VoteController.VoteOptionCount[3],
            VoteOption5 = VoteController.VoteOptionCount[4],
            Potentialvotes = _currentVote.VoteInfo.PotentialClients.Count,
        };
        
        @event.FireEvent(false);
    }

    /// <summary>
    /// When we started new vote before fully closing vote UI, the vote UI will not appear.
    /// so we make a some delay here.
    /// </summary>
    private void DelayedVoteFinishUpdate()
    {
        _plugin.AddTimer(5.0F, Finish);
    }

    private void Finish()
    {
        if(VoteController == null)
            return;
        
        
        for (int i = 0; i < 5; i++)
        {
            VoteController.VoteOptionCount[i] = 0;
        }
        
        foreach (CCSPlayerController client in Utilities.GetPlayers())
        {
            VoteController.VotesCast[client.Slot] = (int)VoteOption.VOTE_UNCAST;
        }
        
        _currentVoteState = NativeVoteState.NoActiveVote;
        _voteInProgress = false;
        _currentVote = null;
    }
}