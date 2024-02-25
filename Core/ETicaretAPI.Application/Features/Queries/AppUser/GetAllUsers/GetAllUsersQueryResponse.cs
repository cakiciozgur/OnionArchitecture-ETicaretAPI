using ETicaretAPI.Application.DTOs.User;

namespace ETicaretAPI.Application.Features.Queries.AppUser.GetAllUsers
{
    public class GetAllUsersQueryResponse
    {
        public List<ListUser> Users { get; set; }
        public int TotalUserCount { get; set; }
    }
}