using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using VKR.ViewModels.Admin;

namespace VKR.Views.Admin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPage : ContentPage
    {
        public MapPage(ref VKR.Models.Admin.Group group)
        {
            InitializeComponent();
            MapPageViewModel.map = MyMap;
            MapPageViewModel.groupId = group.GroupId;
            this.BindingContext = new MapPageViewModel();
        }
    }
}