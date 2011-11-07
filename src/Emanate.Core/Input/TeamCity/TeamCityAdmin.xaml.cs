using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Emanate.Core.Input.TeamCity
{
    /// <summary>
    /// Interaction logic for TeamCityAdmin.xaml
    /// </summary>
    public partial class TeamCityAdmin
    {
        public TeamCityAdmin()
        {
            Projects = new ObservableCollection<TeamCityItem>(CreateFoos());
            InitializeComponent();
        }

        private static List<TeamCityItem> CreateFoos()
        {
            var projects = new List<TeamCityItem>
                       {
                           new TeamCityItem("Project 1")
                               {
                                   Children =
                                       {
                                           new TeamCityItem("Build 1"),
                                           new TeamCityItem("Build 2"),
                                           new TeamCityItem("Build 3"),
                                       }
                               },
                           new TeamCityItem("Project 2")
                               {
                                   Children =
                                       {
                                           new TeamCityItem("Build 4"),
                                           new TeamCityItem("Build 5"),
                                           new TeamCityItem("Build 6"),
                                       }
                               },
                           new TeamCityItem("Project 3")
                               {
                                   Children =
                                       {
                                           new TeamCityItem("Build 7"),
                                           new TeamCityItem("Build 8"),
                                           new TeamCityItem("Build 9"),
                                       }
                               },
                       };

            foreach (var project in projects)
            {
                project.Initialize();
            }
            return projects;
        }

        public ObservableCollection<TeamCityItem> Projects { get; private set; }

        public class ProjectInfo
        {
            public string Name { get; set; }
            public ObservableCollection<BuildInfo> Builds { get; set; }
        }

        public class BuildInfo
        {
            public string Name { get; set; }
            public bool IsMonitored { get; set; }
        }
    }

    public class TeamCityItem : INotifyPropertyChanged
    {
        bool? _isChecked = false;
        TeamCityItem _parent;

        public TeamCityItem(string name)
        {
            Name = name;
            Children = new List<TeamCityItem>();
        }

        public void Initialize()
        {
            foreach (var child in Children)
            {
                child._parent = this;
                child.Initialize();
            }
        }

        public List<TeamCityItem> Children { get; private set; }
        public bool IsInitiallySelected { get; set; }
        public string Name { get; private set; }

        /// <summary>
        /// Gets/sets the state of the associated UI toggle (ex. CheckBox).
        /// The return value is calculated based on the check state of all
        /// child FooViewModels.  Setting this property to true or false
        /// will set all children to the same check state, and setting it 
        /// to any value will cause the parent to verify its check state.
        /// </summary>
        public bool? IsChecked
        {
            get { return _isChecked; }
            set { SetIsChecked(value, true, true); }
        }

        void SetIsChecked(bool? value, bool updateChildren, bool updateParent)
        {
            if (value == _isChecked)
                return;

            _isChecked = value;

            if (updateChildren && _isChecked.HasValue)
                Children.ForEach(c => c.SetIsChecked(_isChecked, true, false));

            if (updateParent && _parent != null)
                _parent.VerifyCheckState();

            OnPropertyChanged("IsChecked");
        }

        void VerifyCheckState()
        {
            bool? state = null;
            for (int i = 0; i < Children.Count; ++i)
            {
                bool? current = Children[i].IsChecked;
                if (i == 0)
                {
                    state = current;
                }
                else if (state != current)
                {
                    state = null;
                    break;
                }
            }
            SetIsChecked(state, false, true);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }


    public static class VirtualToggleButton
    {
        #region attached properties

        #region IsChecked

        /// <summary>
        /// IsChecked Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.RegisterAttached("IsChecked", typeof(bool?), typeof(VirtualToggleButton),
                new FrameworkPropertyMetadata((bool?)false,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
                    OnIsCheckedChanged));

        /// <summary>
        /// Gets the IsChecked property.  This dependency property 
        /// indicates whether the toggle button is checked.
        /// </summary>
        public static bool? GetIsChecked(DependencyObject d)
        {
            return (bool?)d.GetValue(IsCheckedProperty);
        }

        /// <summary>
        /// Sets the IsChecked property.  This dependency property 
        /// indicates whether the toggle button is checked.
        /// </summary>
        public static void SetIsChecked(DependencyObject d, Nullable<bool> value)
        {
            d.SetValue(IsCheckedProperty, value);
        }

        /// <summary>
        /// Handles changes to the IsChecked property.
        /// </summary>
        private static void OnIsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pseudobutton = d as UIElement;
            if (pseudobutton != null)
            {
                var newValue = (bool?)e.NewValue;
                if (newValue == true)
                {
                    RaiseCheckedEvent(pseudobutton);
                }
                else if (newValue == false)
                {
                    RaiseUncheckedEvent(pseudobutton);
                }
                else
                {
                    RaiseIndeterminateEvent(pseudobutton);
                }
            }
        }

        #endregion

        #region IsThreeState

        /// <summary>
        /// IsThreeState Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsThreeStateProperty =
            DependencyProperty.RegisterAttached("IsThreeState", typeof(bool), typeof(VirtualToggleButton),
                new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Gets the IsThreeState property.  This dependency property 
        /// indicates whether the control supports two or three states.  
        /// IsChecked can be set to null as a third state when IsThreeState is true.
        /// </summary>
        public static bool GetIsThreeState(DependencyObject d)
        {
            return (bool)d.GetValue(IsThreeStateProperty);
        }

        /// <summary>
        /// Sets the IsThreeState property.  This dependency property 
        /// indicates whether the control supports two or three states. 
        /// IsChecked can be set to null as a third state when IsThreeState is true.
        /// </summary>
        public static void SetIsThreeState(DependencyObject d, bool value)
        {
            d.SetValue(IsThreeStateProperty, value);
        }

        #endregion

        #region IsVirtualToggleButton

        /// <summary>
        /// IsVirtualToggleButton Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsVirtualToggleButtonProperty =
            DependencyProperty.RegisterAttached("IsVirtualToggleButton", typeof(bool), typeof(VirtualToggleButton),
                new FrameworkPropertyMetadata(false,
                    OnIsVirtualToggleButtonChanged));

        /// <summary>
        /// Gets the IsVirtualToggleButton property.  This dependency property 
        /// indicates whether the object to which the property is attached is treated as a VirtualToggleButton.  
        /// If true, the object will respond to keyboard and mouse input the same way a ToggleButton would.
        /// </summary>
        public static bool GetIsVirtualToggleButton(DependencyObject d)
        {
            return (bool)d.GetValue(IsVirtualToggleButtonProperty);
        }

        /// <summary>
        /// Sets the IsVirtualToggleButton property.  This dependency property 
        /// indicates whether the object to which the property is attached is treated as a VirtualToggleButton.  
        /// If true, the object will respond to keyboard and mouse input the same way a ToggleButton would.
        /// </summary>
        public static void SetIsVirtualToggleButton(DependencyObject d, bool value)
        {
            d.SetValue(IsVirtualToggleButtonProperty, value);
        }

        /// <summary>
        /// Handles changes to the IsVirtualToggleButton property.
        /// </summary>
        private static void OnIsVirtualToggleButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as IInputElement;
            if (element != null)
            {
                if ((bool)e.NewValue)
                {
                    element.MouseLeftButtonDown += OnMouseLeftButtonDown;
                    element.KeyDown += OnKeyDown;
                }
                else
                {
                    element.MouseLeftButtonDown -= OnMouseLeftButtonDown;
                    element.KeyDown -= OnKeyDown;
                }
            }
        }

        #endregion

        #endregion

        #region routed events

        #region Checked

        /// <summary>
        /// A static helper method to raise the Checked event on a target element.
        /// </summary>
        /// <param name="target">UIElement or ContentElement on which to raise the event</param>
        internal static RoutedEventArgs RaiseCheckedEvent(UIElement target)
        {
            if (target == null)
                return null;

            var args = new RoutedEventArgs();
            args.RoutedEvent = ToggleButton.CheckedEvent;
            RaiseEvent(target, args);
            return args;
        }

        #endregion

        #region Unchecked

        /// <summary>
        /// A static helper method to raise the Unchecked event on a target element.
        /// </summary>
        /// <param name="target">UIElement or ContentElement on which to raise the event</param>
        internal static RoutedEventArgs RaiseUncheckedEvent(UIElement target)
        {
            if (target == null)
                return null;

            var args = new RoutedEventArgs();
            args.RoutedEvent = ToggleButton.UncheckedEvent;
            RaiseEvent(target, args);
            return args;
        }

        #endregion

        #region Indeterminate

        /// <summary>
        /// A static helper method to raise the Indeterminate event on a target element.
        /// </summary>
        /// <param name="target">UIElement or ContentElement on which to raise the event</param>
        internal static RoutedEventArgs RaiseIndeterminateEvent(UIElement target)
        {
            if (target == null)
                return null;

            var args = new RoutedEventArgs();
            args.RoutedEvent = ToggleButton.IndeterminateEvent;
            RaiseEvent(target, args);
            return args;
        }

        #endregion

        #endregion

        #region private methods

        private static void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            UpdateIsChecked(sender as DependencyObject);
        }

        private static void OnKeyDown(object sender, KeyEventArgs e)
        {
            var senderObject = sender as DependencyObject;
            if (senderObject == null)
                return;

            if (e.OriginalSource == senderObject)
            {
                if (e.Key == Key.Space)
                {
                    // ignore alt+space which invokes the system menu
                    if ((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
                        return;

                    UpdateIsChecked(sender as DependencyObject);
                    e.Handled = true;

                }
                else if (e.Key == Key.Enter && (bool)(senderObject).GetValue(KeyboardNavigation.AcceptsReturnProperty))
                {
                    UpdateIsChecked(sender as DependencyObject);
                    e.Handled = true;
                }
            }
        }

        private static void UpdateIsChecked(DependencyObject d)
        {
            var isChecked = GetIsChecked(d);
            if (isChecked == true)
            {
                SetIsChecked(d, GetIsThreeState(d) ? (bool?)null : false);
            }
            else
            {
                SetIsChecked(d, isChecked.HasValue);
            }
        }

        private static void RaiseEvent(DependencyObject target, RoutedEventArgs args)
        {
            if (target is UIElement)
            {
                (target as UIElement).RaiseEvent(args);
            }
            else if (target is ContentElement)
            {
                (target as ContentElement).RaiseEvent(args);
            }
        }

        #endregion
    }
}
