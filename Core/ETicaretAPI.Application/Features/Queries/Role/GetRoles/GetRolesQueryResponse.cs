namespace ETicaretAPI.Application.Features.Queries.Role.GetRoles
{
    public class GetRolesQueryResponse
    {

        public IDictionary<string,string> Roles { get; set; }
        public int TotalRoleCount { get; set; }
    }
}