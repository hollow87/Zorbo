using System;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Windows;

using Zorbo.Interface;

namespace Zorbo.UI
{
    public class Help : NotifyObject
    {
        String helptext;
        Boolean isopen;

        UIElement control;


        public Boolean IsOpen {
            get { return isopen; }
            set {
                if (isopen != value) {
                    isopen = value;
                    RaisePropertyChanged(() => IsOpen);
                }
            }
        }

        public String Text {
            get { return helptext; }
            set {
                if (helptext != value) {
                    helptext = value;
                    RaisePropertyChanged(() => Text);
                }
            }
        }

        public UIElement Control {
            get { return control; }
            set {
                if (control != value) {
                    control = value;
                    RaisePropertyChanged(() => Control);
                }
            }
        }

        public static void SetHelpText(DependencyObject target, string text) {
            target.SetValue(HelpTextProperty, text);
        }

        public static string GetHelpText(DependencyObject target) {
            return (string)target.GetValue(HelpTextProperty);
        }


        private static void HelpTextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
            SetHelpText(obj, e.NewValue as string);
        }


        public static readonly DependencyProperty HelpTextProperty =
            DependencyProperty.RegisterAttached(
            "HelpText",
            typeof(String),
            typeof(Help),
            new PropertyMetadata(HelpTextChanged));
    }
}
