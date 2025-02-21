using System.ComponentModel;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using DevExpress.Xpo;

namespace GRPS_BLAZOR.Module.BusinessObjects;

[MapInheritance(MapInheritanceType.ParentTable)]
[DefaultProperty(nameof(UserName))]
public class ApplicationUser : PermissionPolicyUser, ISecurityUserWithLoginInfo, ISecurityUserLockout {
    string userEmail;
    private int accessFailedCount;
    private DateTime lockoutEnd;

    public ApplicationUser(Session session) : base(session) { }


    [Browsable(false)]
    public int AccessFailedCount {
        get { return accessFailedCount; }
        set { SetPropertyValue(nameof(AccessFailedCount), ref accessFailedCount, value); }
    }

    [Browsable(false)]
    public DateTime LockoutEnd {
        get { return lockoutEnd; }
        set { SetPropertyValue(nameof(LockoutEnd), ref lockoutEnd, value); }
    }

    [Browsable(false)]
    [Aggregated, Association("User-LoginInfo")]
    public XPCollection<ApplicationUserLoginInfo> LoginInfo {
        get { return GetCollection<ApplicationUserLoginInfo>(nameof(LoginInfo)); }
    }

    IEnumerable<ISecurityUserLoginInfo> IOAuthSecurityUser.UserLogins => LoginInfo.OfType<ISecurityUserLoginInfo>();

    ISecurityUserLoginInfo ISecurityUserWithLoginInfo.CreateUserLoginInfo(string loginProviderName, string providerUserKey) {
        ApplicationUserLoginInfo result = new ApplicationUserLoginInfo(Session);
        result.LoginProviderName = loginProviderName;
        result.ProviderUserKey = providerUserKey;
        result.User = this;
        return result;
    }

    private string fullName;
    [Size(SizeAttribute.DefaultStringMappingFieldSize)]
    public string FullName
    {
        get => fullName;
        set => SetPropertyValue(nameof(FullName), ref fullName, value);
    }
    bool enableTwoFactorAuthentication;
    public bool EnableTwoFactorAuthentication
    {
        get => enableTwoFactorAuthentication;
        set => SetPropertyValue(nameof(EnableTwoFactorAuthentication), ref enableTwoFactorAuthentication, value);
    }
    string verificationCode;
    [Browsable(false)]
    [Size(SizeAttribute.DefaultStringMappingFieldSize)]
    public string VerificationCode
    {
        get => verificationCode;
        set => SetPropertyValue(nameof(VerificationCode), ref verificationCode, value);
    }
    string email;
    [Size(SizeAttribute.DefaultStringMappingFieldSize)]
    public string Email
    {
        get => email;
        set => SetPropertyValue(nameof(Email), ref email, value);
    }
}
