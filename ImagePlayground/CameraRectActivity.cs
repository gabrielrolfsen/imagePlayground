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
using Android.Provider;

namespace ImagePlayground
{
	[Activity (Label = "DraggableRect", Icon = "@drawable/launcher")]
	public class CameraRectActivity : Activity
	{
		MyView2 view; 

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.CameraRect);

			Button btnCamera = FindViewById<Button> (Resource.Id.btnCamera);
			view = FindViewById<MyView2> (Resource.Id.customView);

			btnCamera.Click += delegate { 
				// Creates a new intent to take a picture with the camera
				Intent intent = new Intent(MediaStore.ActionImageCapture);
				StartActivityForResult(intent, 0);

			};
		
		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data){
			base.OnActivityResult (requestCode, resultCode, data);
	
			// Send bitmap to Custom Vew
			Bundle extras = data.Extras;
			Bitmap bitmap = (Bitmap)extras.Get ("data");
			view.Bitmap = bitmap;



		}



	}
}


