using Microsoft.AspNetCore.SignalR;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using BMWDomain.Entities;
using BMWDomain.interfaces;

namespace BMW.ASP.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IChatContainer _chatContainer;
        private readonly IUserContainer _userContainer;
        private readonly IClubContainer _clubContainer;

        public ChatHub(IChatContainer chatContainer, IUserContainer userContainer, IClubContainer clubContainer)
        {
            _chatContainer = chatContainer;
            _userContainer = userContainer;
            _clubContainer = clubContainer;
        }


        public async Task SendMessage(string message, int userId, int clubId)
        {
            try
            {
                var user = _userContainer.GetUserById(userId);
                string username = user.Name;
                
                // Save the message to the database
                _chatContainer.SendMessage(new Message(0, userId, clubId, message));
                
                // Send the message to all clients in the group corresponding to the club ID
                await Clients.Group(clubId.ToString()).SendAsync("ReceiveMessage", username, message);


            }
            catch (SqlException)
            {
                await Clients.Caller.SendAsync("ReceiveError", "An error occurred while sending the message.");
            }
            catch (Exception)
            {
                await Clients.Caller.SendAsync("ReceiveError", "An unexpected error occurred while sending the message.");
            }
        }

        public async Task JoinClubGroup(int clubId, int userId)
        {
            var club = _clubContainer.GetClubDetail(clubId);
            bool isMember = club.Members!.Exists(membership => membership.UserId == userId);
            
            // Add the connection to the group corresponding to the club ID
            if (isMember)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, clubId.ToString());
            }
            else
            {
                await Clients.Caller.SendAsync("ReceiveError", "You are not a member of this club.");
            }

        }

        public async Task LeaveClubGroup(int clubId)
        {
            // Remove the connection from the group corresponding to the club ID
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, clubId.ToString());
        }
    }
}
