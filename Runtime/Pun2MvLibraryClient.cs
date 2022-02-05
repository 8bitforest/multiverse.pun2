using System;
using ExitGames.Client.Photon;
using Multiverse.LibraryInterfaces;
using Photon.Pun;
using Photon.Realtime;

namespace Multiverse.Pun2
{
    public class Pun2MvLibraryClient : MonoBehaviourPunCallbacks, IMvLibraryClient, IOnEventCallback
    {
        public Disconnected Disconnected { get; set; }
        public ClientByteMessageReceiver MessageReceiver { get; set; }

        private RaiseEventOptions _eventOptions;

        private int _masterClientId;

        private void Awake()
        {
            _eventOptions = new RaiseEventOptions();
            PhotonNetwork.AddCallbackTarget(this);
            _masterClientId = PhotonNetwork.MasterClient.ActorNumber;
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

        public void SendMessageToServer(ArraySegment<byte> message, bool reliable)
        {
            _eventOptions.Receivers = ReceiverGroup.MasterClient;
            PhotonNetwork.RaiseEvent(Pun2Messages.MvServerMessageCode, message, _eventOptions,
                reliable ? SendOptions.SendReliable : SendOptions.SendUnreliable);
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            MvNetworkManager.I.Client.Disconnect();
        }

        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == Pun2Messages.MvClientMessageCode)
                MessageReceiver?.Invoke(new ArraySegment<byte>((byte[]) photonEvent.CustomData));
        }
    }
}