
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Graphics;
using System.Runtime.ConstrainedExecution;
using System.Security.Policy;
using Android.Util;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using System.Collections;
using System.Drawing;
using System.Runtime.Remoting.Messaging;
using Java.Util;
using Android.Nfc;
using Android.Views.InputMethods;
using Android.Provider;
using Graphics = Android.Graphics;
using Android.Graphics.Drawables;
using Android.Widget;
using Android.Media;
using System.Threading;
using Java.Util.Concurrent;

namespace ImagePlayground
{
	[Activity (Label = "MyView2")]			
	public class MyView2 : View
	{
		Graphics.Point[] points = new Graphics.Point[4];

		// Array that hold the circle
		private List<ResizeCircle> circles = new List<ResizeCircle>();

		// Variable to keep tracking of which circle is being dragged
		private int circleId = -1;

		// Points are grouped in groups of two so there's always only one fixed point	    
		// groupId = 0 > Touch Inside the Rectangle
		// groupId = 1 > Points 0 and 2
		// groupId = 2 > Points 1 and 3
		int groupId = -1;

		// FirstTouch's Coordinate for Tracking on Dragging
		int xFirstTouch = 0;
		int yFirstTouch = 0;

		private Bitmap mBitmap;

		/** Main Bitmap **/
		public Bitmap Bitmap { get{ return mBitmap; } set{ mBitmap = value; Invalidate (); } }

		/** Measured Size of the View **/
		private Rect mMeasuredRect;

		/** Paint to Draw Rectangles **/
		private Paint mRectPaint;

		public MyView2(Context ctx) : base (ctx){
			init (ctx);
		}

		public MyView2 (Context ctx, IAttributeSet attrs) : base (ctx, attrs){
			init (ctx);
		}

		public MyView2 (Context ctx, IAttributeSet attrs, int defStyle) : base(ctx,attrs,defStyle){
			init (ctx);
		}			
			
		private void init(Context ctx){
			// For Touch Events
			Focusable = true;
			// Draw the Image on the Background
			mBitmap = BitmapFactory.DecodeResource(ctx.Resources, Resource.Drawable.bg);

			// Sets up the paint for the Drawable Rectangles
			mRectPaint = new Paint ();
			mRectPaint.Color = Android.Graphics.Color.Aqua;
			mRectPaint.StrokeWidth = 4;
			mRectPaint.SetStyle (Paint.Style.Stroke);
		}

		protected override void OnDraw(Canvas canvas){
			// Background Bitmap to Cover all Area
			canvas.DrawBitmap(mBitmap, null, mMeasuredRect, null);

			// Just draw the points only if it has already been initiated
			if (points [3] != null) {				
				int left, top, right, bottom;
				left = points [0].X;
				top = points [0].Y;
				right = points [0].X;
				bottom = points [0].Y;

				// Sets the circles' locations
				for (int i = 1; i < points.Length; i++) {
					left = left > points [i].X ? points [i].X : left;
					top = top > points [i].Y ? points [i].Y : top;
					right = right < points [i].X ? points [i].X : right;
					bottom = bottom < points [i].Y ? points [i].Y : bottom;
				}

				mRectPaint.AntiAlias = true;
				mRectPaint.Dither = true;
				mRectPaint.StrokeJoin = Paint.Join.Round;
				mRectPaint.StrokeWidth = 5;
				mRectPaint.SetStyle (Paint.Style.Stroke);
				mRectPaint.Color = Graphics.Color.ParseColor ("#0079A3");

				canvas.DrawRect (
					left + circles [0].GetCircleWidth () / 2,
					top + circles [0].GetCircleWidth () / 2, 
					right + circles [2].GetCircleWidth () / 2, 
					bottom + circles [2].GetCircleWidth () / 2, mRectPaint);

				// Fill The Rectangle
				mRectPaint.SetStyle (Paint.Style.Fill);
				mRectPaint.Color = Graphics.Color.ParseColor ("#B2D6E3");
				mRectPaint.Alpha = 75;
				mRectPaint.StrokeWidth = 0;

				canvas.DrawRect (
					left + circles [0].GetCircleWidth () / 2,
					top + circles [0].GetCircleWidth () / 2, 
					right + circles [2].GetCircleWidth () / 2, 
					bottom + circles [2].GetCircleWidth () / 2, mRectPaint);

				// DEBUG
				mRectPaint.Color = Graphics.Color.Red;
				mRectPaint.TextSize = 18;
				mRectPaint.StrokeWidth = 0;

				// Draw every circle on the right position
				for (int i = 0; i < circles.Count (); i++) {
					ResizeCircle circle = circles [i];
					float x = circle.GetX ();
					float y = circle.GetY ();
					canvas.DrawBitmap (circle.GetBitmap (), x, y,
						mRectPaint);

					// DEBUG
					canvas.DrawText ("" + (i + 1), circle.GetX (), circle.GetY (), mRectPaint);
				}
			}
		}

