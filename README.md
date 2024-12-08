# DiscordLab ![Downloads](https://img.shields.io/github/downloads/JayXTQ/DiscordLab/total)

The modular Discord bot plugin for SCP: Secret Laboratory servers. With DiscordLab you can pick and choose what
features you want on your Discord server/bot. This allows you to have a Discord bot that is tailored to your server's needs.

## Get started

Go to the [Releases](https://github.com/JayXTQ/DiscordLab/releases) and download the latest version
of `DiscordLab.Bot.dll` and all other modules you want for your bot (make sure you have a look for the latest version of your module released, 
it could cause issues if you download an outdated one). If you are curious about modules please go to
the [Modules](#modules) section. After downloading the files, put them in your EXILED plugins folder and start
your server.

You will then need to configure the config options to get the bot started. Make sure you have already created
your Discord bot and saved your token somewhere because you will need to put this in the Token section of the
`DiscordLab` config. If you don't know how to create a bot, please follow this
useful guide here (not made by the DiscordLab team) [over at discord.py](https://discordpy.readthedocs.io/en/stable/discord.html).

Then for the other modules you will have to fill in the other config options like channel IDs, role IDs, whatever.
If you are unsure about how to get IDs, Discord has a 
[great tutorial which is worth a read](https://support.discord.com/hc/en-us/articles/206346498-Where-can-I-find-my-User-Server-Message-ID).

After you have configured the bot, start your server and the bot should start up. If you have any issues, please make an issue.

## Modules

- `DiscordLab.StatusChannel.dll` - Will create a message in the set channel with the current status of the server. If there is already a message by the bot then it will edit that.
- `DiscordLab.BotStatus.dll` - Will set the bot's status to the current server status.
- `DiscordLab.ConnectionLogs.dll` - Will send a message in a specific channel when people connect, disconnect or when round starts.
- `DiscordLab.DeathLogs.dll` - Will send a message in a specific channel when someone dies, or gets killed when cuffed (good for moderation purposes, if needed)
- `DiscordLab.XPSystem.dll` - Allows integration with the XPSystem plugin (EXILED only) to give you a `/getlevel` (customisable) command and a logger to log when people level up.
- `DiscordLab.AdvancedLogging.dll` - Allows you to add your own custom logs from Exiled to the bot with your own special message and your own channel. You can read more
about this in the [Advanced Logging](#advanced-logging) section.
- `DiscordLab.Moderation.dll` - Allows you to ban and unban people directly from the Discord bot. This is useful for when you are not near your server and need to ban someone, requires `DiscordLab.ModerationLogs` to log out
any information.
- `DiscordLab.ModerationLogs.dll` - Logs all moderation actions done on the SCP server into your chosen Discord channels, including admin chat logs and reporting. 
- `DiscordLab.SCPSwap.dll` - Logs all swaps that happen between SCPs on the server. This is useful if you want to see if someone switched to a different SCP.

## Requesting modules

You can make module requests by making an issue on the GitHub repository. Please make sure
to include a detailed description of what you want the module to do and how you want it to work.

If the module is something that interacts with another plugin, please link the plugin.

## API Guide

You should really only reference DiscordLab.Bot in your project, unless you need to hook to another module for their Channel or whatever.

With DiscordLab.Bot, it has some methods that you can use to instantly start up your module. There is 2 interfaces which can be used, IRegisterable and ISlashCommand.

IRegisterable is used for registering Handlers, such as Discord bot related stuff and events, and ISlashCommand is used for registering Slash Commands.

If you use both interfaces on your classes, this allows for you to easily register your module with the bot. All you need to do is get `DiscordLab.Bot.API.Modules.HandlerLoader` and make a new instance of it, then in
`OnEnabled` call `HandlerLoader.Load()` and inside the Load method, pass in `Assembly`. This will then load all the handlers and slash commands within your module. Also don't forget to remove the handlers in the `OnDisabled`
method, you can do this by calling `HandlerLoader.Unload()`, no need for `Assembly` in this one though.

In `DiscordLab.Bot.Handlers` there is a `DiscordBot` class which you can use to interact with the bot which is logged in.
This class has an `Instance` property which means you can just do `DiscordLab.Bot.Handlers.DiscordBot.Instance` to get the bot instance. This has
a `Client` property which is the main Discord client and a `Guild` property which is the guild the main bot has referenced in the config.

You should always bind stuff to the `Guild` property for sending messages or finding channels, but you may need to
use the `Client` property sometimes too, like for the `Ready` event. When doing the Ready event, you can just add the
event on like a normal event, so usually it would be like this:

```csharp
DiscordLab.Bot.Handlers.DiscordBot.Instance.Client.Ready += OnReady;
```

```csharp
private async Task OnReady()
{
    // Do stuff here
}
```

If you want to access Guild in the ready event, I would recommend doing a `Timing.CallDelayed` as Guild is fetched in the Ready
event too, so it may not be available straight away.

Also, you can just do any event that Discord.Net supports, not just Ready.

Last thing, make sure channels are always text channels if you plan on sending/receiving messages, `Guild.GetTextChannel` is a good method to use.

### Example Slash Command

```csharp
public class Players : ISlashCommand
{
    public SlashCommandBuilder Data { get; } = new()
    {
        Name = "players",
        Description = "Get the list of players on the server"
    };

    public async Task Run(SocketSlashCommand command)
    {
        string players = string.Join("\n", Player.List.Select(player => player.Nickname));
        await command.RespondAsync(players);
    }
}
```

For more things relating to the SlashCommandBuilder object, go to the [Discord.Net documentation page](https://docs.discordnet.dev/api/Discord.SlashCommandBuilder.html) where it has every single thing you can instantiate SlashCommandBuilder with.

## Advanced Logging

Advanced Logging is a feature that allows you to log custom messages to a specific channel in Discord. This is useful for logging
events that DiscordLab doesn't already have modules for, but it is advanced hence it's name, but this should be a clear guide for
you to follow. Once you've added the module and started your bot, you don't need to edit the config file, because there is a command
you can use to add your own events, `/addlog`. Here when you run the command, a modal will show up with multiple different boxes, let
me explain how each one works:

- **Handler** - This is the handler, so `Player` might be an example, this would get the Handler `Exiled.Events.Handlers.Player`.
- **Event** - This is the event, so `Died` might be an example, this would get the event `Died` from `Exiled.Events.Handlers.Player`, 
if you set your handler as `Player` that is. This would get the event `Exiled.Events.Handlers.Player.Died` and bind to it.
- **Message** - This is the message you want to send to the channel, here is the tricky part though, I do not write every single event that
is created using this method, so you need to add your own placeholder, they work like this, let's say we are bound to `Player.Died`,
here is what the message may look like for this case.
```
Player {Player.Nickname} has been killed.
```
So in this case you will see that `{Player.Nickname}` is a placeholder, you will need to go to the event args you are binding to
and find all the variables you need, you need to bind to that variable exactly or risk a null exception error. Capitals also matter.
Ignore the `ev.` part though, that is not required and is done for you.
- **Nullables** - This is a comma seperated list of nullable variables, and if one of the variables you mention inside here is null,
then the message will not be sent, this is useful for stuff like `Attacker` in the `Player.Died` event, as if the attacker is null,
then the message will not be sent.
- **Channel** - This is the channel ID you want to send the message to, you can get this by right-clicking a channel and clicking 
`Copy ID`. There is a tutorial on how to get IDs in the [Get started](#get-started) section.

After running this /addlog command, you shouldn't need to restart your server.
