using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using Reaction;

namespace Multiverse.Pun2
{
    public class Pun2MvLibraryMatchmaker : MonoBehaviourPunCallbacks, IMvLibraryMatchmaker
    {
        public bool Connected => PhotonNetwork.IsConnected;
        RxnEvent IMvLibraryMatchmaker.OnDisconnected { get; } = new RxnEvent();

        private TaskCompletionSource _connectTask;
        private TaskCompletionSource _disconnectTask;
        private TaskCompletionSource _createMatchTask;
        private TaskCompletionSource _joinMatchTask;

        private readonly HashSet<MvMatch> _matchListCache = new HashSet<MvMatch>();

        public async Task Connect()
        {
            if (PhotonNetwork.IsConnected && PhotonNetwork.InLobby)
                return;

            _connectTask = new TaskCompletionSource();
            PhotonNetwork.ConnectUsingSettings();
            await _connectTask.Task;
        }

        public override void OnConnectedToMaster()
        {
            PhotonNetwork.JoinLobby();
        }

        public override void OnJoinedLobby()
        {
            _connectTask?.SetResult();
            _connectTask = null;
        }

        public async Task Disconnect()
        {
            if (!PhotonNetwork.IsConnected)
                return;

            _disconnectTask = new TaskCompletionSource();
            PhotonNetwork.Disconnect();
            await _disconnectTask.Task;
            ((IMvLibraryMatchmaker) this).OnDisconnected.AsOwner.Invoke();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            _matchListCache.Clear();
            _disconnectTask?.SetResult();
            _disconnectTask = null;
        }

        public async Task CreateMatch(string matchName, int maxPlayers)
        {
            _createMatchTask = new TaskCompletionSource();
            PhotonNetwork.CreateRoom(matchName, new RoomOptions
            {
                MaxPlayers = (byte) maxPlayers
            });
            await _createMatchTask.Task;
        }

        public override void OnCreatedRoom()
        {
            _matchListCache.Clear();
            _createMatchTask?.SetResult();
            _createMatchTask = null;
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            _matchListCache.Clear();
            _createMatchTask?.SetException(new MvException(message));
            _createMatchTask = null;
        }

        public async Task JoinMatch(MvMatch match)
        {
            _joinMatchTask = new TaskCompletionSource();
            PhotonNetwork.JoinRoom(match.Id);
            await _joinMatchTask.Task;
        }

        public override void OnJoinedRoom()
        {
            _matchListCache.Clear();
            _joinMatchTask?.SetResult();
            _joinMatchTask = null;
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            _joinMatchTask?.SetException(new MvException(message));
            _joinMatchTask = null;
        }

        public Task<IEnumerable<MvMatch>> GetMatchList()
        {
            return Task.FromResult(_matchListCache.AsEnumerable());
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            _matchListCache.Clear();
            foreach (var match in roomList)
                _matchListCache.Add(new MvMatch(match.Name, match.Name, match.MaxPlayers));
        }
    }
}