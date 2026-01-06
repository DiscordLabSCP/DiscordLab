# DiscordLab ![Downloads](https://img.shields.io/github/downloads/JayXTQ/DiscordLab/total)

The modular Discord bot plugin for SCP: Secret Laboratory servers. With DiscordLab you can pick and choose what
features you want on your Discord server/bot. This allows you to have a Discord bot that is tailored to your server's
needs.

## Get started

To get started, check out our installation guide: https://discordlab.jxtq.moe/getting-started/installation/

## Features

### Modular

DiscordLab easily allows users to only have a subset of features and doesn't require the server to be tracking loads of
events that will never even be logged.
Your server (or other plugin) developers can easily integrate with the bot as well because of
the [NuGet package](https://www.nuget.org/packages/DiscordLab.Bot).
Go to the [API section](#api) for more information.

### Logging for events

DiscordLab's modules track loads of different kinds of events and sends them directly to your Discord server.

### Customisable messages

DiscordLab has a feature that makes it so messages can easily be edited from their default to make it so you can have an
embed, raw message content or both!

There is also large variety of placeholders that can be used, which will then be replaced by DiscordLab before sending
out the message.

### Support for multiple servers/channels

DiscordLab allows for you to put channel and guild IDs directly into configs that allows you to seperate logs however
you want,
with each trackable event being assigned a separate channel config option.

If you want damage logs and death logs to be in separate channels, they can be routed to different channels.

### Commands

Some DiscordLab modules come with slash commands (can also be disabled) that can be used within Discord, i.e. in
`DiscordLab.Administration` there is the `/send` command that
allows admins to send commands to your server.

*Slash commands on DiscordLab do not have their own permission system, commands that should be hidden are hidden behind
some default permissions, if you wish to edit the
permissions, you can read up on how to on
this [Discord blog post](https://discord.com/blog/slash-commands-permissions-discord-apps-bots)*

### Moderation Utilities

DiscordLab.Moderation comes with commands and utilities to better help with mutes, including adding the functionality of
temporary mutes. Can be done via RA commands or Discord.

## API

You can find all information here: https://discordlab.jxtq.moe/api/

## Support

Need support? Check out our Discord: https://discord.gg/XBzuGbsNZK
