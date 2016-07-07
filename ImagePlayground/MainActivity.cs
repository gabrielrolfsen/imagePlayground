
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace ImagePlayground
{
	[Activity (Label = "Image Playground",  MainLauncher = true, Icon = "@drawable/launcher")]			
	public class MainActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.Main);

			Button btn1 = FindViewById<Button> (Resource.Id.button1);
			Button btn2 = FindViewById<Button> (Resource.Id.button2);
			Button btn3 = FindViewById<Button> (Resource.Id.button3);
			Button btn4 = FindViewById<Button> (Resource.Id.button4);

			btn1.Click += delegate {
				StartActivity (typeof (CameraRectActivity));
			};

			btn2.Click += delegate {
				StartActivity (typeof (ResizableRectActivity));	
			};

			btn3.Click += delegate {
				StartActivity (typeof (EnhancedCameraActivity));
			};
				
			btn4.Click += delegate {
				StartActivity (typeof (CameraRectActivity));	
			};
				
		}
	}
}

