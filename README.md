# Achilles

Always wanted to try my hand at creating a Habbo emulator, so let's go.
The project is messy right now as I'm still working out how I want to have the database structure set up, any part that interacts with the database will most likely change later.

Please keep in mind that this isn't my usual cup of tea, I usually work in the HTTP space :)

## Requirements

* Habbo v17 client files + loader

## Features

### Abstractions, Dependency Injection & Entity Framework

Easily swap out database connection, TCP server or even the Habbo implementation without worrying about breaking other parts. In the future I'd like to be able to support versions from v1 all the way up to v26 (everything after that sucks anyways)

Since the project uses the Microsoft Hosting model, you could for example spin up an infinite amount of servers in the same process or with a little work offer something like Minecraft Realms where users can get their own server.

### Easily extended

The way incoming packets and outgoing packets are handled using class attributes allows for easy extension of the emulator without rewriting code. For example you could support higher or lower client versions by using *MessageTypeResolver.ReplaceResolver()* to replace version-specific packets or just create new *IncomingMessage* classes for the features you'd like to implement.

## Roadmap

### Client


| Category        | Completed | Remaining      |
| :---------------- | ----------- | :--------------- |
| Registration    | Yes       |                |
| Login           | Yes        |             |
| User Profile    | No        | Set badge      |
| Habbo Club      | Yes       |                |
| Purse           | Yes       |                |
| Messenger       | No        | follow friend  |
| Navigator       | Yes       |                |
| Private Rooms   | No        | Entering rooms |
| Public Rooms    | No        | Entering rooms |
| Catalogue       | No        | Not started    |
| Inventory       | No        | Not started    |
| Recycler        | No        | Not started    |
| Pets            | No        | Not started    |
| Bots            | No        | Not started    |
| Trading         | No        | Not started    |
| Moderation      | No        | Not started    |
| Room events     | No        | Not started    |
| Battleball      | No        | Not started    |
| Wobble Squabble | No        | Not started    |
| Snowstorm       | No        | Not started    |
| Pool Dive       | No        | Not started    |
| Infobus         | No        | Not started    |

### Project


| Category      | Completed | Remaining   |
| :-------------- | ----------- | :------------ |
| Plugin System | No        | Not started |
| Housekeeping  | No        | Not started |

### Refactor

When all these steps are completed, I'll take what I've learned and do a full refactor.

## Hopes and dreams

I'm hoping to be able to complete this, but it will take a while for sure.
A lot of this work is done by digging through decompiled files, and even then I have to look at other peoples emulators to really understand what's going on as the decompiled code is often just absolute junk. I actually started out by trying to understand the Debbo source and I hope I never have to look at it again.
