using Microsoft.Extensions.Localization;

namespace NativeVoteAPI;

public class TranslatableVoteTexts(
    IStringLocalizer localizer,
    VoteTranslations? detailsTranslation = null,
    VoteTranslations? displayTranslation = null)
{
    public IStringLocalizer Localizer { get; } = localizer;
    public VoteTranslations? DetailsTranslation { get; } = detailsTranslation;
    public VoteTranslations? DisplayTranslation { get; } = displayTranslation;
}