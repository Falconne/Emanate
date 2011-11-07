using System;

namespace Emanate.Core.Configuration
{
    public class ConfigurationAttribute : Attribute
    {
        public string Name { get; private set; }
        public Type ViewType { get; private set; }

        public ConfigurationAttribute(string name, Type view)
        {
            Name = name;
            ViewType = view;
        }
    }
}