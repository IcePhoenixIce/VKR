using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VKR.Models;
using VKR.ViewModels.Admin;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VKR.Views.Admin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MarkerCreate : ContentPage
    {
        public MarkerCreate()
        {
            InitializeComponent();
            if (MapPageViewModel.buttonClickedType == ButtonClickedType.Change)
                mm.IsVisible = true;
        }
    }
}