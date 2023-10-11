﻿namespace SignalR.Api.Models
{
    public class Team
    {
        //team.user.add yapabilmel için user.add yapacaksan gerek yok 
        public Team()
        {
            Users = new List<User>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual  ICollection<User> Users { get; set; }
    }
}
