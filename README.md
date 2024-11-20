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
- `DiscordLab.XPSystem.dll` - Allows integration with the XPSystem plugin (EXILED only) to give you a `/getlevel` (customisable) command and an logger to log when people level up.

## Requesting modules

You can make module requests by making an issue on the GitHub repository. Please make sure
to include a detailed description of what you want the module to do and how you want it to work.

If the module is something that interacts with another plugin, please link the plugin.

## API Guide

You should really only reference DiscordLab.Bot in your project, unless you need to hook to another module for their Channel or whatever.

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

### Make a simple module that creates a command to show how many members are in the server

First off, you should have your project and a .csproj file setup referencing `DiscordLab.Bot` as a dependency.
You will also need `Discord.Net.Core` and `Discord.Net.WebSocket` as dependencies too.

After that you should get the `Client` instance from `DiscordLab.Bot.Handlers.DiscordBot.Instance.Client`
and then bind to the `Ready` event like displayed above in the last section. After that start by making
your command, I would recommend using the
[Discord.Net Guide](https://docs.discordnet.dev/guides/int_basics/application-commands/slash-commands/creating-slash-commands.html)
for making commands.

Once you have your SlashCommandBuilder setup, run `Timing.CallDelayed` with a 1-second delay
(we need to wait for the Guild to be fetched) and then bind your command to `Guild` by doing:
```csharp
Guild.CreateApplicationCommandAsync(command.Build());
```

Make sure you also add a try catch block around it so there is no errors when the command is created.

After that, the command should appear inside the Discord server, but will most likely do nothing.

Now this is where we make the command functionality, luckily in Discord.Net, they have an easy event
to bind to called `SlashCommandExecuted`. This event is called when a slash command is executed, hence
the name. You should bind to this event by making a method like so:

```csharp
private async Task SlashCommandHandler(SocketSlashCommand command)
{
    // Do stuff here
}
```

Inside your command handler, you should check that the command name is the correct one you set inside the
`SlashCommandBuilder` you made earlier, you can check this by doing `command.Data.Name`. Check if the
command name is the same as the one you set. For this I will reference players as the name of the
command.

For my case the command name is `players`, so I would do:
```csharp
if (command.Data.Name == "players")
{
    // Do stuff here
}
```

Inside the if statement, I will get the amount of players in the server by doing:
```csharp
int playerCount = Server.PlayerCount;
```
and then do something like:
```csharp
string message = $"There are {playerCount} players in the server.";
await command.RespondAsync(message);
```

This pretty much creates a string to respond with using the server's player count and responds with
that message. Pretty simple. I won't teach you the ins and outs with Discord.Net and Exiled so check
out their relevant resources for more information and don't be afraid to ask Google for answers, they
tend to know every answer.

[Discord.Net Documentation](https://docs.discordnet.dev/guides/introduction/intro.html)