
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Graphics;

namespace ImagePlayground
{
	public class EnhancedImageView : SurfaceView, ISurfaceHolderCallBack
	{
		private Paint paint =  new Paint (PaintFlags.AntiAlias);
		private VelocityTracker mVelocityTracker = null;

		public EnhancedImageView(Context ctx) : base(ctx){
			paint.Color = Color.Red;
			paint.SetStyle (Paint.Style.Stroke);
			paint.StrokeWidth = 3.0f;
		}

		public override bool OnTouchEvent (MotionEvent e)
		{
			int index = e.ActionIndex;
			int pointerId = e.GetPointerId (index);

			switch (e.Action)
			{
			case MotionEventActions.Down:
				if (this.Holder.Surface.IsValid) {
					
						
					return true;
				}
				break;
									
				case MotionEventActions.Move:
					if (mVelocityTracker == null) {
						// Retrieve a new VelocityTracker object to watch the velocity of a motion.
						mVelocityTracker = VelocityTracker.Obtain ();
					}
					if (this.Holder.Surface.IsValid) {
						mVelocityTracker.AddMovement (e);
						// When you want to determine the velocity, call 
						// computeCurrentVelocity(). Then call getXVelocity() 
						// and getYVelocity() to retrieve the velocity for each pointer ID. 
						mVelocityTracker.ComputeCurrentVelocity (1000);
						// Log velocity of pixels per second
						// Best practice to use VelocityTrackerCompat where possible.
						Log.Debug ("", "X velocity: " + mVelocityTracker.GetXVelocity (pointerId));
						Log.Debug ("", "Y velocity: " +
						mVelocityTracker.GetYVelocity (pointerId));
					}
					break;

				case MotionEventActions.Cancel:			
					// Return a VelocityTracker object back to be re-used by others.
					mVelocityTracker.Recycle ();
					return true;

			}
			return false;
		}
	}
}

