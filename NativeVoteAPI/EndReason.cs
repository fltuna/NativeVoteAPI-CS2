namespace NativeVoteAPI;

/// <summary>
/// This is a reason list can be used for cancel vote.
/// </summary>
public enum EndReason
{
    /// <summary>
    /// Simply send a vote failed text
    /// </summary>
    NoReason = 0,
    /// <summary>
    /// #SFUI_vote_failed_transition_vote
    /// </summary>
    OtherPlayerIsLoading = 1,
    /// <summary>
    /// #Panorama_vote_failed_vote_spam
    /// </summary>
    CalledVoteRecently = 2,
    /// <summary>
    /// #SFUI_vote_failed_yesno
    /// </summary>
    YesVoteShouldExceedNo = 3,
    /// <summary>
    /// #SFUI_vote_failed_quorum
    /// </summary>
    NotEnoughPlayersVoted = 4,
    /// <summary>
    /// #SFUI_vote_failed_disabled_issue
    /// </summary>
    ServerDisabledThatIssue = 5,
    /// <summary>
    /// #SFUI_vote_failed_map_not_found
    /// </summary>
    MapDoesntExist = 6,
    /// <summary>
    /// #SFUI_vote_failed_map_name_required
    /// </summary>
    MapNameRequired = 7,
    /// <summary>
    /// #SFUI_vote_failed_recently
    /// </summary>
    VoteFailedRecently = 8,
    /// <summary>
    /// #SFUI_vote_failed_team_cant_call
    /// </summary>
    CurrentTeamCannotCallVote = 9,
    /// <summary>
    /// #SFUI_vote_failed_waitingforplayers
    /// </summary>
    VoteIsNotAllowedInWarmup = 10,
    /// <summary>
    /// #SFUI_vote_failed_cannot_kick_admin
    /// </summary>
    CannotKickServerAdmin = 12,
    /// <summary>
    /// #SFUI_vote_failed_scramble_in_prog
    /// </summary>
    TeamScrambleInProgress = 13,
    /// <summary>
    /// #SFUI_vote_failed_spectator
    /// </summary>
    SpectatorVoteIsDisabled = 14,
    /// <summary>
    /// #SFUI_vote_failed_recent_kick
    /// </summary>
    VoteToKickSpecificPlayerFailedRecently = 15,
    /// <summary>
    /// #SFUI_vote_failed_recent_changemap
    /// </summary>
    VoteToChangeSpecificMapFailedRecently = 16,
    /// <summary>
    /// #SFUI_vote_failed_recent_swapteams
    /// </summary>
    SwapTeamFailedRecently = 17,
    /// <summary>
    /// #SFUI_vote_failed_recent_scrambleteams
    /// </summary>
    ScrambleTeamFailedRecently = 18,
    /// <summary>
    /// #SFUI_vote_failed_recent_restart
    /// </summary>
    RestartFailedRecently = 19,
    /// <summary>
    /// #SFUI_vote_failed_swap_in_prog
    /// </summary>
    TeamSwapIsInProgress = 20,
    /// <summary>
    /// #SFUI_vote_failed_disabled
    /// </summary>
    VoteDisabledForThisServer = 21,
    /// <summary>
    /// #SFUI_vote_failed_nextlevel_set
    /// </summary>
    NextLevelHasAlreadySet = 22,
    /// <summary>
    /// #SFUI_vote_failed_surrender_too_early
    /// </summary>
    SurrenderTooEarly = 23,
    /// <summary>
    /// #SFUI_vote_failed_paused
    /// </summary>
    MatchIsAlreadyPaused = 24,
    /// <summary>
    /// #SFUI_vote_failed_not_paused
    /// </summary>
    MatchIsNotPaused = 25,
    /// <summary>
    /// #SFUI_vote_failed_not_in_warmup
    /// </summary>
    MatchIsNotInWarmup = 26,
    /// <summary>
    /// #SFUI_vote_failed_not_10_players
    /// </summary>
    Requires10Players = 27,
    /// <summary>
    /// #SFUI_vote_failed_timeout_active
    /// </summary>
    TimeoutInProgress = 28,
    /// <summary>
    /// #SFUI_vote_failed_timeouts_exhausted
    /// </summary>
    NoTimeOutsLeft = 30,
    /// <summary>
    /// #SFUI_vote_failed_cant_round_end
    /// </summary>
    CantSucceedAfterRoundEnd = 31,
    /// <summary>
    /// #SFUI_vote_failed_rematch
    /// </summary>
    FailedToCollect10VotesForRematch = 32,
    /// <summary>
    /// #SFUI_vote_failed_continue
    /// </summary>
    AllPlayersDidNotAgreeToContinueWithBots = 33,
}