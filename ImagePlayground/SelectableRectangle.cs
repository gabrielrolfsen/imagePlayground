
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
using Android.Media;

namespace ImagePlayground
{
	public class SelectableRectangle : View
	{
		float x;
		float y;
		float radius = 10;

		Paint paint = new Paint (PaintFlags.AntiAlias);

		public SelectableRectangle(Context ctx, float x, float y) : base(ctx) {
			init (x, y);	
		}

		private void init(float x, float y){
			this.x = x;
			this.y = y;

			paint.Color = Color.Red;
			paint.SetStyle (Paint.Style.Stroke);
			paint.StrokeWidth = 3.0f;
		}

		protected override void OnDraw(Canvas canvas){
			base.OnDraw (canvas);
			Log.Debug ("<Rect> X: ", this.x.ToString());
			Log.Debug ("<Rect> Y: ", this.y.ToString());
			Log.Debug ("Canvas W", base.Height.ToString());
			Log.Debug ("Canvas H", canvas.Height.ToString());
//			canvas.DrawColor (Color.Black);
//			canvas.DrawCircle (this.x, this.y, this.radius, paint);
			canvas.DrawRect (new RectF (this.x,this.y,this.x + 50.0f,this.y + 50.0f), paint);
		}

		private void setPos (int x, int y) {
			base.SetX (x);
			base.SetY (y);
		}

		private void setRadius (float radius){
			this.radius = radius;
			Invalidate ();
			RequestLayout ();
		}

		public override bool OnTouchEvent (MotionEvent e){
			
			switch (e.Action) {
			case MotionEventActions.Down:
				Log.Debug ("RECT", "PRESSED.");
				setRadius (this.radius += 10);
				return true;
				break;

			case MotionEventActions.Move:
				//setPos (50, 140);
				break;

			case MotionEventActions.Cancel:
				break;
				return true;
			}
			return false;
		}
		private class ViewArea{
			int radius;
			int posX;
			int posY;

			ViewArea (int posX, int posY, int radius){
				this.posX = posX;
				this.posY = posY;
				this.radius = radius;
			}

			public override string ToString ()
			{
				return string.Format ("[ViewArea] [\" + centerX + \", \" + centerY + \", \" + radius + \"]\";");
			}
		}
	}
		
}

