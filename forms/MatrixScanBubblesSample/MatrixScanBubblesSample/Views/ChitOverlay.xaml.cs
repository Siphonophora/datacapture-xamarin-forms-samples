namespace MatrixScanBubblesSample.Views
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChitOverlay : ContentView
    {
        public ChitOverlay(int count)
        {
            InitializeComponent();

            this.HeightRequest = 25;
            this.WidthRequest = 15;

            if (count > 0)
            {
                Label.Text = count.ToString();
                StackLayout.BackgroundColor = Color.LightGreen;
            }
            else
            {
                Label.Text = count.ToString();
                StackLayout.BackgroundColor = Color.LightGray;
            }
        }
    }
}
