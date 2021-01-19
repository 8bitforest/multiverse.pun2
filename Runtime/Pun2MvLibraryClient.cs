using System.Linq;
using System.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using Reaction;

namespace Multiverse.Pun2
{
    public class Pun2MvLibraryClient : MonoBehaviourPunCallbacks, IMvLibraryClient
    {
        public IMvConnection LocalConnection { get; private set; }
        public RxnSet<IMvConnection> Connections { get; } = new RxnSet<IMvConnection>();
        
        RxnEvent IMvLibraryClient.OnDisconnected { get; } = new RxnEvent();
        
        private void Awake()
        {
            LocalConnection = NewConnectionForPlayer(PhotonNetwork.LocalPlayer);
            Connections.AsOwner.Add(LocalConnection);
            foreach (var player in PhotonNetwork.PlayerListOthers)
                Connections.AsOwner.Add(NewConnectionForPlayer(player));
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

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            MvNetworkManager.I.Client.Disconnect();
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Connections.AsOwner.Add(NewConnectionForPlayer(newPlayer));
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            var conn = Connections.First(c => c.Id == otherPlayer.ActorNumber);
            Connections.AsOwner.Remove(conn);
        }

        private static IMvConnection NewConnectionForPlayer(Player player)
        {
            return new DefaultMvConnection
            {
                Name = player.NickName,
                Id = player.ActorNumber,
                IsHost = player.IsMasterClient,
                IsLocal = player.IsLocal
            };
        }
    }
}