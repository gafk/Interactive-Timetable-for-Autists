using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using InteractiveTimetable.BusinessLayer.Models;
using InteractiveTimetable.Droid.ApplicationLayer;

namespace InteractiveTimetable.Droid.UserInterfaceLayer
{
    public class TimetableTapeFragment : Fragment
    {
        #region Constants
        public static readonly string FragmentTag = "timetable_tape_fragment";
        #endregion

        #region Widgets
        private RecyclerView _recyclerView;
        private ImageButton _editTapeButton;
        private TextView _userName;
        private ImageView _userImage;
        private FrameLayout _staticGoalCardFrame;
        private ImageView _staticGoalCard;
        #endregion

        #region Internal Variables
        private XamarinRecyclerViewOnScrollListener _scrollListener;
        private LockableLinearLayoutManager _layoutManager;
        private TimetableTapeListAdapter _tapeItemListAdapter;
        private IList<Card> _tapeCards;
        private int _userId;
        private bool _isLocked; 
        #endregion

        #region Events
        public event Action<int> EditTimetableTapeButtonClicked;
        public event Action<object, EventArgs> ClickedWhenLocked;
        #endregion

        #region Methods

        #region Construct Methods
        public static TimetableTapeFragment NewInstance(int userId, IList<Card> tapeCards)
        {
            var timetableTapeFragment = new TimetableTapeFragment()
            {
                _userId = userId,
                _tapeCards = tapeCards,
                _isLocked =  false
            };

            return timetableTapeFragment;
        }
        #endregion

        #region Event Handlers
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);

            /* Get data */
            var user = InteractiveTimetable.Current.UserManager.GetUser(_userId);

            /* Get views */
            _recyclerView = Activity.FindViewById<RecyclerView>(Resource.Id.tape_item_list);
            _editTapeButton = Activity.FindViewById<ImageButton>(Resource.Id.tape_edit_button);
            _userName = Activity.FindViewById<TextView>(Resource.Id.tape_user_name);
            _userImage = Activity.FindViewById<ImageView>(Resource.Id.tape_user_image);
            _staticGoalCard = Activity.FindViewById<ImageView>(Resource.Id.static_goal_card);
            _staticGoalCardFrame =
                    Activity.FindViewById<FrameLayout>(Resource.Id.static_goal_card_frame);
            GenerateNewIdsForViews();

            /* Set up layout manager */
            _layoutManager = new LockableLinearLayoutManager(Activity, LinearLayoutManager.Horizontal, false);
            _recyclerView.SetLayoutManager(_layoutManager);

            /* Set up scroll listener for recycler view */
            _scrollListener = new XamarinRecyclerViewOnScrollListener(
                _layoutManager,
                _tapeCards.Count - 1
            );
            _scrollListener.LastItemIsVisible += OnLastItemIsVisible;
            _scrollListener.LastItemIsHidden += OnLastItemIsHidden;
            _recyclerView.AddOnScrollListener(_scrollListener);

            /* Set widgets data */
            _userName.Text = user.FirstName;
            _userImage.SetImageURI(Android.Net.Uri.Parse(user.PhotoPath));
            // TODO: Change to normal load - _staticGoalCard.SetImageURI(Android.Net.Uri.Parse(_tapeCards.Last().PhotoPath));
            var imageSize = ImageHelper.ConvertDpToPixels(
                140,
                InteractiveTimetable.Current.ScreenDensity
            );
            var bitmap = _tapeCards.Last().PhotoPath.LoadAndResizeBitmap(imageSize, imageSize);
            _staticGoalCard.SetImageBitmap(bitmap);

            /* Set up the adapter */
            _tapeItemListAdapter = new TimetableTapeListAdapter(Activity, _tapeCards);
            _tapeItemListAdapter.ItemClick += OnItemClick;
            _tapeItemListAdapter.ItemLongClick += OnItemLongClick;
            _recyclerView.SetAdapter(_tapeItemListAdapter);

            /* Set handlers */
            _editTapeButton.Click += OnEditTimetableTapeButtonClicked;

            // TODO: Show message if no cards yet
        }

        private void OnLastItemIsHidden()
        {
            _staticGoalCardFrame.Visibility = ViewStates.Visible;
        }

        private void OnLastItemIsVisible()
        {
            _staticGoalCardFrame.Visibility = ViewStates.Gone;
        }

        private void OnItemLongClick(View clickedView, int tapeCardId, int positionInList)
        {
            if (_isLocked)
            {
                OnLockedClicked();
                return;
            }
            Console.WriteLine("Item long clicked");
        }

        private void OnItemClick(View clickedView, int tapeCardId, int positionInList)
        {
            if (_isLocked)
            {
                OnLockedClicked();
                return;
            }
            Console.WriteLine("Item clicked");
        }

        private void OnEditTimetableTapeButtonClicked(object sender, EventArgs e)
        {
            EditTimetableTapeButtonClicked?.Invoke(_userId);
        }

        public override View OnCreateView(
            LayoutInflater inflater,
            ViewGroup container, 
            Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.timetable_tape, container, false);
        }

        public override void OnDestroy()
        {
            _tapeItemListAdapter.ItemClick -= OnItemClick;
            _tapeItemListAdapter.ItemLongClick -= OnItemLongClick;
            _editTapeButton.Click -= OnEditTimetableTapeButtonClicked;
            _recyclerView.ClearOnScrollListeners();
            GC.Collect();

            base.OnDestroy();
        }

        private void OnLockedClicked()
        {
            ClickedWhenLocked?.Invoke(this, null);
        }
        #endregion

        #region Other Methods
        public void DataSetChanged()
        {
            // TODO: Implement when need to change schedule after timetable created or changed
        }

        public void GenerateNewIdsForViews()
        {
            int newId = View.GenerateViewId();

            /* New id for _recyclerView */
            _recyclerView.Id = newId;
            _recyclerView = Activity.FindViewById<RecyclerView>(newId);

            /* New id for _editTapeButton */
            newId = View.GenerateViewId();
            _editTapeButton.Id = newId;
            _editTapeButton = Activity.FindViewById<ImageButton>(newId);

            /* New id for _userName */
            newId = View.GenerateViewId();
            _userName.Id = newId;
            _userName = Activity.FindViewById<TextView>(newId);

            /* New id for _userImage */
            newId = View.GenerateViewId();
            _userImage.Id = newId;
            _userImage = Activity.FindViewById<ImageView>(newId);

            /* New id for _staticGoalCard */
            newId = View.GenerateViewId();
            _staticGoalCard.Id = newId;
            _staticGoalCard = Activity.FindViewById<ImageView>(newId);

            /* New id for _staticGoalCardFrame */
            newId = View.GenerateViewId();
            _staticGoalCardFrame.Id = newId;
            _staticGoalCardFrame = Activity.FindViewById<FrameLayout>(newId);
        }

        public void LockFragment()
        {
            _isLocked = true;
            _layoutManager.IsScrollEnabled = false;
        }

        public void UnlockFragment()
        {
            _isLocked = false;
            _layoutManager.IsScrollEnabled = true;
        }
        #endregion

        #endregion
    }
}