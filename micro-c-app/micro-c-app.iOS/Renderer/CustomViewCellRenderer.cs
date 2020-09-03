using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ViewCell), typeof(micro_c_app.iOS.Renderer.CustomViewCellRenderer))]
namespace micro_c_app.iOS.Renderer
{
    //
    //https://stackoverflow.com/a/25887514
    //
    public class CustomViewCellRenderer : ViewCellRenderer
    {
        public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
        {
            var cell = base.GetCell(item, reusableCell, tv);
            if (Xamarin.Forms.Application.Current.UserAppTheme == OSAppTheme.Dark || (Xamarin.Forms.Application.Current.UserAppTheme == OSAppTheme.Unspecified && Xamarin.Forms.Application.Current.RequestedTheme == OSAppTheme.Dark))
            {
                cell.SelectedBackgroundView = new UIView
                {
                    BackgroundColor = UIColor.DarkGray,
                };
                cell.SetNeedsLayout();
            }
            return cell;
        }
    }
}