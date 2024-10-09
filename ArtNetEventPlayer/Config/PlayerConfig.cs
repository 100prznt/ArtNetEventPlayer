using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventPlayer.Config
{
    /// <summary>
    /// Player configuration
    /// </summary>
    internal class PlayerConfig
    {
        /// <summary>
        /// Player name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Player description
        /// e.g. location
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// DMX control channel number (PLAY, STOP, PAUSE)
        /// </summary>
        public ushort DmxControlChannel { get; set; }

        /// <summary>
        /// DMX volume channel
        /// </summary>
        public ushort DmxVolumeChannel { get; set; }

        /// <summary>
        /// DMX track channel
        /// </summary>
        public ushort DmxTrackChannel { get; set; }

        /// <summary>
        /// Player ID
        /// </summary>
        public int PlayerId { get; set; }

        /// <summary>
        /// GUID of mapped output device
        /// </summary>
        public Guid OutputDeviceGuid { get; set; }
    }
}
