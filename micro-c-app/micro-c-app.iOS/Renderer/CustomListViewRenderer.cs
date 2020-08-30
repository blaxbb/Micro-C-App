using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using micro_c_app.ViewModels;
using micro_c_app.Views;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ListView), typeof(micro_c_app.iOS.Renderer.CustomListViewRenderer))]
namespace micro_c_app.iOS.Renderer
{
    //
    //https://stackoverflow.com/a/49202240
    //
    public class CustomListViewRenderer : ListViewRenderer
    {
        public CustomListViewRenderer()
        {
            ElementChanged += CustomListViewRenderer_ElementChanged;
            BuildPageViewModel.CellUpdated += UpdateTableView;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            base.OnElementChanged(e);

            if(Control != null)
            {
                Control.KeyboardDismissMode = UIScrollViewKeyboardDismissMode.OnDrag;
            }
        }

        private void CustomListViewRenderer_ElementChanged(object sender, ElementChangedEventArgs<ListView> e)
        {
            if (e.NewElement is ListView listView)
            {
                listView.ItemSelected += (s,a) => UpdateTableView();
            }
        }

        private void UpdateTableView()
        {
            var tv = Control as UITableView;
            if (tv == null) return;
            tv.BeginUpdates();
            tv.EndUpdates();
        }
    }
}