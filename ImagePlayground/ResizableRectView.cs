
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

namespace ImagePlayground
{
	[Activity (Label = "ResizableRectangleView")]			
	public class ResizableRectangleView : View
	{
		Graphics.Point[] points = new Graphics.Point[4];

		// Array that hold the circle
		private List<ResizeCircle> resizeCircles = new List<ResizeCircle>();

		// Variable to keep tracking of which circle is being dragged
		private int circleId = 0;

		// 	 point1 and point3 are of same group and same as point2 and point4	    
		int groupId = -1;

		/** Main Bitmap **/
		private Bitmap mBitmap = null;

		private Rect mMeasuredRect;

		/** Paint to Draw Rectangles **/
		private Paint mRectPaint;

		public ResizableRectangleView(Context ctx) : base (ctx){
			init (ctx);
		}

		public ResizableRectangleView (Context ctx, IAttributeSet attrs) : base (ctx, attrs){
			init (ctx);
		}

		public ResizableRectangleView (Context ctx, IAttributeSet attrs, int defStyle) : base(ctx,attrs,defStyle){
			init (ctx);
		}


		private void init(Context ctx){
			// For Touch Events
			Focusable = true;
			mBitmap = BitmapFactory.DecodeResource(ctx.Resources, Resource.Drawable.bg);

			mRectPaint = new Paint ();
			mRectPaint.Color = Android.Graphics.Color.Aqua;
			mRectPaint.StrokeWidth = 4;
			mRectPaint.SetStyle (Paint.Style.Stroke);
		}

		protected override void OnDraw(Canvas canvas){
			if (points [3] == null) {
				return;
			}
			int left, top, right, bottom;
			left = points[0].X;
			top = points[0].Y;
			right = points[0].X;
			bottom = points[0].Y;

			for (int i = 1; i < points.Length; i++) {
				left = left > points[i].X ? points[i].X : left;
				top = top > points[i].Y ? points[i].Y : top;
				right = right < points[i].X ? points[i].X : right;
				bottom = bottom < points[i].Y ? points[i].Y : bottom;
			}

			mRectPaint.AntiAlias = true;
			mRectPaint.Dither = true;
			mRectPaint.StrokeJoin = Paint.Join.Round;
			mRectPaint.StrokeWidth = 5;
			mRectPaint.SetStyle (Paint.Style.Stroke);
			mRectPaint.Color = Graphics.Color.ParseColor ("#AADB1255");


			canvas.DrawRect(
				left + resizeCircles[0].GetCircleWidth() / 2,
				top + resizeCircles[0].GetCircleWidth() / 2, 
				right + resizeCircles[2].GetCircleWidth() / 2, 
				bottom + resizeCircles[2].GetCircleWidth() / 2, mRectPaint);
	
			// Fill The Rectangle
			mRectPaint.SetStyle(Paint.Style.Fill);
			mRectPaint.Color = Graphics.Color.ParseColor("#55DB1255");
			mRectPaint.StrokeWidth = 0;

			canvas.DrawRect(
				left + resizeCircles[0].GetCircleWidth() / 2,
				top + resizeCircles[0].GetCircleWidth() / 2, 
				right + resizeCircles[2].GetCircleWidth() / 2, 
				bottom + resizeCircles[2].GetCircleWidth() / 2, mRectPaint);

			//draw the corners

			// draw the balls on the canvas
			mRectPaint.Color = Graphics.Color.Blue;
			mRectPaint.TextSize = 18;
			mRectPaint.StrokeWidth = 0;

			for (int i = 0; i < resizeCircles.Count(); i ++) {
				ResizeCircle circle = resizeCircles[i];
				float x = circle.GetX ();
				float y = circle.GetY ();
				canvas.DrawBitmap(circle.GetBitmap(),x,y,
					mRectPaint);
				

				canvas.DrawText("" + (i+1), circle.GetX(), circle.GetY(), mRectPaint);
			}
			
		}

		public override bool OnTouchEvent(MotionEvent e){

			int xTouch = (int) e.GetX ();
			int yTouch = (int) e.GetY ();
			int actionIndex = e.ActionIndex;

			switch (e.ActionMasked) {
			case MotionEventActions.Down:

				if (points [0] == null) {
					//initialize rectangle.
					points [0] = new Graphics.Point ();
					points [0].X = xTouch;
					points [0].Y = yTouch;

					points [1] = new Graphics.Point ();
					points [1].X = xTouch;
					points [1].Y = yTouch + 30;

					points [2] = new Graphics.Point ();
					points [2].X = xTouch + 30;
					points [2].Y = yTouch + 30;

					points [3] = new Graphics.Point ();
					points [3].X = xTouch + 30;
					points [3].Y = yTouch;

					circleId = 2;
					groupId = 1;
					// declare each ball with the ColorBall class
					foreach (Graphics.Point pt in points) {
						resizeCircles.Add (new ResizeCircle (Context, Resource.Drawable.circle, pt));
					}
				}else {
					//resize rectangle
					circleId = -1;
					groupId = -1;
					for (int i = resizeCircles.Count - 1; i >= 0; i--) {
						ResizeCircle circle = resizeCircles [i];
						// check if inside the bounds of the ball (circle)
						// Get the center for the ball
						int centerX = circle.GetX() + circle.GetCircleWidth();
						int centerY = circle.GetY() + circle.GetCircleHeight();
						mRectPaint.Color = Graphics.Color.Cyan;

						// calculate the radius from the touch to the center of the
						// ball
						double radCircle = Math
							.Sqrt((double) (((centerX - xTouch) * (centerX - xTouch)) + (centerY - yTouch)
								* (centerY - yTouch)));

						if (radCircle < circle.GetCircleWidth()) {

							circleId = circle.GetID();
							if (circleId == 1 || circleId == 3) {
								groupId = 2;
							} else {
								groupId = 1;
							}
							// Maybe take it off
							Invalidate();
							break;
						}
						Invalidate();
					}
				}
				break;



			case MotionEventActions.Move:
				if (circleId > -1) {
					// move the balls the same as the finger
					resizeCircles[circleId].SetX(xTouch);
					resizeCircles[circleId].SetY(yTouch);

					mRectPaint.Color = Graphics.Color.Cyan;
					if (groupId == 1) {
						resizeCircles[1].SetX(resizeCircles[0].GetX());
						resizeCircles[1].SetY(resizeCircles[2].GetY());
						resizeCircles[3].SetX(resizeCircles[2].GetX());
						resizeCircles[3].SetY(resizeCircles[0].GetY());
					} else {
						resizeCircles[0].SetX(resizeCircles[1].GetX());
						resizeCircles[0].SetY(resizeCircles[3].GetY());
						resizeCircles[2].SetX(resizeCircles[3].GetX());
						resizeCircles[2].SetY(resizeCircles[1].GetY());
					}
					Invalidate();
				}
				break;

			case MotionEventActions.Up:
//				handled = true;
				break;
			default:
				break;			
			}
			Invalidate ();
			return true;
		}
			

		protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec){
			base.OnMeasure (widthMeasureSpec, heightMeasureSpec);

			mMeasuredRect = new Rect (0, 0, MeasuredWidth, MeasuredHeight);
		}
	
		public class ResizeCircle {

			Bitmap bitmap;
			Graphics.Point point;
			int id;
			static int count = 0;

			public ResizeCircle(Context context, int resourceId, Graphics.Point point) {
				this.id = count++;
				bitmap = BitmapFactory.DecodeResource(context.Resources,
					resourceId);
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
				point.X = x;
			}

			public void SetY(int y) {
				point.Y = y;
			}
		}

	}
}

