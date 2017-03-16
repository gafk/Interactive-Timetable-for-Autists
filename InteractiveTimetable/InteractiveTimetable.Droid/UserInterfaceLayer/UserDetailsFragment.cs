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

namespace InteractiveTimetable.Droid.UserInterfaceLayer
{
    public class UserDetailsFragment : Fragment
    {
        private static readonly string UserIdKey = "current_user_id";

        public int UserId
        {
            get { return Arguments.GetInt(UserIdKey, 0); }
        }

        public static UserDetailsFragment NewInstance(int userId)
        {
            var userDetailsFragment = new UserDetailsFragment()
            {
                Arguments = new Bundle()
            };
            userDetailsFragment.Arguments.PutInt(UserIdKey, userId);

            return userDetailsFragment;
        }

        public override View OnCreateView(
            LayoutInflater inflater, 
            ViewGroup container, 
            Bundle savedInstanceState)
        {
            if (container == null)
            {
                return null;
            }

            /* Getting data */
            var user = InteractiveTimetable.Current.UserManager.GetUser(UserId);
            View userView = inflater.Inflate(Resource.Layout.user_details, container, false);

            /* Setting last name */
            var lastNameView = userView.FindViewById<TextView>(Resource.Id.user_details_last_name);
            lastNameView.Text += " " + user.LastName;

            /* Setting first name */
            var firstNameView = userView.FindViewById<TextView>(Resource.Id.user_details_first_name);
            firstNameView.Text += " " + user.FirstName;

            /* Setting patronymic name */
            var patronymicNameView =
                    userView.FindViewById<TextView>(Resource.Id.user_details_patronymic_name);
            patronymicNameView.Text += " " + user.PatronymicName;

            /* Setting age */
            var ageView = userView.FindViewById<TextView>(Resource.Id.user_details_age);
            ageView.Text += user.Age;

            /* Setting birth date */
            var birthView = userView.FindViewById<TextView>(Resource.Id.user_details_birth);
            birthView.Text += " " + user.BirthDate.ToString("dd.MM.yyyy");

            /* Setting photo */
            var photoView = userView.FindViewById<ImageView>(Resource.Id.user_details_photo);
            photoView.SetImageURI(Android.Net.Uri.Parse(user.PhotoPath));

            return userView;
        }
    }
}