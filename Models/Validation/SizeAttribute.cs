using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace Chop9ja.API.Models.Validation
{
    public class SizeAttribute : ValidationAttribute, IModelValidator
    {
        public long Min { get; set; } = -1;
        public long Max { get; set; } = -1;


        public override bool IsValid(object value)
        {
            long size = GetObjectSize(value);
            if (size < 0) return false;

            if (Min >= 0 && size < Min) return false;
            if (Max >= 0 && size > Max) return false;
            return true;
        }

        static long GetObjectSize(object obj)
        {
            long size = -1;
            try
            {
                using (var stream = new MemoryStream())
                {
                    new BinaryFormatter().Serialize(stream, obj);
                    size = stream.Length;
                }
            }
            catch (Exception ex)
            {
                Core.Log.Error(ex, "An error occured while getting the size of {0}", obj);
            }
            return size;
        }

        public IEnumerable<ModelValidationResult> Validate(ModelValidationContext context)
        {
            var errors = new List<ModelValidationResult>();
            long size = GetObjectSize(context.Model);
            if (size < 0) yield return new ModelValidationResult(context.ModelMetadata.Name, "Input size was too insignificant.");

            if (Min >= 0 && size < Min) yield return new ModelValidationResult(context.ModelMetadata.Name, $"Input size was less than the minimum required size of {Min}");
            if (Max >= 0 && size > Max) yield return new ModelValidationResult(context.ModelMetadata.Name, $"Input size was more than the maximum required size of {Max}");
        }
    }
}
