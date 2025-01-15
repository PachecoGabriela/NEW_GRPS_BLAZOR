using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Blazor.Templates.Toolbar.ActionControls;
using DevExpress.ExpressApp.Blazor;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using GRPS_BLAZOR.Module.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Timers;
using Timer = System.Timers.Timer;

namespace GRPS_BLAZOR.Blazor.Server.Controllers.CustomLogon
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class TwoFactorAuthController : ViewController<DetailView>
    {
        SimpleAction SendCode { get; }
        SimpleAction VerifyCode { get; }
        public string VerificationCode { get; set; }
        ApplicationUser user;
        UnitOfWork unitOfWork;
        int counter;
        private static Timer timer;
        private CustomLogonParametersForStandardAuthentication CurrentLogonWindowObject { get; set; }
        IDataLayer dal;
        string connectionsStr;
        public TwoFactorAuthController()
        {
            TargetObjectType = typeof(CustomLogonParametersForStandardAuthentication);
            SendCode = new SimpleAction(this, "acSendLogonVerficationCode", "SendVerificationCode");
            SendCode.Caption = "Re send verfication code";
            SendCode.Execute += SendCode_Execute;
            SendCode.CustomizeControl += SendCode_CustomizeControl;

            VerifyCode = new SimpleAction(this, "acVerifyCode", "SendVerificationCode");
            VerifyCode.Caption = "Verify Code";
            VerifyCode.Execute += VerifyCode_Execute;
            VerifyCode.CustomizeControl += VerifyCode_CustomizeControl;
        }

        private void SendCode_CustomizeControl(object sender, CustomizeControlEventArgs e)
        {
            if (e.Control is DxToolbarItemSimpleActionControl control)
            {
                control.ToolbarItemModel.RenderStyle = DevExpress.Blazor.ButtonRenderStyle.Primary;
            }
        }

        private void VerifyCode_CustomizeControl(object sender, CustomizeControlEventArgs e)
        {
            if (e.Control is DxToolbarItemSimpleActionControl control)
            {
                control.ToolbarItemModel.Alignment = DevExpress.Blazor.ToolbarItemAlignment.Right;
                control.ToolbarItemModel.RenderStyle = DevExpress.Blazor.ButtonRenderStyle.Primary;
            }
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            CurrentLogonWindowObject = ((CustomLogonParametersForStandardAuthentication)View.CurrentObject);
        }
        private void SendCode_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (counter < 3)
            {
                counter++;
                MessageOptions message = new MessageOptions();
                message.Type = InformationType.Error;
                message.Duration = 3000;

                switch (string.IsNullOrEmpty(CurrentLogonWindowObject.UserName) ? "userempty" : string.Empty)
                {
                    case "userempty":
                        message.Message = "UserName field should not be empty";
                        Application.ShowViewStrategy.ShowMessage(message);
                        break;
                    default:
                        var userverificationinfo = CheckUserEmail(CurrentLogonWindowObject.UserName);
                        if (userverificationinfo != null)
                        {
                            SendVerificationCode(userverificationinfo.Email);
                            message.Type = InformationType.Success;
                            message.Message = "Verification Code sent successfully";
                            Application.ShowViewStrategy.ShowMessage(message);
                        }
                        break;
                }
            }
            else
            {
                Application.ShowViewStrategy.ShowMessage("You have made more than three attempts to request a new verification code. Please wait a few minutes to request a new one.", InformationType.Error, 5000, InformationPosition.Top);

                SendCode.Enabled["SendCodeAction"] = false;
                timer = new Timer(120000);
                timer.Elapsed += Timer_Elapsed;
                timer.Start();
            }
        }
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            counter = 0;
            SendCode.Enabled["SendCodeAction"] = true;
            timer.Stop();
            timer.Dispose();
        }
        public string GetConnectionString()
        {
            try
            {
                connectionsStr = ((BlazorApplication)Application).ServiceProvider.GetRequiredService<IConfiguration>().GetConnectionString("ConnectionString");
                return connectionsStr;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
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

        private async void VerifyCode_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (!string.IsNullOrEmpty(GetConnectionString()))
            {
                user = GetUser(CurrentLogonWindowObject);

                if (user != null)
                {
                    if (CurrentLogonWindowObject.VerificationCode == user.VerificationCode && !string.IsNullOrEmpty(CurrentLogonWindowObject.VerificationCode))
                    {
                        CurrentLogonWindowObject.Verified = true;
                        MessageOptions message = new MessageOptions();
                        message.Duration = 3000;
                        message.Message = "Verification Completed Successfully, Proceed to Log In";
                        message.Type = InformationType.Success;
                        Application.ShowViewStrategy.ShowMessage(message);
                        user.VerificationCode = String.Empty;
                        unitOfWork.CommitChanges();
                    }
                    else
                    {
                        MessageOptions message = new MessageOptions();
                        message.Duration = 3000;
                        message.Message = "Wrong Verification Code";
                        message.Type = InformationType.Error;
                        Application.ShowViewStrategy.ShowMessage(message);
                    }
                }
                else
                {
                    throw new UserFriendlyException("Provided username does not exist in that database");
                }
            }
        }

        private void SendVerificationCode(string receiverEmail)
        {
            IConfiguration configuration = (IConfiguration)((BlazorApplication)Application).ServiceProvider.GetService(typeof(IConfiguration));
            var port = configuration.GetSection("EmailConfiguration")["Port"];
            var smtp = configuration.GetSection("EmailConfiguration")["SMTP"];
            var UserCredentials = configuration.GetSection("EmailConfiguration")["UserCredentials"];
            var Password = configuration.GetSection("EmailConfiguration")["Password"];
            var EmailSubject = configuration.GetSection("EmailConfiguration")["EmailSubject"];
            var EmailBody = configuration.GetSection("EmailConfiguration")["EmailBody"];

            string code = GenerateVerificationCode(6);

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
            CurrentLogonWindowObject.Verified = false;
        }
        private UserVerficationInfo CheckUserEmail(string userName)
        {
            UserVerficationInfo verificationInfo = new UserVerficationInfo();

            if (!string.IsNullOrEmpty(GetConnectionString()))
            {
                user = GetUser(CurrentLogonWindowObject);

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
            public int CodeLenght { get; set; }
            public string Email { get; set; }
        }

    }
}
