using System;
using ExitGames.Client.Photon;
using Multiverse.LibraryInterfaces;
using Photon.Pun;
using Photon.Realtime;

namespace Multiverse.Pun2
{
    public class Pun2MvLibraryServer : MonoBehaviourPunCallbacks, IMvLibraryServer, IOnEventCallback
    {
        public Disconnected Disconnected { get; set; }
        public ServerByteMessageReceiver MessageReceiver { get; set; }
        public PlayerConnected PlayerConnected { get; set; }
        public PlayerDisconnected PlayerDisconnected { get; set; }

        private RaiseEventOptions _eventOptions;
        private readonly int[] _targetActors = new int[1];

        private void Awake()
        {
            _eventOptions = new RaiseEventOptions();
            PhotonNetwork.AddCallbackTarget(this);
        }

        public override void OnLeftRoom()
        {
            Disconnected();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Disconnected();
        }

        public void Disconnect()
        {
            if (!PhotonNetwork.InRoom)
            {
                Disconnected();
                return;
            }

            PhotonNetwork.LeaveRoom();
        }

        public void SendMessageToPlayer(int libId, ArraySegment<byte> message, bool reliable)
        {
            _eventOptions.Receivers = ReceiverGroup.All;
            _targetActors[0] = libId;
            _eventOptions.TargetActors = _targetActors;
            PhotonNetwork.RaiseEvent(Pun2Messages.MvClientMessageCode, message, _eventOptions,
                reliable ? SendOptions.SendReliable : SendOptions.SendUnreliable);
        }

        public void SendMessageToAll(ArraySegment<byte> message, bool reliable)
        {
            _eventOptions.Receivers = ReceiverGroup.All;
            _eventOptions.TargetActors = null;
            PhotonNetwork.RaiseEvent(Pun2Messages.MvClientMessageCode, message, _eventOptions,
                reliable ? SendOptions.SendReliable : SendOptions.SendUnreliable);
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            PlayerConnected(newPlayer.ActorNumber);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            PlayerDisconnected(otherPlayer.ActorNumber);
        }

        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == Pun2Messages.MvServerMessageCode)
                MessageReceiver(photonEvent.Sender, new ArraySegment<byte>((byte[]) photonEvent.CustomData));
        }
    }
}