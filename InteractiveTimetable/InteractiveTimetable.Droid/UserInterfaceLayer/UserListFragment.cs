using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using InteractiveTimetable.Droid.ApplicationLayer;
using Android.Content;
using InteractiveTimetable.BusinessLayer.Models;

namespace InteractiveTimetable.Droid.UserInterfaceLayer
{
    public class UserListFragment : Fragment
    {
        #region Constants
        public static readonly string FragmentTag = "user_list_fragment";
        private static readonly string UserIdKey = "current_user_id";
        #endregion

        #region Widgets
        private RecyclerView _recyclerView;
        private ImageButton _addUserBtn;
        private ImageButton _deleteUserBtn;
        #endregion

        #region Internal Variables
        private RecyclerView.LayoutManager _layoutManager;
        private UserListAdapter _userListAdapter;
        private int _currentUserId;
        #endregion

        #region Flags
        private bool _isWideScreenDevice;
        #endregion
        
        public static UserListFragment NewInstance()
        {
            var userListFragment = new UserListFragment
            {
                Arguments = new Bundle()
            };

            return userListFragment;
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            
            /* Getting users ordered by last name */
            var users = GetUsers();

            /* Initializing current user id */
            int userId;
            if (savedInstanceState != null)
            {
                userId = savedInstanceState.GetInt(UserIdKey, 0);
            }
            else
            {
                userId = users[0].Id;
            }

            /* Getting views */
            _recyclerView = Activity.FindViewById<RecyclerView>(Resource.Id.user_recycler_view);
            _recyclerView.SetItemAnimator(null);

            /* Setting up the layout manager */
            _layoutManager = new LinearLayoutManager(Activity);
            _recyclerView.SetLayoutManager(_layoutManager);

            /* Setting up the adapter */
            _userListAdapter = new UserListAdapter(Activity, users);
            _userListAdapter.ItemClick += OnItemClick;
            _recyclerView.SetAdapter(_userListAdapter);

            /* Setting event handlers */
            _addUserBtn = Activity.FindViewById<ImageButton>(Resource.Id.add_user_btn);
            _addUserBtn.Click += OnAddBtnClicked;

            _deleteUserBtn = Activity.FindViewById<ImageButton>(Resource.Id.delete_user_btn);
            _deleteUserBtn.Click += OnDeleteBtnClicked;

            /* Determining wide screen device */
            var layout = Activity.FindViewById<LinearLayout>(Resource.Id.main_landscape);
            _isWideScreenDevice = layout != null && layout.Visibility == ViewStates.Visible;

            if (_isWideScreenDevice)
            {
                ShowUserDetails(userId);
            }
            _currentUserId = userId;
        }

        public override View OnCreateView(
            LayoutInflater inflater, 
            ViewGroup container, 
            Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.user_list, container, false);
        }

        public override void OnDestroy()
        {
            _addUserBtn.Click -= OnAddBtnClicked;
            _deleteUserBtn.Click -= OnDeleteBtnClicked;
            GC.Collect();

            base.OnDestroy();
        }

        public void OnItemClick(object sender, UserListEventArgs args)
        {   
            // TODO: Delete the line below when dubugging is done
            Toast.MakeText(Activity, $"This is user with id: {args.UserId}", ToastLength.Short).Show();

            ShowUserDetails(args.UserId);
            _currentUserId = args.UserId;
        }

        public void OnAddBtnClicked(object sender, EventArgs args)
        {
            var editUserDetails = FragmentManager.FindFragmentByTag(UserDetailsEditFragment.FragmentTag)
                        as UserDetailsEditFragment;

            if (editUserDetails == null)
            {
                var editUserDetailsFragment = UserDetailsEditFragment.NewInstance(0);

                var fragmentManager = FragmentManager.BeginTransaction();
                fragmentManager.Replace(
                        Resource.Id.user_details,
                        editUserDetailsFragment,
                        UserDetailsEditFragment.FragmentTag
                    );

                fragmentManager.SetTransition(FragmentTransit.FragmentFade);
                fragmentManager.AddToBackStack(UserDetailsEditFragment.FragmentTag);
                fragmentManager.Commit();
            }
        }

        public void OnDeleteBtnClicked(object sender, EventArgs eventArgs)
        {
            /* Show alert if user in current timetable */
            if (InteractiveTimetable.Current.UserManager.IsUserInPresentTimetable(_currentUserId))
            {
                AskAndDeleteUser(
                    GetString(Resource.String.user_in_present_timetable),
                    _currentUserId);
            }
            /* Show general alert */
            else
            {
                AskAndDeleteUser(
                    GetString(Resource.String.sure_to_delete_user),
                    _currentUserId);
            }
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutInt(UserIdKey, _currentUserId);
            base.OnSaveInstanceState(outState);
        }

        private void ShowUserDetails(int userId)
        {
            if (_isWideScreenDevice)
            {
                /* Checking what fragment is shown and replacing if needed */
                var userDetails = FragmentManager.FindFragmentByTag(UserDetailsFragment.FragmentTag)
                        as UserDetailsFragment;

                if (userDetails == null || _currentUserId != userId)
                {
                    var userDetailsFragment = UserDetailsFragment.NewInstance(userId);

                    var fragmentManager = FragmentManager.BeginTransaction();
                    fragmentManager.Replace(
                            Resource.Id.user_details, 
                            userDetailsFragment, 
                            UserDetailsFragment.FragmentTag
                        );

                    fragmentManager.SetTransition(FragmentTransit.FragmentFade);
                    fragmentManager.Commit();
                }
            }
            else
            {
                var intent = new Intent();

                intent.SetClass(Activity, typeof(UserDetailsActivivty));
                intent.PutExtra(UserIdKey, userId);
                StartActivity(intent);
            }

        }

        private IList<User> GetUsers()
        {
            return InteractiveTimetable.Current.UserManager.GetUsers().
                                        OrderBy(x => x.LastName).
                                        ToList();
        }

        public void DataSetChanged()
        {
            _userListAdapter.Users = GetUsers();
            _userListAdapter.NotifyDataSetChanged();
        }

        private void AskAndDeleteUser(string questionToAsk, int userId)
        {
            using (var alert = new AlertDialog.Builder(Activity))
            {
                alert.SetTitle(GetString(Resource.String.delete_user));
                alert.SetMessage(questionToAsk);
                alert.SetPositiveButton(GetString(Resource.String.delete_button), (sender1, args) =>
                {
                    DeleteUser(userId);
                });
                alert.SetNegativeButton(GetString(Resource.String.cancel_button), (sender1, args) => {});

                Dialog dialog = alert.Create();
                dialog.Show();
            }
        }

        private void DeleteUser(int userId)
        {
            /* Delete from database */
            InteractiveTimetable.Current.UserManager.DeleteUser(userId);

            /* Delete from adapter */
            // TODO: Rename to RemoveCurrentItem and get rid of public CurrentPosition
            _userListAdapter.RemoveItem(_userListAdapter.CurrentPosition);
        }
    }
}