using System.Net;
using System.Text.RegularExpressions;
using c6_boss_web_framework.app.view_models;
using c6_boss_web_framework.framework;
using c6_boss_web_framework.framework.exceptions;
using c6_boss_web_framework.framework.requests;
using c6_boss_web_framework.framework.routers;
using c6_boss_web_framework.requests;

// using System.Web.Http;

namespace c6_boss_web_framework.app;

public class UserController(UserSystem userSystem) : Controller
{
    private static readonly Regex BearerTokenRegex =
        new(@"^Bearer [0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$",
            RegexOptions.Compiled);

    private readonly Dictionary<int, string> _userIdToToken = new();

    public override void Routes(IRouter router)
    {
        router.Post("/api/users/login", Login);
        router.Post("/api/users", Register);
        router.Patch("/api/users/{userId}", Rename);
        router.Get("/api/users", QueryUsers);
    }

    private async Task<IResponseEntity> Login(HttpListenerRequest request,
        Dictionary<string, string> parameters)
    {
        var loginRequest = await request.ReadBodyAsObject<LoginRequest>();
        try
        {
            var member = userSystem.Login(loginRequest.Email, loginRequest.Password);
            var response = new ResponseEntity();
            var token = GenerateBearerToken(response, member.Id);
            response.StatusCode = HttpStatusCode.OK;
            response.Body = UserResponse.ToViewModel(member, token);
            return response;
        }
        catch (ArgumentException)
        {
            throw new BadRequestException("Login's format incorrect.");
        }
    }

    private string GenerateBearerToken(ResponseEntity response, int userId)
    {
        var token = "Bearer " + Guid.NewGuid();
        response.AddHeader("Authorization", token);
        _userIdToToken[userId] = token;
        return token;
    }

    private async Task<IResponseEntity> Register(HttpListenerRequest request, Dictionary<string, string> parameters)
    {
        var registerRequest = await request.ReadBodyAsObject<RegisterRequest>();

        var member = userSystem.Register(registerRequest.Name, registerRequest.Email, registerRequest.Password);
        var response = new ResponseEntity
        {
            Body = UserResponse.ToViewModel(member, null)
        };
        return response;
    }

    private async Task<IResponseEntity> Rename(HttpListenerRequest request, Dictionary<string, string> parameters)
    {
        var bearerToken = request.Headers["Authorization"];
        var userId = int.Parse(parameters["userId"]);
        ValidateBearerToken(bearerToken);
        if (!_userIdToToken.TryGetValue(userId, out var value) || !value.Equals(bearerToken))
        {
            throw new OperationForbiddenException("Forbidden");
        }

        var renameRequest = await request.ReadBodyAsObject<RenameRequest>();
        userSystem.Rename(userId, renameRequest.NewName);

        return new ResponseEntity();
    }


    private Task<IResponseEntity> QueryUsers(HttpListenerRequest request, Dictionary<string, string> parameters)
    {
        ValidateBearerToken(request.Headers["Authorization"]);
        var queryParams = request.QueryString;
        var members = userSystem.Query(queryParams["keyword"]);
        return Task.FromResult<IResponseEntity>(new ResponseEntity
        {
            StatusCode = HttpStatusCode.OK,
            Body = members.Select(member => UserResponse.ToViewModel(member, null)).ToList()
        });
    }

    private static void ValidateBearerToken(string? authorizationHeader)
    {
        if (string.IsNullOrEmpty(authorizationHeader) || !BearerTokenRegex.IsMatch(authorizationHeader))
        {
            throw new UnauthorizedException("Can't authenticate who you are.");
        }
    }
}