using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using AndroidX.Core.Content;
using FormsLoyalty.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Picker), typeof(ExtendedPicker))]
namespace FormsLoyalty.Droid.Renderers
{
    public class ExtendedPicker : PickerRenderer
    {
       Picker element;
        IElementController ElementController => Element as IElementController;
        private AlertDialog _dialog;
        public ExtendedPicker(Context context):base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
        {
            base.OnElementChanged(e);
            element = (Picker)this.Element;
            if (this.Control != null)
            {

                Control.Background = null;
                //Control.TextAlignment = Android.Views.TextAlignment.TextStart;
                Control.TextAlignment = Android.Views.TextAlignment.TextStart;

                //Control.Background = AddPickerStyles();
                Control.SetCompoundDrawablesWithIntrinsicBounds(null, null, GetDrawable(), null);
            }


        }
        
    

        private Drawable AddPickerStyles()
        {
            ShapeDrawable border = new ShapeDrawable();
            border.Paint.Color = Android.Graphics.Color.Gray;
            border.SetPadding(10, 10, 10, 10);
            border.Paint.SetStyle(Paint.Style.Stroke);

            Drawable[] layers = { border };


            LayerDrawable layerDrawable = new LayerDrawable(layers);
            layerDrawable.SetLayerInset(0, 0, 0, 0, 0);

            return layerDrawable;
        }
        private BitmapDrawable GetDrawable()
        {
            int resID = Resources.GetIdentifier("arrow_down", "drawable", this.Context.PackageName);
            var drawable = ContextCompat.GetDrawable(this.Context, resID);
            var bitmap = ((BitmapDrawable)drawable).Bitmap;

            var result = new BitmapDrawable(Resources, Bitmap.CreateScaledBitmap(bitmap, 15,15, true));
            result.Gravity = Android.Views.GravityFlags.Right;

            return result;
        }

       
    }
   
}