		public override bool OnTouchEvent(MotionEvent e){

			// Get the Coordinates of Touch
			int xTouch = (int) e.GetX ();
			int yTouch = (int) e.GetY ();
			int actionIndex = e.ActionIndex;

			switch (e.ActionMasked) {
			// In case user touch the screen
			case MotionEventActions.Down:

				// If no points were created
				if (points [0] == null) {
					// Offset to create the points
					int offset = 60;
					// Initialize a new Rectangle.
					points [0] = new Graphics.Point ();
					points [0].X = xTouch;
					points [0].Y = yTouch;

					points [1] = new Graphics.Point ();
					points [1].X = xTouch;
					points [1].Y = yTouch + offset;

					points [2] = new Graphics.Point ();
					points [2].X = xTouch + offset;
					points [2].Y = yTouch + offset;

					points [3] = new Graphics.Point ();
					points [3].X = xTouch + offset;
					points [3].Y = yTouch;

					// Add each circle to circles array
					foreach (Graphics.Point pt in points) {
						circles.Add (new ResizeCircle (Context, Resource.Drawable.circle, pt, mMeasuredRect));
					}
				} else {
					// Register Which Circle (if any) th user has touched
					groupId = getTouchedCircle (xTouch, yTouch);
					xFirstTouch = xTouch;
					yFirstTouch = yTouch;

				}
				break;
			case MotionEventActions.PointerDown:
				break;

			case MotionEventActions.Move:												
				if (groupId == 1 || groupId == 2) {
					
					// Move touched Circle as the finger moves
					circles[circleId].SetX(xTouch);
					circles[circleId].SetY(yTouch);

					// Move the two other circles accordingly
					if (groupId == 1) {
						circles[1].SetX(circles[0].GetX());
						circles[1].SetY(circles[2].GetY());
						circles[3].SetX(circles[2].GetX());
						circles[3].SetY(circles[0].GetY());
					} else {						
						circles[0].SetX(circles[1].GetX());
						circles[0].SetY(circles[3].GetY());
						circles[2].SetX(circles[3].GetX());
						circles[2].SetY(circles[1].GetY());
					}
					Invalidate();
				} else if (groupId == 0){
					// Calculate the delta for the dragging
					int xDelta =  (xTouch-xFirstTouch);
					int yDelta =  (yTouch-yFirstTouch);
					xFirstTouch = xTouch;
					yFirstTouch = yTouch;

					// Move each circle accordingly
					foreach (ResizeCircle circle in circles) {
						circle.SetX (circle.GetX () + xDelta);
						circle.SetY (circle.GetY () + yDelta);
					}

					// Redraw the view
					Invalidate ();				
				}
				break;

			case MotionEventActions.Up:
				break;
			default:
				break;			
			}
			Invalidate ();
			return true;
		}

	
		private int getTouchedCircle(int xTouch, int yTouch){
			int groupId = -1;
			for (int i = 0; i < circles.Count; i++) {
				ResizeCircle circle = circles [i];

				// Check if the touch was inside the bounds of the circle
				int centerX = circle.GetX () + circle.GetCircleWidth ();
				int centerY = circle.GetY () + circle.GetCircleHeight ();

				// Calculate the radius from the touch to the center of the circle
				double radCircle = Math.Sqrt ((double)(((centerX - xTouch) * (centerX - xTouch)) + (centerY - yTouch)
					* (centerY - yTouch)));
				
				// If the touch was on one of the circles
				if (radCircle < circle.GetCircleWidth ()) {
					circleId = circle.GetID ();
					if (circleId == 1 || circleId == 3) {
						groupId = 2;
						break;
					} else {
						groupId = 1;
						break;
					}
				} else {
					// User didn't touch any of the circles no the inside area
					groupId = -1;
				}
			}
			// If the touch wasn't on one of the circles, check if it was inside the rectangle
			if (groupId == -1) {
				List<int> xCoords = new List<int> ();
				List<int> yCoords = new List<int> ();

				// Gather Coordinates from all circles		
				foreach (ResizeCircle circle in circles){
					xCoords.Add (circle.GetX());
					yCoords.Add (circle.GetY());
				}

				// Store the max and min coordinates
				int minX = xCoords.Min ();
				int maxX = xCoords.Max ();
				int minY = yCoords.Min ();
				int maxY = yCoords.Max ();

				// Check if user has touched inside the rectangle
				if ((xTouch > minX && xTouch < maxX) && (yTouch > minY && yTouch < maxY)) {
					// User has touched inside the Rectangle
					groupId = 0;
				}
			}

			return groupId;
		}

		protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec){
			base.OnMeasure (widthMeasureSpec, heightMeasureSpec);

			mMeasuredRect = new Rect (0, 0, MeasuredWidth, MeasuredHeight);
		}
			
		public class ResizeCircle {

			Bitmap bitmap;
			Graphics.Point point;
			Rect measuredRect;
			static bool[] circleLocked = new bool[4];
			static bool locked;
			int id; 
			static int count = 0;

			public ResizeCircle(Context context, int resourceId, Graphics.Point point, Rect measuredRect) {
				this.id = count++;
				this.measuredRect = measuredRect;
				bitmap = BitmapFactory.DecodeResource(context.Resources,
					resourceId);
				Log.Debug("BITMAP" , bitmap.Height.ToString());
				this.point = point;
			}

			public int GetCircleWidth() {
				return bitmap.Width;
			}

			public int GetCircleHeight() {
				return bitmap.Height;
			}

			public Bitmap GetBitmap() {
				return bitmap;
			}

			public int GetX() {
				return point.X;
			}

			public int GetY() {
				return point.Y;
			}

			public int GetID() {
				return id;
			}
						
			public void SetX(int x) {				
				// If coordinates are out of the boundaries
				if (x <= 0 || x >= (measuredRect.Right - 30)) {
					// Lock the rectangle
					locked = true;
					// Specify which circles locked the rectangle
					circleLocked [this.id] = true;
				}else {
					if (circleLocked [this.id] == true) {
						Log.Debug ("ID - UNLOCKING", this.id.ToString ());
						for (int i = 0; i < circleLocked.Length; i++) {
							circleLocked [i] = false;
						}
						locked = false;
					}
				}
				if (!locked) {					
					point.X = (x <= 0) ? 0 : ((x >= measuredRect.Right - 30) ? (measuredRect.Right - 30) : x);
					Log.Debug ("X", x.ToString());
				}
				Log.Debug ("LOCKED", locked.ToString ());
			}

			public void SetY(int y) {
				point.Y = (y < 0) ? 0 : ( (y > measuredRect.Bottom - 20) ? measuredRect.Bottom - 20: y);
			}
		}

	}
}

