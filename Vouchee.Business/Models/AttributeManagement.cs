using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Business.Models
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SkipAttribute : Attribute
    {
    }
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ChildAttribute : Attribute
    {
    }
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ExcludeAttribute : Attribute
    {
        public string Field { get; set; }
    }
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ContainAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class HiddenParamsAttribute : Attribute
    {
        public string Params { get; set; }

        public HiddenParamsAttribute(string parameters)
        {
            Params = parameters;
        }
    }
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class HiddenControllerAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class StringAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ImmutableAttribute : Attribute
    {
    }
    public class SortAttribute : Attribute
    {
    }
}
