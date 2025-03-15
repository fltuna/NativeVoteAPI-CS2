namespace NativeVoteAPI.API;

public enum NativeVoteState
{
    NoActiveVote = 0,
    Cancelling,
    InitializeAccepted,
    Initializing,
    Voting,
    Finalizing,
}