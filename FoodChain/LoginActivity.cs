/*
 * @author Yathartha Sharma
 * @email yatharthasharma09@gmail.com
 * @create February 2019
 * @description
*/

using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using Parse;
using System;
using Android.Content;
using Android.Support.Design.Widget;
using AlertDialog = Android.App.AlertDialog;

namespace FoodChain
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class LoginActivity : AppCompatActivity
    {
        Button signinButton;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Back4App Initialization
            ParseClient.Initialize(new ParseClient.Configuration
            {
                ApplicationId = GetString(Resource.String.back4app_app_id),
                WindowsKey = GetString(Resource.String.back4app_dotnet_key),
                Server = "https://parseapi.back4app.com"
            });
            // if the user is logged in redirect the app to home page. DOESN'T WORK IF NO INTERNET AVAILABLE WHEN OPENING UP THE APP.
            if (HelperFunctions.DoIHaveInternet())
            {
                ParseUser user = ParseUser.CurrentUser;
                if (user != null)
                {
                    Intent intent = new Intent(this, typeof(MainActivity));
                    intent.SetFlags(ActivityFlags.NewTask);
                    intent.AddFlags(ActivityFlags.ClearTask);
                    StartActivity(intent);
                }
            }
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_login);

            TextView registerButton = FindViewById<TextView>(Resource.Id.login_registerbutton);
            //Can be used for connection testing
            //ParseInstallation.CurrentInstallation.SaveAsync();
            signinButton = FindViewById<Button>(Resource.Id.login_signinbutton);
            //signinButton.Click += SignInButtonClick;
            signinButton.Click += delegate
                {
                    TextInputEditText username = FindViewById<TextInputEditText>(Resource.Id.login_usernametext);
                    TextInputEditText password = FindViewById<TextInputEditText>(Resource.Id.login_passwordtext);
                    if (username.Text == "")
                    {
                        if (password.Text == "")
                        {
                            password.Error = Resources.GetString(Resource.String.login_passwordempty);
                            username.Error = Resources.GetString(Resource.String.login_usernameempty);
                        }
                        else
                        {
                            password.Error = null;
                            username.Error = Resources.GetString(Resource.String.login_usernameempty);
                        }
                    }
                    else
                    {
                        if (password.Text == "")
                        {
                            password.Error = Resources.GetString(Resource.String.login_passwordempty);
                            username.Error = null;
                        }
                        /*else
                        {
                            password.Error = null;
                            username.Error = null;
                        }*/
                    }
                    if (username.Text != "" && password.Text != "")
                        SignInButtonClick(username.Text, password.Text);
                };
            registerButton.Click += delegate
                {
                    if (HelperFunctions.DoIHaveInternet())
                    {
                        Intent intent = new Intent(this, typeof(RegisterActivity));
                        StartActivity(intent);
                    }
                    else
                    {
                        HelperFunctions.UseAlertBox(this, Resources.GetString(Resource.String.no_network_title), Resources.GetString(Resource.String.no_network_message));
                    }
                };
        }
        // handles signing in of users
        private async void SignInButtonClick(string username, string password)
        {
            AlertDialog dialog = null;
            try
            {
                dialog = HelperFunctions.UseAlertBoxProgressBar(this, "Logging you in...");
                await ParseUser.LogInAsync(username, password);
                Intent intent = new Intent(this, typeof(MainActivity));
                HelperFunctions.HideAlertBoxProgressBar(dialog);
                intent.SetFlags(ActivityFlags.NewTask);
                intent.AddFlags(ActivityFlags.ClearTask);
                StartActivity(intent);
            }
            catch (ParseException p)
            {
                if (p.Code == ParseException.ErrorCode.UsernameMissing)
                {
                    HelperFunctions.UseAlertBox(this, "Username Missing", "The username is missing or empty.");
                    HelperFunctions.HideAlertBoxProgressBar(dialog);
                }  
                else if (p.Code == ParseException.ErrorCode.PasswordMissing)
                {
                    HelperFunctions.UseAlertBox(this, "Password Missing", "The password is missing or empty.");
                    HelperFunctions.HideAlertBoxProgressBar(dialog);
                }
                else
                {
                    HelperFunctions.UseAlertBox(this, "Login Failed", "Incorrect Username and/or Password.");
                    HelperFunctions.HideAlertBoxProgressBar(dialog);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                HelperFunctions.UseAlertBox(this, "Login Failed", "Incorrect Username and/or Password.");
                HelperFunctions.HideAlertBoxProgressBar(dialog);
            }
        }
    }
}