using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Uninet.APP.Interfaces;
using Uninet.APP.Services;
using Uninet.Domain.Models;
using Uninet.Domain.StoredProcedures.Requests;
using Uninet.Domain.StoredProcedures.Responses;
using static System.Net.WebRequestMethods;

namespace UninetWebApi2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly IUserServiceApp _userServiceApp;
        private readonly ILogger<RegisterController> _logger;
        private readonly IMailassist _mailasist;
        private readonly IjwtAppService _jwtAppService;
        public IConfiguration Configuration { get; }
        public RegisterController( IUserServiceApp userServiceApp, ILogger<RegisterController> logger, IMailassist mailassist, IjwtAppService jwtAppService, IConfiguration configuration)
        {
           
            _userServiceApp = userServiceApp;
            _logger = logger;
            _mailasist= mailassist;
            _jwtAppService= jwtAppService;
            Configuration = configuration;
        }



   

        //Q1 to Q3
        [Authorize]
        [HttpPost("RegisterBusinessToUser")]
        public async Task<ActionResult> RegisterBusinessToUser([FromBody] List<BusinessRequest> RegisterUserReq)
        {
            try
            {
                // Retrieve the User ID from the JWT token
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var UserBusinesses = new UserBusinesses
                {
                    BusinessRequests = RegisterUserReq,
                    Userid =Convert.ToInt32(userId)
                };
                var res = await _userServiceApp.RegisterBusinessToUser(UserBusinesses);


                return Ok(res);
            }
            catch (Exception ex)
            {
                return Ok(false);
            }



        }


        //[AllowAnonymous]
        //[HttpPost("VerifyEmailLink")]
        //public async Task<ActionResult> VerifyEmailLink()
        //{
        //    try
        //    {
        //        string userguid =  HttpContext.Request.Query["userguid"];
        //        if (string.IsNullOrEmpty(userguid))
        //        {
        //            return BadRequest(Ok(false));
        //        }
        //            else
        //        {
        //           var res=    await _userServiceApp.VerifyEmailLink(userguid);
        //            return Ok(res);
        //        }
        //    }
        //    catch (Exception ex) 
        //    {
        //        return Ok(false);
        //    }
        //}

        //GetExternalCustomizedFieldByExternaLSystemID  ''ExternalSystemCustomizeFieldResult
        [Authorize]
        [HttpPost("SaveExternalCustomizedExternalSystemId")]
        public async Task<ActionResult> SaveExternalCustomizedExternalSystemId([FromBody] SpInputExternalSystemCompanyDetails spInputExternalSystemCompanyDetails)
        {
            // Retrieve the User ID from the JWT token
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Create the JSON object
           
            var res = await _userServiceApp.SaveExternalCustomizedExternalSystemId(spInputExternalSystemCompanyDetails, userId.ToString());

            return Ok(res);
        }

        [Authorize]
        [HttpGet("GetExternalSystem")]
        public async Task<IActionResult> GetExternalSystem()
        {
            var res = await _userServiceApp.GetExternalSystems();
            return Ok(res);
        }

        //[HttpGet("GetToken")]
        //public IActionResult GetToken()
        //{
        //    var secretKey = Configuration["jwtTokenConfig:secret"]; // Replace with your actual secret key
        //    var issuer = "https://localhost:7202/api/"; // Replace with your actual issuer URL
        //    var audience = "your-audience"; // Replace with your actual audience

        //    var claims = new[]
        //    {
        //        new Claim(ClaimTypes.Name, "testuser"),
        //        new Claim(ClaimTypes.Role, "user")
        //    };

        //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        //    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //    var token = new JwtSecurityToken(
        //        issuer: issuer,
        //        audience: audience,
        //        claims: claims,
        //        expires: DateTime.Now.AddHours(1), // Set the token expiration time
        //        signingCredentials: credentials
        //    );

        //    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        //    return Ok(new { Token = tokenString });
        //}
        [Authorize]
        [HttpGet("Test")]
        public async Task<IActionResult> Test()
        {
           

            return Ok(true);
            
        }

        [Authorize]
        [HttpGet("GetExternalCustomizedFieldByExternaLSystemID")]
        public async Task<IActionResult> GetExternalCustomizedFieldByExternaLSystemID(int ExternalSystemId)
        {
            var res= await _userServiceApp.GetExternalCustomizedFieldByExternaLSystemID(ExternalSystemId);
            return Ok(res);
        }
        public static string DecryptUserId(string encryptedUserId, string key, string iv)
        {
            byte[] decryptedBytes;
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.IV = Encoding.UTF8.GetBytes(iv);

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                byte[] encryptedBytes = Convert.FromBase64String(encryptedUserId);
                decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

                decryptor.Dispose();
            }

            return Encoding.UTF8.GetString(decryptedBytes);
        }
        public static byte[] GenerateSalt(int sizeInBytes)
        {
            byte[] salt = new byte[sizeInBytes];
            using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetBytes(salt);
            }
            return salt;
        }
        public static byte[] GenerateAesKey(string passphrase, byte[] salt)
        {
            const int keySizeInBits = 256;
            const int keySizeInBytes = keySizeInBits / 8;

            using (Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(passphrase, salt))
            {
                return deriveBytes.GetBytes(keySizeInBytes);
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

        public static string DecryptUserId(string encryptedUserId, string key, byte[] iv)
        {
            byte[] decryptedBytes;
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = StringToByteArray(key);
                aesAlg.IV = iv;
                aesAlg.Padding = PaddingMode.PKCS7; // Explicitly set the padding mode

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                byte[] encryptedBytes = Convert.FromBase64String(encryptedUserId);
                decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

                decryptor.Dispose();
            }

            return Encoding.UTF8.GetString(decryptedBytes);
        }



        public static byte[] GenerateRandomIV(int sizeInBytes)
        {
            byte[] iv = new byte[sizeInBytes];
            using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetBytes(iv);
            }
            return iv;
        }
        private async Task<LoginWithOtpResponse> RetunValidUser(LoginWithOtpRequest _LoginWithOtpRequest)
        {
            // Check if the user's credentials are valid
            // You can replace this with your own logic to validate the user's credentials
            //byte[] salt = GenerateSalt(16);
           
            string iv = Configuration["EncryptedUserId:iv"];
            byte[] ivBytes = Encoding.UTF8.GetBytes(iv);

           



            string DecryptedUserId = DecryptUserId(_LoginWithOtpRequest.EncryptedUser, Configuration["EncryptedUserId:key"], ivBytes);
            var Res = await _userServiceApp.RegisterWithOtpAndEncryptedUser(_LoginWithOtpRequest.Otp, DecryptedUserId);
            
            return Res;
           

        }
        [HttpPost("RegisterWithOtp")]
        [AllowAnonymous]
        public async Task<ActionResult> RegisterWithOtp([FromBody] LoginWithOtpRequest _LoginWithOtpRequest)
        {

            var Res = await RetunValidUser(_LoginWithOtpRequest);// _userService.LoginWithOtp(_LoginWithOtpRequest.Otp);
            if (Res != null)
            {
                if (Res.verified)
                {
                    var claims = new[]
                    {


                   // new Claim(ClaimTypes.Name,Res.FirstName.ToString()),
                    new Claim(ClaimTypes.NameIdentifier,Res.userId.ToString())
                };
                    var token = await _jwtAppService.GenerateAccessToken(claims);
                    var newRefreshToken = await _jwtAppService.GenerateRefreshToken(Convert.ToInt32(Res.userId));

                    _logger.LogInformation($"Userid [{Res.userId.ToString()}] logged in the system.");


                    //string tt = await _uninetInputAppService.PullUserDatafromExternalSystem(Res.Userid);



                    return Ok(new RegisterResult
                    {

                        //Role = Res.Role.ToString(),
                        accessToken = token,
                        refreshToken = newRefreshToken,
                        success = true,
                        //Userid = Res.Userid
                        verified= Res.verified
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
                        verified = Res.verified
                        // Userid = 0
                    });


                }
            }
            else
            {
                return Ok(new RegisterResult
                {

                    //Role = "",
                    accessToken = "",
                    refreshToken = "",
                    success = false,
                    verified = false
                    // Userid = 0
                });
            }

        }


        [HttpPost("ResentOtp")]
        public async Task<ActionResult> ResentOtp([FromBody] ResentOtpRequest resentOtpRequest)
        {
            try
            {
                SendOtpViaMailResponse sendsmtpmailres = new SendOtpViaMailResponse();
                string iv = Configuration["EncryptedUserId:iv"];
                byte[] ivBytes = Encoding.UTF8.GetBytes(iv);
                string DecryptedUserId = DecryptUserId(resentOtpRequest.EncryptedUserId, Configuration["EncryptedUserId:key"], ivBytes);
                var ReturnUser = await _userServiceApp.ResendOtp(resentOtpRequest,Convert.ToInt32(DecryptedUserId));
                if (ReturnUser.Success)
                {
                    var res = await _userServiceApp.SaveIndicationOfSentApprovalMailToCustomer(ReturnUser.userid, ReturnUser.otp);
                    var resentotpresponse = new ResentOtpResponse
                    {

                        sucess = ReturnUser.Success,
                        textResponse = ReturnUser.Desc,
                        
                        
                    };
                    return Ok(resentotpresponse);
                }
                else
                {
                    var resentotpresponse = new ResentOtpResponse
                    {

                        sucess = false,
                        textResponse = ReturnUser.Desc,
                       
                    };
                    return Ok(resentotpresponse);
                }
               
            }
            catch (Exception ex)
            {
                var resentotpresponse = new ResentOtpResponse
                {

                    sucess = false,
                    textResponse = ex.Message
                   
                };
                return Ok(resentotpresponse);
            }
        }
        private  string EncryptUserId(string userId, string key, byte[] iv)
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

        [HttpPost("Register")]
        public async Task<ActionResult> Register([FromBody] RegisterUserRequest RegisterUserReq)
        {
            //this register function create a new row in AdminUsers table
            //send email to the user to verify
            //update table AdminUsers with datetime and sent email indication
            try
            {
                ApprovalMailIndication res = new ApprovalMailIndication();
                SendOtpViaMailResponse sendsmtpmailres = new SendOtpViaMailResponse();
                /*
                     public  class ReturnRegisterUser
                    {
                        public int Userid { get; set; }
                        public int UserStatusIndication { get; set; }   
                    }
                */

                var ReturnUser = await _userServiceApp.RegisterUser(RegisterUserReq);//0 not exist //1 userexist //null exception

                if (ReturnUser == null)//fail on exception
                {
                    var RegisterResult = new RegisterResponse
                    {

                        sucess = false,
                        textResponse = "User Failed to Register",
                        encryptedUser = "",
                        verified= false
                    };
                    return Ok(RegisterResult);
                }
                else if (ReturnUser.UserStatusIndication == 1)//&& ReturnUser.verified==false
                {
                    //sendsmtpmailres = await _mailasist.sendsmtpmail("סיסמה חד פעמית UNINET ", "eyalbmma@gmail.com", RegisterUserReq.Email, RegisterUserReq.TemplateId, RegisterUserReq.Lang);
                   // res = await _userServiceApp.SaveIndicationOfSentApprovalMailToCustomer(ReturnUser.Userid, sendsmtpmailres.OTP);
                    var RegisterResult = new RegisterResponse
                    {

                        sucess = false,
                        textResponse = "User already Exist ",
                        encryptedUser = res.EncryptedUserid,
                        verified= ReturnUser.verified
                    };
                    return Ok(RegisterResult);
                }
                else
                {
                    string iv = Configuration["EncryptedUserId:iv"];
                    byte[] ivBytes = Encoding.UTF8.GetBytes(iv);
                    string encryptedUserId = EncryptUserId(ReturnUser.Userid.ToString(), Configuration["EncryptedUserId:key"], ivBytes);
                    sendsmtpmailres = await _mailasist.sendsmtpmail("סיסמה חד פעמית UNINET ", "eyalbmma@gmail.com", RegisterUserReq.Email, RegisterUserReq.TemplateId, RegisterUserReq.Lang, encryptedUserId);
                    res = await _userServiceApp.SaveIndicationOfSentApprovalMailToCustomer(ReturnUser.Userid, sendsmtpmailres.OTP);
                    var RegisterResult = new RegisterResponse
                    {

                        sucess = true,
                        textResponse = "User Succesfuly registered , Otp Sent For Verification",
                        encryptedUser = res.EncryptedUserid,
                        verified = ReturnUser.verified,
                        otp= sendsmtpmailres.OTP
                    };
                    return Ok(RegisterResult);

                }




            }
            catch (Exception ex)
            {
                var RegisterResult = new RegisterResponse
                {

                    sucess = false,
                    textResponse = "User failed to  registered and otp wasnt sent",
                    encryptedUser = ""
                };
                return Ok(false);
            }



        }


    }
}
