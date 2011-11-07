using System;
using System.Windows.Controls;

namespace Emanate.Service.Admin
{
    public class ConfigurationInfo
    {
        public string Name { get; set; }
        private readonly Type viewType;

        public ConfigurationInfo(string name, Type viewType)
        {
            Name = name;
            this.viewType = viewType;
        }

        private UserControl view;
        public UserControl View
        {
            get
            {
                if (view == null)
                    view = (UserControl)Activator.CreateInstance(viewType);

                return view;
            }
        }
    }
}