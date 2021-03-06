using System;

namespace Emanate.Service.Admin
{
    public class ConfigurationProperty
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public bool IsPassword { get; set; }
        public string Key { get; set; }
        public object Value { get; set; }
        public string FriendlyName { get; set; }
    }
}