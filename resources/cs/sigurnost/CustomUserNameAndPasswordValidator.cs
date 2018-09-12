using System.IdentityModel.Selectors;
using System.ServiceModel;

namespace PG.Servisi.resources.cs.sigurnost
{
    public class CustomUserNameAndPasswordValidator : UserNamePasswordValidator
    {
        public override void Validate(string userName, string password)
        {
            if (userName == "johndoe" && password == "P@ssw0rd")
                return;

            throw new FaultException("Invalid Username and/or Password");
        }
    }
}