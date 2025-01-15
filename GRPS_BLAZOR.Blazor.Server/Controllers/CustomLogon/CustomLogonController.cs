using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Blazor;
using DevExpress.ExpressApp.Blazor.SystemModule;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using GRPS_BLAZOR.Module.BusinessObjects;
using GRPS_BLAZOR.Module.Interfaces;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Net;

namespace GRIPS.Blazor.Server.Controllers.CustomLogon
{
    public class CustomLogonController : BlazorLogonController
    {
        UnitOfWork unitOfWork;
        ApplicationUser user;
        string databaseName;
        IDataLayer dal;
        string connectionsStr;
        string? targetDataBaseName;
        //protected override void Accept(SimpleActionExecuteEventArgs args)
        //{
        //    //targetDataBaseName = ((BlazorApplication)Application).ServiceProvider.GetRequiredService<ILogonParameterProvider>().GetLogonParameters<IDatabaseNameParameter>().DatabaseName;
        //    //GetDBName();
        //    //if (string.IsNullOrEmpty(targetDataBaseName))
        //    //{
        //    //    throw new UserFriendlyException("The Company Code is not specified");
        //    //}

        //    //if (VerifyPassword((CustomLogonParametersForStandardAuthentication)args.CurrentObject))
        //    //{
        //    if (CheckTwoFactorEnabled((CustomLogonParametersForStandardAuthentication)args.CurrentObject))
        //    {
        //        if (!((CustomLogonParametersForStandardAuthentication)args.CurrentObject).Verified)
        //        {
        //            if (String.IsNullOrEmpty(((CustomLogonParametersForStandardAuthentication)args.CurrentObject).VerificationCode))
        //            {
        //                ((CustomLogonParametersForStandardAuthentication)args.CurrentObject).TwoFactorActivated = true;
        //                SendCode(databaseName, (CustomLogonParametersForStandardAuthentication)args.CurrentObject);
        //                Application.ShowViewStrategy.ShowMessage("Two factor authentication is enabled, a verification code has been sent to your email", InformationType.Success, 5000, InformationPosition.Bottom);
        //                return;
        //            }
        //            else
        //            {
        //                VerifyCode(databaseName);
        //                if (!((BlazorApplication)Application).ServiceProvider.GetRequiredService<ILogonParameterProvider>().GetLogonParameters<IVerifiedParameter>().Verified)
        //                {
        //                    throw new UserFriendlyException("Wrong Verification Code");
        //                }
        //            }
        //        }
        //    }
        //    //}
        //    //else
        //    //{
        //    //    throw new UserFriendlyException("Provided password does not match with the username");
        //    //}

        //    ((BlazorApplication)Application).ServiceProvider.GetRequiredService<IObjectSpaceProviderContainer>().Clear();

        //    base.Accept(args);
        //}

        public ApplicationUser GetUser(CustomLogonParametersForStandardAuthentication logonParameters)
        {
            dal = XpoDefault.GetDataLayer(GetConnectionString(), DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema);
            unitOfWork = new UnitOfWork(dal);
            user = unitOfWork.Query<ApplicationUser>().FirstOrDefault(u => u.UserName == logonParameters.UserName);

            if (user != null)
            {
                return user;
            }
            else
            {
                return null;
            }
        }

        public string GetDBName()
        {
            try
            {
                connectionsStr = ((BlazorApplication)Application).ServiceProvider.GetRequiredService<IConfiguration>().GetConnectionString(targetDataBaseName);
                string myCustomConection = connectionsStr?.Remove(0, 24);
                databaseName = new SqlConnectionStringBuilder(myCustomConection).InitialCatalog;

                return databaseName;
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException(ex.Message);
            }
        }

