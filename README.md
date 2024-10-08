# ArtNetEventPlayer
A ArtNet controllable multitrack audio player. I developed this software for my home halloween show. Here I control DMX spotlights and effects via cues in DMX Control. The ArtNetEventPlayer is used in combination with Bluetooth speakers to automatically and synchronize sound effects in addition to light and fog.

## How it works
The core of the software is an ArtNet receiver, which listens to changes in a defined ArtNet universe. It forwards the channel data assigned to the configured players. The players decode this data and are controlled by it (play/pause/stop, volume and audio track). The player channel assignment can be defined with a central configuration file (JSON) and a specific WaveOut object can be mapped. A WaveOut object can be any standard sound output device on the Windows PC, for example the on-board soundcard or paired Bluetooth speakers. The configuration also includes a collection with audio files, specific playback positions and the mapped value range on the DMX ``TrackChannel``.

![ArtNetEventPlayer overview](docu/overview.drawio.dark.svg#gh-dark-mode-only)
![ArtNetEventPlayer overview](docu/overview.drawio.light.svg#gh-light-mode-only)

When the program is started, a separate instance is created for each configured player. The associated WaveOut object is started in a separate task. This means that the sound output is not interrupted by other players or the ArtNet receiver. Depending on the system performance, it is of course not possible to instantiate an unlimited number of players simultaneously.
The _Track Library_ is also created when the program is started. In addition to the audio files (*.mp3), this library contains the playback position and the assigned DMX value range. When a player on the ``TrackChannel`` receives a changed value, the player searches the _Track Library_ for a fitting entry and plays the track after the next stop/play cycle.

## DMX Channels

3 DMX channels are required for each player. The channel numbers can be freely defined, but must be within an Art Net universe (512 channels).

| Channel            | Description              |                                             | 
|--------------------|--------------------------|---------------------------------------------|
| ``ControlChannel`` | set the player mode      | 0..84: stop; 85..169: play; 170..255: pause |
| ``VolumeChannel``  | set the player volume    | 0..255: 0..100 %                            |
| ``TrackChannel``   | select the track to play | 0..255: track mapping as configured         |
