using CounterStrikeSharp.API.Core.Capabilities;

namespace NativeVoteAPI.API;

public interface INativeVoteApi
{
    public static readonly PluginCapability<INativeVoteApi> Capability = new("nativevote:api");
    
    public event Action<YesNoVoteInfo?> OnVoteFail;
    public event Action<YesNoVoteInfo?> OnVotePass;
    
    /// <summary>
    /// Initiates a vote with the given IVote object.
    /// </summary>
    /// <param name="vote"> IVote object</param>
    /// <returns>Returns a NativeVoteState to describe the current vote state, if NativeVoteState is a InitializeAccepted, it means Vote will be initiated.</returns>
    public NativeVoteState InitiateVote(NativeVoteInfo vote);
    
    /// <summary>
    /// Cancel the current vote.
    /// </summary>
    /// <returns>Returns a NativeVoteState to describe the current vote state, if NativeVoteState is a Cancelling, it means vote will be cancelled.</returns>
    public NativeVoteState CancelVote();
    
    /// <summary>
    /// For get current vote state.
    /// </summary>
    /// <returns>Returns a NativeVoteState to describe the current vote state.</returns>
    public NativeVoteState GetCurrentVoteState();
    
    /// <summary>
    /// For get current vote.
    /// </summary>
    /// <returns>current vote information if vote is in progress. otherwise returns null</returns>
    public YesNoVoteInfo? GetCurrentVote();
}