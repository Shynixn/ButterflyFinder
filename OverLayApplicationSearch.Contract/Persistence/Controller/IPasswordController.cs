using OverLayApplicationSearch.Contract.Persistence.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace OverLayApplicationSearch.Contract.Persistence.Controller
{
    public interface IPasswordController : IDatabaseController<IPa>
    {
        /// <summary>
        /// Creates a new pa
        /// </summary>
        /// <returns>create</returns>
        IPa Create();

        /// <summary>
        /// Generates a new random password with 16 letter.
        /// </summary>
        /// <returns>password</returns>
        SecureString GeneratePassword();
    }
}
