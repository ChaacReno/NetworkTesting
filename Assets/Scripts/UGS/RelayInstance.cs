using System.Threading.Tasks;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;

namespace UGS
{
    public class RelayInstance
    {
        public string joinCode;
        private Allocation _allocation;
        private JoinAllocation _joinAllocation;
        private RelayServerData _relayServerData;
        
        public async Task CreateAllocation(int maxConnections, string region = null)
        {
            _allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections, region);
            _relayServerData = new RelayServerData(_allocation, "dtls");
            joinCode = await RelayService.Instance.GetJoinCodeAsync(_allocation.AllocationId);
            ConfigureWithNgo();
        }

        private void ConfigureWithNgo()
        {
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(_relayServerData);
        }

        public async Task JoinAllocation(string code)
        {
            _joinAllocation = await RelayService.Instance.JoinAllocationAsync(code);
            _relayServerData = new RelayServerData(_joinAllocation, "dtls");
            ConfigureWithNgo();
        }
        
        public async Task<string> GetJoinCode()
        {
            return await RelayService.Instance.GetJoinCodeAsync(_allocation.AllocationId);
        }
    }
}