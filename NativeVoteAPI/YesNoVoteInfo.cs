namespace NativeVoteAPI;

public class YesNoVoteInfo
{
    public NativeVoteInfo VoteInfo;
    public int TotalVotes => YesVotes + NoVotes;
    public int YesVotes = 0;
    public int NoVotes = 0;
    public Dictionary<uint, VoteType> ClientVoteInfo = new();

    public YesNoVoteInfo(NativeVoteInfo voteInfo)
    {
        VoteInfo = voteInfo;

        foreach (uint client in VoteInfo.PotentialClients)
        {
            ClientVoteInfo[client] = VoteType.NotParticipated;
        }
    }
}

public enum VoteType
{
    Yes,
    No,
    Excluded,
    NotParticipated,
}