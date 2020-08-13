/*
 * @author Yathartha Sharma
 * @email yatharthasharma09@gmail.com
 * @create February 2019
 * @description
*/

using Android.App;
using Android.Content;
using Plugin.Connectivity;
using Android.Graphics;
using System.Net;

namespace FoodChain
{
    class HelperFunctions
    {
        // Check if the device is connected to internet
        public static bool DoIHaveInternet()
        {
            return CrossConnectivity.Current.IsConnected;
        }
        // Used to create informational alert boxes - provide the title and message to be included in the box
        public static void UseAlertBox(Context context, string title, string message)
        {
            AlertDialog.Builder noNetworkAlert = new AlertDialog.Builder(context);
            noNetworkAlert.SetTitle(title)
            .SetMessage(message)
            .SetPositiveButton("Okay", (senderAlert, args) => {});
            Dialog dialog = noNetworkAlert.Create();
            dialog.Show();
        }
        // show progress bar having the specified title
        public static AlertDialog UseAlertBoxProgressBar(Context context, string title)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(context);
            builder.SetTitle(title).SetCancelable(false).SetView(Resource.Layout.alertdialog_progressbar);
            AlertDialog dialog = builder.Create();
            dialog.Show();
            return dialog;
        }
        public static void HideAlertBoxProgressBar(AlertDialog dialog)
        {
            dialog.Hide();
        }
        // return images from the given URLs in Bitmap format
        public static Bitmap GetImageBitmapFromUrl(string url)
        {
            Bitmap imageBitmap = null;

            using (var webClient = new WebClient())
            {
                var imageBytes = webClient.DownloadData(url);
                if (imageBytes != null && imageBytes.Length > 0)
                {
                    imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                }
            }
            return imageBitmap;
        }
    }
}