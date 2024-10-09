using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EventPlayer.Config
{
    /// <summary>
    /// GlobalConfig
    /// </summary>
    internal class GlobalConfig
    {
        #region Member


        #endregion Member

        #region Properties

        public int ArtNetUniverse { get; set; }
        public List<PlayerConfig> Players { get; set; }
        public List<AudioFile> Tracks { get; set; }

        #endregion Properties

        #region Constructors
        /// <summary>
        /// Empty constructor for Config
        /// </summary>
        public GlobalConfig()
        {

        }

        #endregion Constructors

        #region Serialization
        /// <summary>
        /// Converts all properties of this instance into JSON format and write it to the specified file.
        /// </summary>
        /// <param name="path">Output path for the JSON file</param>
        public void ToJson(string path)
        {
            JsonSerializer serializer = new()
            {
                Formatting = Newtonsoft.Json.Formatting.Indented
            };

            using StreamWriter sw = new(path);
            using JsonWriter writer = new JsonTextWriter(sw);
            serializer.Serialize(writer, this);
        }

        /// <summary>
        /// Deserialize a JSON file and convert it into a new Config instance
        /// </summary>
        /// <param name="path">Path to the JSON file</param>
        /// <returns>Deserialized Config instance</returns>
        public static GlobalConfig? FromJson(string path)
        {
            using StreamReader file = File.OpenText(path);
            JsonSerializer serializer = new();

            return (GlobalConfig?)serializer.Deserialize(file, typeof(GlobalConfig));
        }

        #endregion Serialization

        /// <summary>
        /// Default configuration.
        /// After serialization, the JSON file can be used as a template to create your own configurations.
        /// </summary>
        public static GlobalConfig Default
        {
            get
            {
                return new GlobalConfig()
                {
                    ArtNetUniverse = 0,
                    Tracks =
                    [
                        new()
                        {
                            Title = "Test 1",
                            FilePath = @"C:\Temp\test_01.mp3",
                            DmxStartValue = 0,
                            DmxEndValue = 24,
                        },
                        new()
                        {
                            Title = "Test 2",
                            FilePath = @"C:\Temp\test_01.mp3",
                            Position = TimeSpan.FromSeconds(70),
                            DmxStartValue = 25,
                            DmxEndValue = 49,
                        }
                    ],
                    Players =
                    [
                        new() {
                            Name = "Player 1",
                            Description = "On-board sound",
                            PlayerId = 1,
                            OutputDeviceGuid = Guid.Parse("f5e35214-174c-4485-a7f3-17e2a48d7d52"),
                            DmxControlChannel = 33,
                            DmxVolumeChannel = 34,
                            DmxTrackChannel = 35,
                        },
                        new() {
                            Name = "Player 2",
                            Description = "BT speaker",
                            PlayerId = 2,
                            OutputDeviceGuid = Guid.Parse("52e4a75f-46c3-4acb-833e-b0a2b67af3df"),
                            DmxControlChannel = 43,
                            DmxVolumeChannel = 44,
                            DmxTrackChannel = 45,
                        }
                    ]
                };
            }
        }
    }
}
