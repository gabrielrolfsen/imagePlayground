
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

namespace ImagePlayground
{
	[Activity (Label = "MyView")]			
	public class MyView : View
	{
		public class RectArea
		{
			public int Right { get; set; }
			public int Top { get; set; }
			public int Left { get; set; }
			public int Bottom { get; set; }
			public int Radius  { get; set; }

			public RectArea (int right, int top, int left, int bottom){
				this.Right = right;
				this.Top = top;
				this.Left = left;
				this.Bottom = bottom;
			}

		}

		/** Main Bitmap **/
		private Bitmap mBitmap = null;

		/** Variable to get view's size **/
		private Rect mMeasuredRect;

		/** Paint to Draw Rectangles **/
		private Paint mRectPaint;

		private readonly static int CIRCLES_LIMIT = 5;
		private readonly static int RECT_RADIUS = 50;

		// List to Store the created Rectangles
		private List<RectArea> mRect = new List<RectArea> (CIRCLES_LIMIT);

		// Array to Store Pointers to Rectangle
		private SparseArray<RectArea> mRectPointer = new SparseArray<RectArea> (CIRCLES_LIMIT);

		public MyView(Context ctx) : base (ctx){
			init (ctx);
		}

		public MyView (Context ctx, IAttributeSet attrs) : base (ctx, attrs){
			init (ctx);
		}

		public MyView (Context ctx, IAttributeSet attrs, int defStyle) : base(ctx,attrs,defStyle){
			init (ctx);
		}


		private void init(Context ctx){
			// Background Bitmap to Cover all Area
			mBitmap = BitmapFactory.DecodeResource(ctx.Resources, Resource.Drawable.bg);

			// Setup Paint to Draw Rectangle
			mRectPaint = new Paint ();
			mRectPaint.Color = Android.Graphics.Color.Aqua;
			mRectPaint.StrokeWidth = 4;
			mRectPaint.SetStyle (Paint.Style.Stroke);
		}

		protected override void OnDraw(Canvas canvas){
			// Background Bitmap to Cover all Area
			canvas.DrawBitmap(mBitmap, null, mMeasuredRect, null);

			foreach (RectArea rect in mRect) {
				canvas.DrawRect (rect.Left, rect.Top, rect.Right, rect.Bottom, mRectPaint);
			}
		}

		public override bool OnTouchEvent(MotionEvent e){

			bool handled = false;
			RectArea touchedRect;

			int xTouch;
			int yTouch;
			int pointerId;
			int actionIndex = e.ActionIndex;

			switch (e.ActionMasked) {
			case MotionEventActions.Down:

				// Clears the Rectangle Pointer
				ClearRectPointer ();

				// Get Touched Coordinates
				xTouch = (int)e.GetX (0);
				yTouch = (int)e.GetY (0);

				// Get the Rectangle for the touched area (newly created or not)
				touchedRect = ObtainTouchedRect (xTouch, yTouch);
				touchedRect.Right = xTouch + RECT_RADIUS;
				touchedRect.Top = yTouch - RECT_RADIUS;
				touchedRect.Left = xTouch - RECT_RADIUS;
				touchedRect.Bottom = yTouch + RECT_RADIUS;
				mRectPointer.Put (e.GetPointerId (0), touchedRect);

				Invalidate ();
				handled = true;
				break;
			case MotionEventActions.PointerDown:
				Log.Warn ("TAG", "Pointer Down");

				pointerId = e.GetPointerId (actionIndex);

				xTouch = (int)e.GetX (actionIndex);
				yTouch = (int)e.GetY (actionIndex);

				// Get the Rectangle for the touched area (newly created or not)
				touchedRect = ObtainTouchedRect (xTouch, yTouch);
				mRectPointer.Put (e.GetPointerId (0), touchedRect);
				touchedRect.Right = xTouch + RECT_RADIUS;
				touchedRect.Top = yTouch - RECT_RADIUS;
				touchedRect.Left = xTouch - RECT_RADIUS;
				touchedRect.Bottom = yTouch + RECT_RADIUS;

				Invalidate ();
				handled = true;

				break;

			case MotionEventActions.Move:
				int pointerCount = e.PointerCount;
				Log.Warn ("TAG", "Pointer Move");

				for (actionIndex = 0; actionIndex < pointerCount; actionIndex++) {
					pointerId = e.GetPointerId (actionIndex);

					xTouch = (int)e.GetX (actionIndex);
					yTouch = (int)e.GetY (actionIndex);

					touchedRect = mRectPointer.Get (pointerId);
					if (touchedRect != null){
						touchedRect.Right = xTouch + RECT_RADIUS;
						touchedRect.Top = yTouch - RECT_RADIUS;
						touchedRect.Left = xTouch - RECT_RADIUS;
						touchedRect.Bottom = yTouch + RECT_RADIUS;
					}

				}
				Invalidate ();
				handled = true;
				break;
			case MotionEventActions.Up:
				ClearRectPointer();
				Invalidate ();
				handled = true;
				break;

			case MotionEventActions.PointerUp:
				pointerId = e.GetPointerId (actionIndex);
				mRectPointer.Remove (pointerId);
				Invalidate ();
				handled = true;
				break;

			case MotionEventActions.Cancel:
				handled = true;
				break;
			default:
				break;			
			}

			return base.OnTouchEvent (e) || handled;
		}


		private void ClearRectPointer(){
			Log.Warn ("TAG", "ClearRectPointer()");
			mRectPointer.Clear ();
		}

		/// <summary>
		/// Obtain a Rectangle.
		/// </summary>
		/// <returns>The touched rectangle or a newly drawn rectangle.</returns>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		private RectArea ObtainTouchedRect(int x, int y){
			RectArea touchedRect = GetTouchedRect (x, y);

			if (null == touchedRect){
				touchedRect = new RectArea(x + RECT_RADIUS, y + RECT_RADIUS, x - RECT_RADIUS, y - RECT_RADIUS);

				if (mRect.Count == CIRCLES_LIMIT){
					Log.Warn("TAG", "Clear all circles,size is " + mRect.Count);
					mRect.Clear();
				}

				Log.Warn("ATG", "Added Rect " + touchedRect);
				mRect.Add(touchedRect);
			}

			return touchedRect;
		}

		/// <summary>
		/// Gets the touched rectangle.
		/// </summary>
		/// <returns>The touched rect. - If there is one.</returns>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		private RectArea GetTouchedRect (int x, int y){
			RectArea touched = null;

			foreach (RectArea rect in mRect) {
				// Checks if the touched area is withing a rectangle
				if ((x <= rect.Right) && (x >= rect.Left) && (y <= rect.Bottom) && (y >= rect.Top)){
					touched = rect;
					break;
				}
			}

			return touched;
		}


		protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec){
			base.OnMeasure (widthMeasureSpec, heightMeasureSpec);
			mMeasuredRect = new Rect (0, 0, MeasuredWidth, MeasuredHeight);
		}

	}
}

