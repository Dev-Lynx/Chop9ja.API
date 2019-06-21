
using AspNetCore.Identity.Mongo.Model;
using AspNetCore.Identity.MongoDbCore.Models;
using Blueshift.EntityFrameworkCore.MongoDB.Annotations;
using Blueshift.Identity.MongoDB;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDbGenericRepository.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Models.Entities
{
    [MongoCollection("Roles")]
    [CollectionName("Roles")]
    public class UserRole : MongoIdentityRole
    {
        public UserRole() { }
        public UserRole(string name) : base(name) { }
    }
}
