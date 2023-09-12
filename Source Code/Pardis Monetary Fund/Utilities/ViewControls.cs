using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Pardis_Monetary_Fund
{
    public static class ViewControls
    {
        public static void FindChildGroup<T>(DependencyObject parent, string childName, ref List<T> list) where T : DependencyObject
        {
            // Checks should be made, but preferably one time before calling.
            // And here it is assumed that the programmer has taken into
            // account all of these conditions and checks are not needed.
            //if ((parent == null) || (childName == null) || (<Type T is not inheritable from FrameworkElement>))
            //{
            //    return;
            //}

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < childrenCount; i++)
            {
                // Get the child
                var child = VisualTreeHelper.GetChild(parent, i);

                // Compare on conformity the type
                T child_Test = child as T;

                // Not compare - go next
                if (child_Test == null)
                {
                    // Go the deep
                    FindChildGroup<T>(child, childName, ref list);
                }
                else
                {
                    // If match, then check the name of the item
                    FrameworkElement child_Element = child_Test as FrameworkElement;

                    if (child_Element.Name == childName)
                    {
                        // Found
                        list.Add(child_Test);
                    }

                    // We are looking for further, perhaps there are
                    // children with the same name
                    FindChildGroup<T>(child, childName, ref list);
                }
            }

            return;
        }
    }
}
