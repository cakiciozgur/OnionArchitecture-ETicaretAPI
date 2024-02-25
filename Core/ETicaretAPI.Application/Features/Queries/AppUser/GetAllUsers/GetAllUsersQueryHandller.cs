using ETicaretAPI.Application.Abstractions.Services.User;
using ETicaretAPI.Application.DTOs.User;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Queries.AppUser.GetAllUsers
{
    public class GetAllUsersQueryHandller : IRequestHandler<GetAllUsersQueryRequest, GetAllUsersQueryResponse>
    {
        readonly IUserService _userService;

        public GetAllUsersQueryHandller(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<GetAllUsersQueryResponse> Handle(GetAllUsersQueryRequest request, CancellationToken cancellationToken)
        {
            (List<ListUser> users, int totalUserCount) allUsers = await _userService.GetAllUsersAsync(request.Page, request.Size);

            return new GetAllUsersQueryResponse
            {
                TotalUserCount = allUsers.totalUserCount,
                Users = allUsers.users
            };
        }
    }
}
