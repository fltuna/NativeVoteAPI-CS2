# NativeVoteAPI CS2

CounterStrikeSharp implementation of API for Counter-Strike 2's native vote.

## Caution

This is just an API for native vote!

This API does not provide standalone functionality. You need to create your own plugin to use it.


## Features

- Basic vote management
- Panorama vote UI
- Event system (on vote fail, pass, cancel)

## Setup

1. Download NativeVoteAPI from latest [Release](https://github.com/fltuna/NativeVoteAPI-CS2/releases/latest)
2. Put into your `game/csgo/addons/counterstrikesharp/` directory
3. Ready to use this API

## Development

### Setup

1. Create or go to your existing project
2. run `dotnet add package NativeVoteAPI-CS2`
3. Ready to develop.

Full example is here: [NativeVoteAPITest.cs](NativeVoteAPITest/NativeVoteApiTest.cs)



## Using custom string in vote text

Since valve fixed a XSS exploit in html text, we can't use custom string in normal mean.

When we provide custom UI text in a `platform_<language name>.txt` file (e.g. platform_english.txt) and place it in the `game/csgo/resource/` directory on both server and client, we can use custom text from the plugin.

### Note

We can use pictures using `<img src='{s:s1}'>`, but it will display only once. after second one is not showing correctly.

### Example

platform_english.txt

```
"lang"
{
    "Language" "English" 
    "Tokens"
    {
        "SFUI_vote_custom_vote_default"           "{s:s1}"
        "SFUI_vote_custom_vote_green"             "<font color='#00FF00'>{s:s1}</font>"
        "SFUI_vote_custom_vote_red"               "<font color='#FF0000'>{s:s1}</font>"
        "SFUI_vote_custom_vote_blue"              "<font color='#0000FF'>{s:s1}</font>"
        "SFUI_vote_custom_vote_image"             "<img src='{s:s1}'>"
    }
}
```

Plugin code (edit from NativeVoteApiTest.cs)
```csharp
var potentialClients = Utilities.GetPlayers().Where(p => p is { IsBot: false, IsHLTV: false }).ToList();
var potentialClientsIndex = potentialClients.Select(p => p.Index).ToList();


string displayString = "#SFUI_vote_custom_vote_default";
string detailsString = "Custom string that will displayed in vote";

string voteIdentifier = "TEST_VOTE!";


NativeVoteInfo nInfo = new NativeVoteInfo(voteIdentifier, displayString ,detailsString, potentialClientsIndex, VoteThresholdType.AbsoluteValue, 0.5F, 5.0F);

// When the vote is successfully initiated, it will return InitializeAccepted.
NativeVoteState state = _nativeVoteApi.InitiateVote(nInfo);
```