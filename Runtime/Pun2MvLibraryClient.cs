using System;
using System.Threading.Tasks;
using ExitGames.Client.Photon;
using Multiverse.LibraryInterfaces;
using Multiverse.Messaging;
using Photon.Pun;
using Photon.Realtime;
using Reaction;

namespace Multiverse.Pun2
{
    public class Pun2MvLibraryClient : MonoBehaviourPunCallbacks, IMvLibraryClient, IOnEventCallback
    {
        public MvConnection LocalConnection { get; private set; }
        public RxnDictionary<int, MvConnection> Connections { get; } = new RxnDictionary<int, MvConnection>();

        RxnEvent IMvLibraryClient.OnDisconnected { get; } = new RxnEvent();

        private ByteMessageReceiver _receiver;
        private RaiseEventOptions _eventOptions;

        private void Awake()
        {
            _eventOptions = new RaiseEventOptions();

            LocalConnection = NewConnectionForPlayer(PhotonNetwork.LocalPlayer);
            Connections.AsOwner[LocalConnection.Id] = LocalConnection;
            foreach (var player in PhotonNetwork.PlayerListOthers)
                Connections.AsOwner[player.ActorNumber] = NewConnectionForPlayer(player);

            PhotonNetwork.AddCallbackTarget(this);
        }

        public override void OnLeftRoom()
        {
            ((IMvLibraryClient) this).OnDisconnected.AsOwner.Invoke();
        }

        public async Task Disconnect()
        {
            if (!PhotonNetwork.InRoom)
                return;

            PhotonNetwork.LeaveRoom();
            await ((IMvLibraryClient) this).OnDisconnected.Wait(Multiverse.Timeout);
        }

        public void SetMessageReceiver(ByteMessageReceiver receiver)
        {
            _receiver = receiver;
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

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Connections.AsOwner[newPlayer.ActorNumber] = NewConnectionForPlayer(newPlayer);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Connections.AsOwner.Remove(otherPlayer.ActorNumber);
        }

        private static MvConnection NewConnectionForPlayer(Player player)
        {
            return new MvConnection(player.NickName, player.ActorNumber, player.IsMasterClient, player.IsLocal);
        }

        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == Pun2Messages.MvClientMessageCode)
                _receiver?.Invoke(photonEvent.Sender == 0 ? null : Connections[photonEvent.Sender],
                    new ArraySegment<byte>((byte[]) photonEvent.CustomData));
        }
    }
}