# VoteKick-EXILED2
A plugin for SCP: Secret Laboratory which allows players to start votes to have other players kicked from the server.

## Commands
- `.votekick start <player> [reason]` Starts a votekick
- `.votekick yes` Votes yes to the current votekick
- `.votekick no` Votes no to the current votekick

## Permissions
- `vk.banned` Prevents users from starting votekicks
- `vk.votebanned` Prevents players from voting on votekicks
- `vk.immune` Prevents players from being kicked by votekicks
- `vk.master` Protects players with `.*` permission from being banned from voting and kicking. Should not be assigned to players manually.
