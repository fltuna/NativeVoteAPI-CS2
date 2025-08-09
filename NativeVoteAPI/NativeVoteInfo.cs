using Microsoft.Extensions.Localization;

namespace NativeVoteAPI;

public class NativeVoteInfo
{
    private const int VoteInitiatorServer = 99;
    private const int AllTeam = -1;
    private const float DefaultVoteThresholdPercentage = 0.5F;
    private const float MinimumVoteThresholdAbsolute = 1.0F;
    
    public string voteIdentifier { get; private set; }
    public string DisplayString { get; private set; }
    public string DetailsString { get; private set; }
    public List<uint> PotentialClients { get; private set; }
    public VoteThresholdType ThresholdType { get; private set; }
    public float VoteThreshold { get; private set; }
    public float VoteDuration { get; private set; }
    public int VoteInitiator { get; private set; }
    public int TargetTeam { get; private set; }
    public TranslatableVoteTexts? TranslatableVoteTexts;


    /// <summary>
    /// Create a native vote information.
    /// </summary>
    /// <param name="voteIdentifier">Identifier of this vote. this can be useful when using OnVoteFail and OnVotePass event</param>
    /// <param name="displayString">We can only use in-game vote related texts since valve fixed a XSS exploit.</param>
    /// <param name="detailsString">When SFUI text contains %s1, it will be display this text. also, if stringlocalizer is set, it will use as a transaltion key.</param>
    /// <param name="potentialClients">Client index list of potential voters</param>
    /// <param name="thresholdType">Threshold type of vote win check</param>
    /// <param name="voteThreshold">
    /// When VoteThresholdType is AbsoluteValue, valid range is 1 to count of potentialClients.
    /// or Percentage, valid range is 0.0 to 1.0.
    /// </param>
    /// <param name="voteDuration">Duration of vote in seconds</param>
    /// <param name="initiator">Optional, when you want to specify who started vote, then put client entity index. otherwise vote initiator is Server.</param>
    /// <param name="targetTeam">Optional, If you want to limit the team, then put team id</param>
    /// <param name="stringLocalizer">Optional, if not null, then vote text will use translated text. translation key is obtained from detailsString</param>
    public NativeVoteInfo(
        string voteIdentifier,
        string displayString,
        string detailsString,
        List<uint> potentialClients,
        VoteThresholdType thresholdType,
        float voteThreshold,
        float voteDuration,
        int initiator = VoteInitiatorServer,
        int targetTeam = AllTeam,
        TranslatableVoteTexts? translatableVoteTexts = null)
    {
        this.voteIdentifier = voteIdentifier;
        DisplayString = displayString;
        DetailsString = detailsString;
        PotentialClients = potentialClients;
        ThresholdType = thresholdType;
        VoteDuration = voteDuration;
        VoteInitiator = initiator;
        TargetTeam = targetTeam;
        TranslatableVoteTexts = translatableVoteTexts;
        
        if (ThresholdType == VoteThresholdType.AbsoluteValue)
        {
            if (voteThreshold > potentialClients.Count)
            {
                
                VoteThreshold = potentialClients.Count;
            }
            else if (voteThreshold < 1)
            {
                VoteThreshold = MinimumVoteThresholdAbsolute;
            }
            else
            {
                VoteThreshold = voteThreshold;
            }
        }
        else
        {
            if (voteThreshold > 1.0 || voteThreshold < 0.0)
            {
                VoteThreshold = DefaultVoteThresholdPercentage;
            }
            else
            {
                VoteThreshold = voteThreshold;
            }
        }
    }
}


public enum VoteThresholdType
{
    AbsoluteValue,
    Percentage,
}