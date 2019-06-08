using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Models.ViewModels
{
    /// <summary>
    /// API Access Token Wrapper
    /// </summary>
    public class AccessTokenModel
    {
        /// <summary>
        /// JWT Access Token for user account.
        /// </summary>
        public string AccessToken { get; set; }
    }
}
