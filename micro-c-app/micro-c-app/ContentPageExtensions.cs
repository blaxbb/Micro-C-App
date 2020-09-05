using micro_c_app.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace micro_c_app
{
    public static class ContentPageExtensions
    {
        public static void SetupActionButton(this ContentPage page)
        {
            if (Device.RuntimePlatform == "iOS" || Device.RuntimePlatform == "UWP")
            {
                page.ToolbarItems.Clear();
                page.ToolbarItems.Add(new ToolbarItem()
                {
                    Text = "Actions",
                    Command = new Command<object>((object param) =>
                    {
                        if (page.BindingContext is BaseViewModel vm)
                        {
                            vm.ShowActions.Execute(param);
                        }
                    })
                });
            }
        }
    }
}
