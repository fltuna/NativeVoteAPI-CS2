using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Events;

namespace NativeVote.Events;

[EventName("vote_changed")]
public class EventVoteChangedOld : GameEvent
{
    public EventVoteChangedOld(IntPtr pointer) : base(pointer){}
    public EventVoteChangedOld(bool force) : base("vote_changed", force){}

                

                
    public int VoteOption1
    {
        get => Get<int>("vote_option1");
        set => Set<int>("vote_option1", value);
    }


                
    public int VoteOption2
    {
        get => Get<int>("vote_option2");
        set => Set<int>("vote_option2", value);
    }


                
    public int VoteOption3
    {
        get => Get<int>("vote_option3");
        set => Set<int>("vote_option3", value);
    }


                
    public int VoteOption4
    {
        get => Get<int>("vote_option4");
        set => Set<int>("vote_option4", value);
    }


                
    public int VoteOption5
    {
        get => Get<int>("vote_option5");
        set => Set<int>("vote_option5", value);
    }


                
    public int Potentialvotes
    {
        get => Get<int>("potentialVotes");
        set => Set<int>("potentialVotes", value);
    }
}