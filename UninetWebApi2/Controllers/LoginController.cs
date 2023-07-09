
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Uninet.APP.Interfaces;
using Uninet.DATA.Interfaces;
using Uninet.DATA.Services;
using Uninet.Domain.Models;
using Uninet.Domain.StoredProcedures.Responses;

//using Google.Apis.Auth;

namespace UninetWebApi2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly JwtConfiguration    _jwtConfiguration;
        private readonly IUserServiceApp _userServiceApp;
        private readonly IUninetInputAppService _uninetInputAppService;
        private readonly IjwtAppService _jwtAppService;
        private readonly ILogger<LoginController> _logger;
        
        public IConfiguration Configuration { get; }
        public LoginController( IUserServiceApp userServiceApp, ILogger<LoginController> logger, IConfiguration configuration, IjwtAppService jwtAppService, IUninetInputAppService uninetInputAppService)
        {
            
            _userServiceApp = userServiceApp;
            _logger = logger;
            Configuration = configuration;
         
            _jwtAppService = jwtAppService;
            _uninetInputAppService = uninetInputAppService;
        }

        [HttpPost("RefreshToken")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequest _refreshTokenRequest)
        {
            var refreshToken = _refreshTokenRequest.authenticationToken;

            // Verify the refresh token and retrieve the associated user ID
             int userId = await _jwtAppService.GetUserIdByRefreshToken(refreshToken);
          
            if (userId == 0) return BadRequest();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,userId.ToString())
            };
            var newJwtToken = await _jwtAppService.GenerateAccessToken(claims);
            
           
            //this line create the token and saves it in AdminUsers table
            var newRefreshToken = await _jwtAppService.GenerateRefreshToken(Convert.ToInt32(userId));
            
            //var result = await _jwtAppService.SaveRefreshToken(Convert.ToInt32(Userid), newRefreshToken);
            return Ok(new RegisterResult
            {
                //Role = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty,
                accessToken = newJwtToken,
                refreshToken = newRefreshToken,
                success = true
                
            }); ;


        }



        [HttpPost("LoginWithEmailPassword")]
        [AllowAnonymous]
        public async Task<ActionResult> LoginWithEmailPassword([FromBody] LoginWithEmailPasswordRequest _LoginWithEmailPasswordRequest)
        {

            var Res = await _userServiceApp.LoginWithEmailPasswordRequest(_LoginWithEmailPasswordRequest);  // _userService.LoginWithOtp(_LoginWithOtpRequest.Otp);
            if (Res != null)
            {
                var claims = new[]
                {


                  
                    new Claim(ClaimTypes.NameIdentifier,Res.Userid.ToString())
                };
                var token = await _jwtAppService.GenerateAccessToken(claims);
                var newRefreshToken = await _jwtAppService.GenerateRefreshToken(Res.Userid);
               
                _logger.LogInformation($"Userid [{Res.Userid.ToString()}] logged in the system.");

                //here i call 
               // string tt = await _uninetInputAppService.PullUserDatafromExternalSystem(Res.Userid);
                return Ok(new RegisterResult
                {

                    //Role = Res.Role.ToString(),
                    accessToken = token,
                    refreshToken = newRefreshToken,
                    success = true,
                    //Userid = Res.Userid
                });
            }
            else
            {
                return Ok(new RegisterResult
                {

                    //Role = "",
                    accessToken = "",
                    refreshToken = "",
                    success = false,
                    // Userid = 0
                });
            }

        }


       


        

        [HttpGet]
        public IActionResult Index()
        {
            var html = System.IO.File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "index.html"));
            return Content(html, "text/html");
        }

        //[HttpGet("google")]
        //public IActionResult GoogleLogin()
        //{
        //    var authenticationProperties = new AuthenticationProperties
        //    {
        //        RedirectUri = Url.Action(nameof(GoogleResponse), "Auth")
        //    };

        //    return Challenge(authenticationProperties, GoogleDefaults.AuthenticationScheme);
        //}

        //[HttpGet("google-response")]
        //public async Task<IActionResult> GoogleResponse()
        //{
        //    var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

        //    if (!result.Succeeded)
        //    {
        //        // Handle authentication failure
        //        return BadRequest();
        //    }

        //    // Handle authentication success
        //    return Ok(result.Principal.Claims);
        //}




        //[HttpGet("google")]
        //public IActionResult Google()
        //{
        //    var properties = new AuthenticationProperties
        //    {
        //        RedirectUri = Url.Action("GoogleCallback"),
        //        Items =
        //{
        //    { "scheme", "ExternalCookie" },
        //},
        //    };

        //    return Challenge(properties, "Google");
        //}

        //[HttpGet("google/callback")]
        //public async Task<ActionResult> GoogleCallback()
        //{
        //    var result = await HttpContext.AuthenticateAsync("ExternalCookie");

        //    if (result.Succeeded)
        //    {
        //        // User has been authenticated with Google
        //        // You can get the user's information from the ClaimsPrincipal
        //        // For example: result.Principal.FindFirst(ClaimTypes.Email).Value
        //    }
        //    else
        //    {
        //        // Authentication failed
        //    }

        //    return Redirect("/");
        //}



        //[HttpPost("signin")]
        //public async Task<ActionResult> SignIn([FromBody] GoogleSignInRequest request)
        //{
        //    var idToken = request.IdToken;

        //    var googleAuthSettings = new GoogleJsonWebSignature.ValidationSettings
        //    {
        //        Audience = new[] { "YOUR_GOOGLE_CLIENT_ID" } // replace with your client ID
        //    };
        //    var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, googleAuthSettings);

        //    // do something with the user's profile information, e.g. create a new user account or sign the user in

        //    return Ok();
        //}

        //public class GoogleSignInRequest
        //{
        //    public string IdToken { get; set; }
        //}


            


        [HttpGet("{id}")]
        [Authorize]
        public ActionResult<string> Get(int id)
        {
            return this.User.Identity.Name;
        }




        //[HttpGet("login")]
        //public IActionResult Login()
        //{
        //    var redirectUrl = Url.Action("GoogleResponse", "Auth", null, Request.Scheme);
        //    var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        //    return Challenge(properties, "Google");
        //}

        //[HttpGet("google-response")]
        //public async Task<IActionResult> GoogleResponse()
        //{
        //    var result = await HttpContext.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
        //    var claims = result.Principal.Claims.ToList();
        //    // Handle user authentication here
        //    return Ok();
        //}







        //[HttpPost]
        //[Route("LoginWithOtp")]
        //public async Task<ActionResult> LoginWithOtp([FromBody] LoginWithOtpRequest _LoginWithOtpRequest)
        //{
        //    if (_LoginWithOtpRequest is null)
        //    {
        //        return BadRequest("Invalid client request");
        //    }
        //    else
        //    {
        //        // Check if the user's credentials are valid
        //        LoginWithOtpResponse ValidUser = await RetunValidUser(_LoginWithOtpRequest);
        //        if (ValidUser is null)
        //            return Unauthorized();



        //        if (ValidUser != null)
        //        {
        //            var accessTokenString = _jwtAppService.GenerateAccessToken(ValidUser.Userid.ToString());
        //            // Create the refresh token
        //            var refreshToken = _jwtAppService.GenerateRefreshToken(ValidUser.Userid);
        //            return Ok(new
        //            {
        //                accessToken = accessTokenString,
        //                refreshToken = refreshToken
        //            });

        //        }

        //        // Return an error if the user's credentials are invalid
        //        return Unauthorized();
        //    }
        //}

     

       











        [AllowAnonymous]
        [HttpPost("LoginSendOtp")]
        public async Task<ActionResult> LoginSendOtp([FromBody] SendOtpRequest _sendOtpRequest)
        {

            bool ResSendOtpByIdAndPhone = await _userServiceApp.SendOtpByPhone(_sendOtpRequest);

            return Ok(ResSendOtpByIdAndPhone);



        }


      
    }



    public class JwtConfiguration
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string AccessTokenSecret { get; set; }
        public int AccessTokenExpirationMinutes { get; set; }
        public string RefreshTokenSecret { get; set; }
        public int RefreshTokenExpirationMinutes { get; set; }
    }
}