        public string GetConnectionString()
        {
            try
            {
                connectionsStr = ((BlazorApplication)Application).ServiceProvider.GetRequiredService<IConfiguration>().GetConnectionString(targetDataBaseName);
                return connectionsStr;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        //public bool VerifyPassword(CustomLogonParametersForStandardAuthentication logonParameters)
        //{
        //    if (!string.IsNullOrEmpty(connectionsStr))
        //    {
        //        user = GetUser(logonParameters);

        //        if (user != null)
        //        {
        //            var saltedPassword = (string)user?.GetMemberValue("StoredPassword");
        //            var verifiedPassword = PasswordCryptographer.VerifyHashedPasswordDelegate(saltedPassword, logonParameters.Password);

        //            if (verifiedPassword)
        //                return true;
        //            return false;
        //        }
        //        else
        //        {
        //            throw new UserFriendlyException("Provided username does not exist in that database");
        //        }
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
        private async void VerifyCode(string database)
        {
            string? username = ((BlazorApplication)Application).ServiceProvider.GetRequiredService<ILogonParameterProvider>().GetLogonParameters<IUserNameParameter>().UserName;

            string? verificationCode = ((BlazorApplication)Application).ServiceProvider.GetRequiredService<ILogonParameterProvider>().GetLogonParameters<IVerificationCodeParameter>().VerificationCode;

            bool verified = ((BlazorApplication)Application).ServiceProvider.GetRequiredService<ILogonParameterProvider>().GetLogonParameters<IVerifiedParameter>().Verified;

            if (!string.IsNullOrEmpty(connectionsStr))
            {
                if (verificationCode == user.VerificationCode && !string.IsNullOrEmpty(verificationCode))
                {
                    //verified = true;
                    ((BlazorApplication)Application).ServiceProvider.GetRequiredService<ILogonParameterProvider>().GetLogonParameters<IVerifiedParameter>().Verified = true;
                    user.VerificationCode = String.Empty;
                    unitOfWork.CommitChanges();
                }
            }
        }

        public bool CheckTwoFactorEnabled(CustomLogonParametersForStandardAuthentication logonParameters)
        {
            //if (!string.IsNullOrEmpty(connectionsStr))
            //{
                var user = GetUser(logonParameters);

                if (user != null)
                {
                    if (user.EnableTwoFactorAuthentication)
                        return true;
                    return false;
                }
                else
                {
                    throw new UserFriendlyException("Provided username does not exist in that database");
                }
            //}
            //else
            //{
            //    throw new UserFriendlyException("Problem with the connection to the datastore");
            //}

        }

        private void SendCode(string database, CustomLogonParametersForStandardAuthentication logonParameters)
        {
            string? username = ((BlazorApplication)Application).ServiceProvider.GetRequiredService<ILogonParameterProvider>().GetLogonParameters<IUserNameParameter>().UserName;

            MessageOptions message = new MessageOptions();
            message.Type = InformationType.Error;
            message.Duration = 3000;
            switch (string.IsNullOrEmpty(database) ? "dbempty" : string.Empty)
            {
                case "dbempty":
                    message.Message = "Database field should not be empty";
                    Application.ShowViewStrategy.ShowMessage(message);
                    break;
                default:
                    switch (string.IsNullOrEmpty(username) ? "userempty" : string.Empty)
                    {
                        case "userempty":
                            message.Message = "UserName field should not be empty";
                            Application.ShowViewStrategy.ShowMessage(message);
                            break;
                        default:
                            var userverificationinfo = CheckUserEmail(username, database, logonParameters);
                            if (userverificationinfo != null)
                            {
                                //SendCode.Caption = "Re send code";
                                SendVerificationCode(userverificationinfo.Email, database, logonParameters);
                                message.Type = InformationType.Success;
                                message.Message = "Verification Code sent successfully";
                                //Application.ShowViewStrategy.ShowMessage(message);
                            }
                            break;
                    }
                    break;
            }
        }
        private void SendVerificationCode(string receiverEmail, string database, CustomLogonParametersForStandardAuthentication logonParameters)
        {
            IConfiguration configuration = (IConfiguration)((BlazorApplication)Application).ServiceProvider.GetService(typeof(IConfiguration));
            var port = configuration.GetSection("EmailConfiguration")["Port"];
            var smtp = configuration.GetSection("EmailConfiguration")["SMTP"];
            var UserCredentials = configuration.GetSection("EmailConfiguration")["UserCredentials"];
            var Password = configuration.GetSection("EmailConfiguration")["Password"];
            var EmailSubject = configuration.GetSection("EmailConfiguration")["EmailSubject"];
            var EmailBody = configuration.GetSection("EmailConfiguration")["EmailBody"];

            string code = GenerateVerificationCode(6);

            string? username = ((BlazorApplication)Application).ServiceProvider.GetRequiredService<ILogonParameterProvider>().GetLogonParameters<IUserNameParameter>().UserName;

            if (!string.IsNullOrEmpty(connectionsStr))
            {
                if (user != null)
                {
                    user.VerificationCode = code;
                    unitOfWork.CommitChanges();
                }
            }

            SmtpClient smtpClient = new SmtpClient(smtp);
            smtpClient.Port = int.Parse(port);
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new NetworkCredential(UserCredentials, Password);
            MailMessage message = new MailMessage();
            message.From = new MailAddress(UserCredentials);
            message.To.Add(receiverEmail);
            message.Subject = EmailSubject + code;
            message.Body = EmailBody + code;
            smtpClient.Send(message);
        }

        private UserVerficationInfo CheckUserEmail(string userName, string database, CustomLogonParametersForStandardAuthentication logonParameters)
        {
            UserVerficationInfo verificationInfo = new UserVerficationInfo();

            if (!string.IsNullOrEmpty(connectionsStr))
            {
                //var user = GetUser(logonParameters);

                if (user != null)
                {
                    bool hasEmail = user.Email != null;
                    verificationInfo.Email = hasEmail ? user.Email : throw new UserFriendlyException("Ther user must have an email");
                    //verificationInfo.CodeLenght = user.VerificationCodeLength > 0 ? user.VerificationCodeLength : 6;
                }
                else
                {
                    throw new UserFriendlyException("Provided username does not exist in that database");
                }
            }
            else
            {
                throw new UserFriendlyException("Problem with the connection to the datastore");
            }

            return verificationInfo;
        }

        private string GenerateVerificationCode(int lenght)
        {
            Random random = new Random();
            const string allowedCharacters = "0123456789";

            char[] buffer = new char[lenght];

            for (int i = 0; i < lenght; i++)
            {
                int randomIndex = random.Next(allowedCharacters.Length);
                buffer[i] = allowedCharacters[randomIndex];
            }
            string verificationCode = new string(buffer);
            return verificationCode;
        }

        private class UserVerficationInfo
        {
            //public int CodeLenght { get; set; }
            public string Email { get; set; }
        }
    }
}
