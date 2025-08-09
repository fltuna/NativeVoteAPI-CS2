namespace NativeVoteAPI;

public class VoteTranslations(string translationKey, params object[] translationArgs)
{
    public string TranslationKey { get; } = translationKey;
    public object[] TranslationArgs { get; } = translationArgs;
}