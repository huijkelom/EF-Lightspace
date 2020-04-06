using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LightSpace_WPF_Engine.Wpf.Views.UserControls.CustomControls
{
    /// <summary>
    /// Interaction logic for CustomGameControlTemplate.xaml
    /// </summary>
    public partial class CustomGameControlTemplate : UserControl
    {
        public CustomGameControlTemplate()
        {
            InitializeComponent();

            // Instructions to make a new custom game control:
            // Right click the folder CustomControls (LightSpace_WPF_Engine/Wpf/Views/UserControls/CustomControls) and create a new UserControl.
            // In the new .xaml file, change the "d:DesignHeight="450" d:DesignWidth="800"" attributes of the UserControl to 300x300.

            // The grid is what's gonna make it easy to use, check out the example template xaml to see how the grid rows are defined.

            // When making a new button or other control, assign the Grid.Row & Grid.Column attributes to fit the correct column (index starting at 0, check template example).
            // If you want buttons to be larger than a single grid cell, use Grid.ColumnSpan & Grid.RowSpan to set the amount of cells it covers starting -
            //      from the one you have assigned in Grid.Row & Grid.Column.

            // To use icons in your control, visit the website "http://modernuiicons.com/". Here you can search for icons and after you select one, it will show it's data.
            // Copy the Path control section of an icon and paste that wherever you want the control. Afterwards you can tweak the size, position and color as you wish.

            // TIP: If your text isn't fitting the way you want it, or is getting cut off, try giving the Label within a negative margin to better fit the button.
            // TIP: Name all controls you need to access at all. This makes it possible to easily access them from their respective .xaml.cx file by the given name.
        }
    }
}
