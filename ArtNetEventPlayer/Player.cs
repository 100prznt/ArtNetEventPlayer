using EventPlayer.Config;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EventPlayer
{
    /// <summary>
    /// Player
    /// </summary>
    internal class Player : INotifyPropertyChanged
    {
        #region Members

        private PlayerConfig m_Configuration;

        private WaveOutEvent? m_OutputDevice;
        private int m_WaveOutDeviceNumber = -1;
        private AudioFileReader? m_AudioFileReader;

        private int m_DmxControlChannelData;
        private byte m_DmxVolumeChannelData;
        private byte m_DmxTrackChannelData;

        private AudioFile m_CurrentTrack;
        private AudioFile? m_NextTrack;

        private bool m_IsPause;

        #endregion Members

        #region Properties
        /// <summary>
        /// Current player configuration
        /// </summary>
        public PlayerConfig Configuration
        {
            get => m_Configuration;
            set 
            {
                SetField(ref m_Configuration, value);
            }
        }

        /// <summary>
        /// WaveOutDeviceNumber
        /// </summary>
        public int WaveOutDeviceNumber
        {
            get => m_WaveOutDeviceNumber;
            set
            {
                SetField(ref m_WaveOutDeviceNumber, value);
            }
        }

        /// <summary>
        /// DmxControlChannelData
        /// </summary>
        public int DmxControlChannelData
        {
            get => m_DmxControlChannelData;
            set
            {
                SetField(ref m_DmxControlChannelData, value);
            }
        }

        /// <summary>
        /// DmxVolumeChannelData
        /// </summary>
        public byte DmxVolumeChannelData
        {
            get => m_DmxVolumeChannelData;
            set
            {
                SetField(ref m_DmxVolumeChannelData, value);
            }
        }

        /// <summary>
        /// DmxTrackChannelData
        /// </summary>
        public byte DmxTrackChannelData
        {
            get => m_DmxTrackChannelData;
            set
            {
                SetField(ref m_DmxTrackChannelData, value);
            }
        }

        /// <summary>
        /// CurrentTrack
        /// </summary>
        public AudioFile CurrentTrack
        {
            get => m_CurrentTrack;
            set
            {
                SetField(ref m_CurrentTrack, value);
            }
        }

        /// <summary>
        /// NextTrack
        /// </summary>
        public AudioFile? NextTrack
        {
            get => m_NextTrack;
            set
            {
                SetField(ref m_NextTrack, value);
            }
        }

        #endregion Properties

        #region Constructor
        /// <summary>
        /// Empty Constructor
        /// </summary>
        public Player(PlayerConfig config)
        {
            Configuration = config;
        }

        #endregion Constructor

        #region Services

        public void Play()
        {
            if (!m_IsPause)
            {
                if (m_OutputDevice is null)
                {
                    m_OutputDevice = new WaveOutEvent
                    {
                        DeviceNumber = WaveOutDeviceNumber
                    };
                    m_OutputDevice.PlaybackStopped += OnPlaybackStopped;
                }

                if (NextTrack is AudioFile nextTrack)
                {
                    CurrentTrack = nextTrack;
                    NextTrack = null;
                    m_AudioFileReader?.Dispose();
                    m_AudioFileReader = null;
                }

                if (m_AudioFileReader is null)
                {
                    m_AudioFileReader = new AudioFileReader(CurrentTrack.FilePath);
                    if (CurrentTrack.Position is TimeSpan pos)
                        m_AudioFileReader.CurrentTime = pos;

                    m_OutputDevice.Init(m_AudioFileReader);

                }
            }

            if (m_OutputDevice is null)
            {
                m_IsPause = false;
                Play();
            }
            else
                m_OutputDevice.Play();
        }

        public void Pause()
        {
            m_IsPause = true;
            m_OutputDevice?.Pause();
        }

        public void Stop()
        {
            m_OutputDevice?.Stop();
        }

        public void ApplyDmxData(byte[] data)
        {
            DmxControlChannelData = data[Configuration.DmxControlChannel - 1];
            DmxVolumeChannelData = data[Configuration.DmxVolumeChannel - 1];
            DmxTrackChannelData = data[Configuration.DmxTrackChannel - 1];
        }

        #endregion Services

        #region Internal services

        private void OnPlaybackStopped(object sender, NAudio.Wave.StoppedEventArgs args)
        {
            m_OutputDevice?.Dispose();
            m_OutputDevice = null;
            m_AudioFileReader?.Dispose();
            m_AudioFileReader = null;
        }

        #endregion Internal services

        #region InotifyPropertyChanged
        /// <summary>
        /// Notifies a change of a property of the instance
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Trigger the <seealso cref="PropertyChanged"/> event if clients are attached
        /// </summary>
        /// <param name="propertyName">Name of the property</param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Applies the value to the corresponding member and triggers the <seealso cref="PropertyChanged"/> event if successful.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field">Local member of the property</param>
        /// <param name="value">New value for the property</param>
        /// <param name="propName">Propertyname, can also detect by caller member name</param>
        /// <returns>true if the value applied to the local member; false if value and local member are already equal</returns>
        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;
            else
            {
                field = value;
                OnPropertyChanged(propName);
                return true;
            }
        }

        #endregion InotifyPropertyChanged
    }
}
