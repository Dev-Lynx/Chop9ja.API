using AutoMapper;
using Chop9ja.API.Data;
using Chop9ja.API.Extensions.UnityExtensions;
using Chop9ja.API.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Controllers
{
    /// <summary>
    /// Manages User Bets
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [AutoBuild]
    public class BetController
    {
        #region Properties

        #region Services
        [DeepDependency]
        ILogger Logger { get; }

        [DeepDependency]
        UserManager<User> UserManager { get; }

        [DeepDependency]
        MongoDataContext DataContext { get; }

        [DeepDependency]
        IMapper Mapper { get; }
        #endregion

        #endregion

        #region Methods

        #region RESTful API Calls
        
        #endregion

        #endregion
    }
}
