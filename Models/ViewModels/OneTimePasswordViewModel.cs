using Chop9ja.API.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Models.ViewModels
{
    public class OneTimePasswordViewModel
    {
        public DateTime Created { get; set; }
        public DateTime Expires { get; set; }
        public OnePasswordType Kind { get; set; }
    }
}
