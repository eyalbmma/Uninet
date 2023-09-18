
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Twilio.Jwt.AccessToken;
using Uninet.APP.Interfaces;
using Uninet.APP.Services;
using Uninet.DATA.Interfaces;
using Uninet.DATA.Services;
using Uninet.Domain.Entities;
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
                accessToken = newJwtToken.Accesstoken,
                refreshToken = newRefreshToken.RefreshToken,
                success = true
                
            }); ;


        }

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest forgotPasswordRequest)
        {
            try
            {

                var  res = await _userServiceApp.ForgotPassword(forgotPasswordRequest);
                return Ok(res);
                
               
          
            }
            catch (Exception ex)
            {

                var ForgotPasswordResponse = new ForgotPasswordResponse
                {
                    Success = false,
                    textResponse = forgotPasswordRequest.Lang == 1 ? "there was an error reseting your password" : "ארעה שגיאה באיפוס הסיסמה"

                };
                return Ok(ForgotPasswordResponse);
            }
        }



        [HttpPost("GoogleSignIn")]
        public async Task<IActionResult> GoogleSignIn(GoogleSignInModel googlesignInrequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                GoogleSigninResponse googleresponse = await _userServiceApp.GoogleSignIn(googlesignInrequest);

                if (googleresponse.Success)
                {
                    var claims = new[]
                    {

                         new Claim(ClaimTypes.NameIdentifier,googleresponse.UserId.ToString())
                     };
                    var token = await _jwtAppService.GenerateAccessToken(claims);
                    var newRefreshToken = await _jwtAppService.GenerateRefreshToken(googleresponse.UserId);

                    _logger.LogInformation($"Userid [{googleresponse.UserId.ToString()}] logged in the system.");

                    string iv = Configuration["EncryptedUserId:iv"];
                    byte[] ivBytes = Encoding.UTF8.GetBytes(iv);
                    string encryptedUserId = EncryptUserId(googleresponse.UserId.ToString(), Configuration["EncryptedUserId:key"], ivBytes);
                    var req1_4 = new Q1_Q4_Request
                    {
                        Userid = googleresponse.UserId

                    };

                    var res1_4 = await _userServiceApp.GetQ1_Q4_Indication(req1_4);
                    

                    return Ok(new RegisterResult
                    {

                        //Role = Res.Role.ToString(),
                        accessToken = token.Accesstoken,
                        refreshToken = newRefreshToken.RefreshToken,
                        success = googleresponse.Success,
                        Q1_Q2_InidicationRes = res1_4.Q1_Q2_InidicationRes,
                        Q3_InidicationRes = res1_4.Q3_InidicationRes,
                        verified = googleresponse.verified,
                        EncryptedUserId = encryptedUserId,
                        textResponse = googleresponse.textResponse,
                        BusinessId= res1_4.BusinessID,
                        Fullname= res1_4.Fullname,
                        AccesstokenExpiredTime = token.ExpirationDateAccesstoken,
                        RefreshTokenExpiredTime = newRefreshToken.RefreshTokenExpireTime
                        //Userid = Res.Userid
                    });


                }
                else
                {
                    return Ok(new RegisterResult
                    {

                        //Role = Res.Role.ToString(),
                        accessToken = null,
                        refreshToken = null,
                        success = googleresponse.Success,
                        Q1_Q2_InidicationRes =false,
                        Q3_InidicationRes = false,
                        verified = false,
                        EncryptedUserId = null,
                        textResponse = googleresponse.textResponse,
                        BusinessId =null,
                        Fullname = "",
                        AccesstokenExpiredTime = null,
                        RefreshTokenExpiredTime = null

                        //Userid = Res.Userid
                    });
                }



               


            }
            catch (Exception ex)
            {
                // Handle the exception and return an appropriate response
                  return Ok(new RegisterResult
                {

                    //Role = Res.Role.ToString(),
                    accessToken = null,
                    refreshToken = null,
                    success = false,
                    Q1_Q2_InidicationRes = false,
                    Q3_InidicationRes = false,
                    verified = false,
                    EncryptedUserId = null,
                    textResponse = googlesignInrequest.lang == 1 ? "verfication failed9"+ex.InnerException +ex.Message : " האימות נכשל ",
                      Fullname = "",
                      AccesstokenExpiredTime = null,
                      RefreshTokenExpiredTime = null
                      //Userid = Res.Userid
                  });
            }
        }


        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequestcs resetpasswordrequest)
        {
            try
            {
                var Res = await _userServiceApp.ResetPassword(resetpasswordrequest);
                return Ok(Res);

            }
            catch (Exception ex) {

                var ResResetPassword = new ResetPasswordReponse
                {
                    success = false,
                    textResponse = resetpasswordrequest.Lang == 1 ? "an eror occured" : "ארעה שדיאה הסיסמה לא אופסה "
                };
                return Ok(ResResetPassword);
            }
        }
        public static byte[] StringToByteArray(string hex)
        {
            int length = hex.Length / 2;
            byte[] bytes = new byte[length];
            for (int i = 0; i < length; i++)
            {
                bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return bytes;
        }
        private string EncryptUserId(string userId, string key, byte[] iv)
        {
            byte[] encryptedBytes;
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = StringToByteArray(key);
                aesAlg.IV = iv;
                aesAlg.Padding = PaddingMode.PKCS7; // Set the padding mode

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                byte[] userIdBytes = Encoding.UTF8.GetBytes(userId);
                encryptedBytes = encryptor.TransformFinalBlock(userIdBytes, 0, userIdBytes.Length);

                encryptor.Dispose();
            }

            return Convert.ToBase64String(encryptedBytes);
        }

        [HttpPost("LoginWithEmailPassword")]
        [AllowAnonymous]
        public async Task<ActionResult> LoginWithEmailPassword([FromBody] LoginWithEmailPasswordRequest _LoginWithEmailPasswordRequest)
        {

            var Res = await _userServiceApp.LoginWithEmailPasswordRequest(_LoginWithEmailPasswordRequest);  // _userService.LoginWithOtp(_LoginWithOtpRequest.Otp);
            if (Res!=null)
            {
               if (Res.Userid != 0)
                {
                    var claims = new[]
                   {



                        new Claim(ClaimTypes.NameIdentifier,Res.Userid.ToString())
                    };
                    AccesstokenReturnObj token = await _jwtAppService.GenerateAccessToken(claims);
                    var newRefreshToken = await _jwtAppService.GenerateRefreshToken(Res.Userid);

                    _logger.LogInformation($"Userid [{Res.Userid.ToString()}] logged in the system.");

                    string iv = Configuration["EncryptedUserId:iv"];
                    byte[] ivBytes = Encoding.UTF8.GetBytes(iv);
                    string encryptedUserId = EncryptUserId(Res.Userid.ToString(), Configuration["EncryptedUserId:key"], ivBytes);

                    string Resmessage = "";
                    if (_LoginWithEmailPasswordRequest.Lang == 1)
                    {
                        Resmessage = "User Login Succesfully";
                    }
                    else
                    {
                        Resmessage = "המשתמש התחבר בהצלחה";
                    }





                   
                   





                    return Ok(new RegisterResult
                    {

                        //Role = Res.Role.ToString(),
                        accessToken = token.Accesstoken,
                        refreshToken = newRefreshToken.RefreshToken,
                        success = Res.Userid != 0 ? true : false,
                        Q1_Q2_InidicationRes = Res.Q1_Q2_InidicationRes,
                        Q3_InidicationRes = Res.Q3_InidicationRes,
                        verified = Res.verified,
                        EncryptedUserId = encryptedUserId,
                        textResponse = Resmessage,
                        BusinessId = Res.BusinessID,
                        Fullname = Res.FullName,
                        AccesstokenExpiredTime = Convert.ToDateTime(token.ExpirationDateAccesstoken),
                        RefreshTokenExpiredTime = newRefreshToken.RefreshTokenExpireTime
                        //Userid = Res.Userid
                    });


                }
               else
                {
                    string Resmessage = "";
                    if (_LoginWithEmailPasswordRequest.Lang == 1)
                    {
                        Resmessage = "user failed to login";
                    }
                    else
                    {
                        Resmessage = "המשתמש נכשל בהתחברות";
                    }
                    return Ok(new RegisterResult
                    {




                        //Role = "",
                        accessToken = "",
                        refreshToken = "",
                        success = false,
                        Q1_Q2_InidicationRes = false,
                        Q3_InidicationRes = false,
                        verified = false,
                        EncryptedUserId = "",
                        textResponse = Resmessage,
                        BusinessId = Res.BusinessID,
                        Fullname = Res.FullName,
                        AccesstokenExpiredTime = null,
                        RefreshTokenExpiredTime = null
                        // Userid = 0
                    });
                }

                
                   
                
               
            }
            else
            {
                string Resmessage = "";
                if (_LoginWithEmailPasswordRequest.Lang == 1)
                {
                    Resmessage = "user failed to login";
                }
                else
                {
                    Resmessage = "המשתמש נכשל בהתחברות";
                }


                return Ok(new RegisterResult
                {

                  


                    //Role = "",
                    accessToken = "",
                    refreshToken = "",
                    success = false,
                    Q1_Q2_InidicationRes = false,
                    Q3_InidicationRes = false,
                    verified = false,
                    EncryptedUserId = "",
                    textResponse = Resmessage,
                    BusinessId = Res.BusinessID,
                    Fullname = Res.FullName,
                     AccesstokenExpiredTime =null,
                    RefreshTokenExpiredTime = null
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









        [HttpPost("Logout")]
        [Authorize]
        public async Task<IActionResult> Logout(LogOutRequest logoutrequest)
        { 
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

           
            await HttpContext.SignOutAsync(JwtBearerDefaults.AuthenticationScheme);



            return Ok(new { textResponse =  logoutrequest.Lang==1? "Logged out successfully.":"יצאת בהצלחה מהמערכת" });
        }





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
