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
    public class Pun2MvLibraryServer : MonoBehaviourPunCallbacks, IMvLibraryServer, IOnEventCallback
    {
        public RxnDictionary<int, MvConnection> Clients { get; } = new RxnDictionary<int, MvConnection>();

        RxnEvent IMvLibraryServer.OnDisconnected { get; } = new RxnEvent();

        private ByteMessageReceiver _receiver;
        private RaiseEventOptions _eventOptions;
        private int[] _targetActors = new int[1];

        private void Awake()
        {
            _eventOptions = new RaiseEventOptions();

            var localClient = NewConnectionForPlayer(PhotonNetwork.LocalPlayer);
            Clients.AsOwner[localClient.Id] = localClient;
            foreach (var player in PhotonNetwork.PlayerListOthers)
                Clients.AsOwner[player.ActorNumber] = NewConnectionForPlayer(player);

            PhotonNetwork.AddCallbackTarget(this);
        }

        public override void OnLeftRoom()
        {
            ((IMvLibraryServer) this).OnDisconnected.AsOwner.Invoke();
        }

        public async Task Disconnect()
        {
            if (!PhotonNetwork.InRoom)
                return;

            PhotonNetwork.LeaveRoom();
            await ((IMvLibraryServer) this).OnDisconnected.Wait(Multiverse.Timeout);
        }

        public void SetMessageReceiver(ByteMessageReceiver receiver)
        {
            _receiver = receiver;
        }

        public void SendMessageToClient(MvConnection connection, ArraySegment<byte> message, bool reliable)
        {
            _eventOptions.Receivers = ReceiverGroup.All;
            _targetActors[0] = connection.Id;
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
            Clients.AsOwner[newPlayer.ActorNumber] = NewConnectionForPlayer(newPlayer);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Clients.AsOwner.Remove(otherPlayer.ActorNumber);
        }

        private static MvConnection NewConnectionForPlayer(Player player)
        {
            return new MvConnection(player.NickName, player.ActorNumber, player.IsMasterClient, player.IsLocal);
        }

        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == Pun2Messages.MvServerMessageCode)
                _receiver(photonEvent.Sender == 0 ? null : Clients[photonEvent.Sender],
                    new ArraySegment<byte>((byte[]) photonEvent.CustomData));
        }
    }
}