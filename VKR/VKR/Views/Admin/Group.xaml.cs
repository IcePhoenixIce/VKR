using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VKR.Views.Admin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Group : TabbedPage
    {
        VKR.Models.Admin.Group group;
        public Group(ref VKR.Models.Admin.Group group)
        {
            this.group = group;
            InitializeComponent();
            Children.Add(new GroupCreate(ref this.group));
            Children.Add(new TimeShedule(ref this.group));
            Children.Add(new Employeers(ref this.group));
            Children.Add(new MapPage(ref this.group));
            //Children.Add();
        }
    }
}