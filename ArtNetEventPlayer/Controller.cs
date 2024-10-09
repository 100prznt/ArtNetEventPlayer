using ArtNetSharp.Communication;
using ArtNetSharp;
using NAudio.Wave;
using org.dmxc.wkdt.Light.ArtNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using EventPlayer.Config;

namespace EventPlayer
{
    internal class Controller : INotifyPropertyChanged
    {
        #region Members

        internal const string DEFAULT_CONFIG_PATH = @"../../../configuration.json";

        private GlobalConfig m_Configuration;
        private List<Player> m_Players;

        #endregion Members

        #region Properties
        /// <summary>
        /// Configuration
        /// </summary>
        public GlobalConfig Configuration
        {
            get => m_Configuration;
            set
            {
                SetField(ref m_Configuration, value);
            }
        }

        /// <summary>
        /// Players
        /// </summary>
        public List<Player> Players
        {
            get => m_Players;
            set
            {
                SetField(ref m_Players, value);
            }
        }

        /// <summary>
        /// Player1
        /// </summary>
        public Player Player1
        {
            get => m_Player1;
            set
            {
                SetField(ref m_Player1, value);
            }
        }
        private Player m_Player1;

        #endregion Properties

        #region Constructors
        /// <summary>
        /// Empty constructor for Controller
        /// </summary>
        public Controller()
        {
            LoadConfig();

            InitPlayers();
        }

        #endregion Constructors

        #region ArtNet handling
        /// <summary>
        /// Setup and start ArtNet instance
        /// </summary>
        public void SetupArtNetNode()
        {
            var address = new Address(new Subnet(0), new Universe(0));
            var portAddress = new PortAddress(address);

            NodeInstance nodeInstance = new NodeInstance(ArtNet.Instance);
            nodeInstance.Name = nodeInstance.ShortName = "ArtNetNode EventPlayer";

            nodeInstance.AddPortConfig(new PortConfig(1, portAddress, true, false)
            {
                PortNumber = (byte)1,
                Type = EPortType.OutputFromArtNet,
                GoodOutput = new GoodOutput(outputStyle: GoodOutput.EOutputStyle.Continuous, isBeingOutputAsDMX: true)
            });

            // Listen for new data
            nodeInstance.DMXReceived += OnDmxDataReceived;

            // Add instance
            ArtNet.Instance.AddInstance(nodeInstance);
        }

        /// <summary>
        /// Process received ArtNet data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDmxDataReceived(object sender, PortAddress e)
        {
            if (!(sender is NodeInstance ni))
                return;

            // Can be called from anywere anytime without listen to the Event!!!
            var data = ni.GetReceivedDMX(e);

            DmxByte39 = data[38];
            DmxByte40 = data[39];
            DmxByte41 = data[40];

        }

        #endregion ArtNet handling

        #region Internal services

        private void LoadConfig(string path = "")
        {
            if (string.IsNullOrEmpty(path))
            {
                Configuration = GlobalConfig.Default;
                Configuration.ToJson(DEFAULT_CONFIG_PATH);
            }
            else
                Configuration = GlobalConfig.FromJson(path);

            foreach (var file in Configuration.Tracks)
                file.UpdateMetaData();
        }

        private void InitPlayers()
        {
            Players = new List<Player>();

            Dictionary<int, WaveOutCapabilities> caps = [];
            for (int n = -1; n < WaveOut.DeviceCount; n++)
                caps.Add(n, WaveOut.GetCapabilities(n));

            foreach (var playerConf in Configuration.Players)
            {
                Player player = new(playerConf);

                try
                {
                    var outputDeviceCaps = caps.First(x => x.Value.ProductGuid.Equals(playerConf.OutputDeviceGuid));
                    player.WaveOutDeviceNumber = outputDeviceCaps.Key;
                }
                catch (Exception) { }

                //if (Player1 is null)
                //{
                //    Player1 = player;
                //    return;
                //}

                Players.Add(player);
            }
        }

        #endregion Internal services

        #region Commands

        private ICommand m_Cmd_ArtNet;
        public ICommand cmd_ArtNet
        {
            get
            {
                if (m_Cmd_ArtNet == null)
                    m_Cmd_ArtNet = new RelayCommand(Perform_ArtNet);

                return m_Cmd_ArtNet;
            }
        }
        private void Perform_ArtNet(object commandParameter)
        {
            SetupArtNetNode();
        }

        #endregion Commands



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
