using Chop9ja.API.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Models.ViewModels.Agent
{
    public class AgentDepositViewModel : DepositViewModel
    {
        public Guid User { get; set; }
    }
}
