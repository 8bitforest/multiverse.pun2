using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Multiverse.LibraryInterfaces;
using Photon.Pun;
using Photon.Realtime;

namespace Multiverse.Pun2
{
    public class Pun2MvLibraryMatchmaker : MonoBehaviourPunCallbacks, IMvLibraryMatchmaker
    {
        public Connected Connected { get; set; }
        public Disconnected Disconnected { get; set; }
        public Connected ConnectedToMatch { get; set; }

        public ErrorHandler HostMatchError { get; set; }
        public ErrorHandler JoinMatchError { get; set; }

        public MatchesUpdated MatchesUpdated { get; set; }

        private Dictionary<int, RoomInfo> _rooms = new Dictionary<int, RoomInfo>();

        public void Connect()
        {
            if (PhotonNetwork.IsConnected && PhotonNetwork.InLobby)
            {
                Connected();
                return;
            }

            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnConnectedToMaster()
        {
            PhotonNetwork.JoinLobby();
        }

        public override void OnJoinedLobby()
        {
            Connected();
        }

        public void Disconnect()
        {
            if (!PhotonNetwork.IsConnected)
            {
                Disconnected();
                return;
            }

            PhotonNetwork.Disconnect();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            _rooms.Clear();
            Disconnected();
        }

        public void HostMatch(byte[] data)
        {
            PhotonNetwork.CreateRoom(Guid.NewGuid().ToString(), new RoomOptions
            {
                MaxPlayers = 0,
                CustomRoomProperties = new Hashtable {["m"] = data},
                CustomRoomPropertiesForLobby = new[] {"m"}
            });
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            _rooms.Clear();
            HostMatchError(message);
        }

        public void JoinMatch(int libId)
        {
            PhotonNetwork.JoinRoom(_rooms[libId].Name);
        }

        public void UpdateMatchList()
        {
            MatchesUpdated(_rooms.Values.Select(r => (r.GetHashCode(), (byte[]) r.CustomProperties["m"])));
        }

        public override void OnJoinedRoom()
        {
            _rooms.Clear();
            ConnectedToMatch();
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            _rooms.Clear();
            JoinMatchError(message);
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            _rooms = roomList.ToDictionary(r => r.GetHashCode());
            UpdateMatchList();
        }
    }
}