//using AspNetCore.Identity.LiteDB;
//using AspNetCore.Identity.LiteDB.Models;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Extensions
{
    /*
    public static class LiteDBExtensions
    {
        public static LiteCollection<T> Collection<T>(this LiteDatabase db)
        {
            string name = typeof(T).Name.ToLower();
            if (db.CollectionExists(name)) return db.GetCollection<T>(name);
            else if (db.CollectionExists(name + 's')) return db.GetCollection<T>(name + 's');

            return db.GetCollection<T>();
        }
    }
    */
}
