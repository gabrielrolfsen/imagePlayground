
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
using Android.Provider;
using Android.Graphics;
using System.Resources;
using System.ComponentModel.Design;

namespace ImagePlayground
{
	[Activity (Label = "EnhancedCameraActivity")]			
	public class EnhancedCameraActivity : Activity
	{
		Camera mCamera = null;
		
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.CameraEnhanced);

			SurfaceView cameraPreview = FindViewById<SurfaceView> (Resource.Id.cameraPreview);
			// Create your application here
		}

		private bool OpenCameraSafely(int id){
			bool success = false;

			try {
				ReleaseCameraAndPreview();
				mCamera = Camera.Open(id);
				success = (mCamera != null);
			}catch (Exception e){
				e.StackTrace;
			}

			return success;
		}

		private void ReleaseCameraAndPreview(){
			mPreview.SetCamera (null);
			if (mCamera != null) {
				mCamera.Release ();
				mCamera = null;
			}
		}

		class Preview : ViewGroup, ISurfaceHolderCallback{

			SurfaceView mSurfaceView;
			ISurfaceHolder mHolder;

		}
			
	}
}

