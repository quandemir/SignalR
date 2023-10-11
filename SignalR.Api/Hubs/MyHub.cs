using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SignalR.Api.Models;

namespace SignalR.Api.Hubs
{
    public class MyHub : Hub
    {
        private readonly AppDbContext _appDbContext;

        public MyHub(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        private static List<string> Names { get; set; } = new List<string>();//bunu veri tabanına bağlamamak için yaptık 
        private static int ClientCount { get; set; } = 0;
        public static int TeamCount { get; set; } = 7;
        public async Task SendProduct(Product p)
        {
            await Clients.All.SendAsync("ReceiveProduct",p);
        }
        public async Task SendName(string name)
        {
            if (Names.Count >= TeamCount)
            {
                await Clients.Caller.SendAsync("Error",$"Takım en fazla {TeamCount} kişi olabilir");
            }
            else
            {
                Names.Add(name);
                await Clients.All.SendAsync("ReceiveName", name);
            }
        }
        public async Task GetNames()
        {
            await Clients.All.SendAsync("ReceiveNames", Names);
        }



        //Groups
        public async Task AddToGroup(string teamName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, teamName);
        }
        public async Task SendNameByGroup(string Name,string teamName)
        {
            var team=_appDbContext.Teams.Where(x=>x.Name == teamName).FirstOrDefault();
            if (team != null)
            {
                team.Users.Add(new User { Name = Name });
            }
            else
            {
                var newTeam = new Team { Name = teamName };
                newTeam.Users.Add(new User { Name = Name });
                _appDbContext.Teams.Add(newTeam);
            }
            await _appDbContext.SaveChangesAsync();

            await Clients.Group(teamName).SendAsync("ReceiveMessageByGroup",Name,team.Id);
        }
        public async Task GetNamesByGroup()
        {
            var teams = _appDbContext.Teams.Include(x => x.Users).Select(x => new
            {
                teamId=x.Id,
                Users=x.Users.ToList()
            });
            await Clients.All.SendAsync("ReceiveNamesByGroup",teams);
        }
        public async Task RemoveToGroup(string teamName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, teamName);
        }




        public async override Task OnConnectedAsync()
        {
            ClientCount++;
            await Clients.All.SendAsync("ReceiveClientCount", ClientCount);
            await base.OnConnectedAsync();
        }
        public async override Task OnDisconnectedAsync(Exception? exception)
        {
            ClientCount--;
            await Clients.All.SendAsync("ReceiveClientCount", ClientCount);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
