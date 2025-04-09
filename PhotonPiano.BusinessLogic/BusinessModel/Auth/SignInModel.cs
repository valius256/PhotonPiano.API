using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotonPiano.BusinessLogic.BusinessModel.Auth
{
    public record SignInModel(
        string Email,
        string Password
    );
}
