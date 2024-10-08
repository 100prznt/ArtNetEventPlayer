# ArtNetEventPlayer
A ArtNet controllable multichannel audio player.


## How it works

![ArtNetEventPlayer overview](docu/overview.drawio.dark.svg#gh-dark-mode-only)
![ArtNetEventPlayer overview](docu/overview.drawio.light.svg#gh-light-mode-only)

The core of the software is an ArtNet receiver, which listens to changes in a defined ArtNet universe. It forwards the channel data assigned to the configured players. The players decode this data and are controlled by it (play/pause/stop, volume and audio track). The player channel assignment can be defined with a central configuration file (JSON) and a specific WaveOut device can be mapped. A WaveOut device can be any standard sound output device on the Windows PC, for example the on-board soundcard or paired Bluetooth speakers. The configuration also includes a collection with audio files, specific playback positions and the mapped value range on the DMX ``TrackChannel``.
