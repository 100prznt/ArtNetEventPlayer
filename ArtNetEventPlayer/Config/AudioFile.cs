using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EventPlayer.Config
{
    internal class AudioFile : IEquatable<AudioFile>
    {
        public string Title { get; set; }

        public string FilePath { get; set; }

        public TimeSpan? Position { get; set; }

        public byte DmxStartValue { get; set; }

        public byte DmxEndValue { get; set; }

        [JsonIgnore]
        public TimeSpan Length { get; set; }

        [JsonIgnore]
        public bool FileExists => File.Exists(FilePath);

        public bool Equals(AudioFile? other)
        {
            return
                string.Equals(FilePath, other.FilePath, StringComparison.OrdinalIgnoreCase) &&
                Position == other.Position;
        }

        public void UpdateMetaData()
        {
            var reader = new MediaFoundationReader(FilePath);
            Length = reader.TotalTime;

            if (Position is TimeSpan pos && pos <= Length)
                throw new ArgumentOutOfRangeException("Seek position (" + pos + ") is out of title length (" + Length + ").");

            if (string.IsNullOrEmpty(Title))
                Title = Path.GetFileNameWithoutExtension(FilePath);
        }
    }
}
