using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics;
using Android.Util;
using Org.Xml.Sax;

namespace ImagePlayground
{
	[Activity (Label = "DraggableRect", Icon = "@drawable/launcher")]
	public class ResizableRectActivity : Activity
	{
		RelativeLayout parent;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.ResizableRect);
					
		}
			
	}
}


