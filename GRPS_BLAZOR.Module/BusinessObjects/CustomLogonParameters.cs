using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using GRPS_BLAZOR.Module.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRPS_BLAZOR.Module.BusinessObjects
{
    [DomainComponent]
    [XafDisplayName("Login")]
    [Appearance("HideVerificationCode", AppearanceItemType.ViewItem, "[TwoFactorActivated] = False", TargetItems = nameof(VerificationCode), Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("HideVerificationActions", AppearanceItemType.Action, "[TwoFactorActivated] = False", TargetItems = "acSendLogonVerficationCode;acVerifyCode", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    public class CustomLogonParametersForStandardAuthentication : AuthenticationStandardLogonParameters, IServiceProviderConsumer, IUserNameParameter, IVerificationCodeParameter, IVerifiedParameter
    {
        IServiceProvider serviceProvider;

        string verificationCode;
        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string VerificationCode
        {
            get
            {
                return verificationCode;
            }
            set
            {
                verificationCode = value;
                RaisePropertyChanged("VerificationCode");
            }
        }
        bool twoFactor;
        [VisibleInDetailView(false)]
        public bool TwoFactorActivated
        {
            get
            {
                return twoFactor;
            }
            set
            {
                twoFactor = value;
                RaisePropertyChanged("TwoFactorActivated");
            }
        }
        bool deactivateLogin;
        [VisibleInDetailView(false)]
        public bool DeactivateLogin
        {
            get
            {
                return deactivateLogin;
            }
            set
            {
                deactivateLogin = value;
                RaisePropertyChanged("DeactivateLogin");
            }
        }
        bool verified;
        [VisibleInDetailView(false)]
        public bool Verified
        {
            get
            {
                return verified;
            }
            set
            {
                verified = value;
                RaisePropertyChanged("Verified");
            }
        }
        public void SetServiceProvider(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
    }
}
