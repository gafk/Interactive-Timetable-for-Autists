using System;
using Android.App;
using Android.OS;
using Android.Text;
using Android.Views;
using Android.Widget;
using InteractiveTimetable.BusinessLayer.Models;

namespace InteractiveTimetable.Droid.UserInterfaceLayer
{
    public class TripDetailsEditFragment : Fragment
    {
        #region Constants
        public static readonly string FragmentTag = "trip_details_edit_fragment";
        private static readonly string HospitalTripIdKey = "current_trip_id";
        private static readonly string DateFormat = "dd.MM.yyyy";
        private static readonly string TimeFormat = "H:mm";
        #endregion

        #region Widgets
        private Button _applyButton;
        private Button _cancelButton;
        private EditText _startDate;
        private EditText _startTime;
        private EditText _finishDate;
        private EditText _finishTime;
        private TextView _fragmentLabel;
        #endregion

        #region Events
        public event Action<int> NewTripAdded;
        public event Action<int> TripEdited;
        #endregion

        #region Internal Variables
        private HospitalTrip _trip;
        private DateTime _currentStartDate;
        private DateTime _currentFinishDate;
        #endregion

        #region Flags
        private bool _dataWasChanged;
        private bool _newTrip;
        #endregion

        #region Properties
        public int TripId
        {
            get { return Arguments.GetInt(HospitalTripIdKey, 0); }
        }
        #endregion

        #region Methods

        #region Construct Methods
        public static TripDetailsEditFragment NewInstance(int tripId)
        {
            var tripDetailsEditFragment = new TripDetailsEditFragment()
            {
                Arguments = new Bundle()
            };
            tripDetailsEditFragment.Arguments.PutInt(HospitalTripIdKey, tripId);

            return tripDetailsEditFragment;
        }
        #endregion

        #region Event Handlers
        public override View OnCreateView(
            LayoutInflater inflater, 
            ViewGroup container, 
            Bundle savedInstanceState)
        {
            if (container == null)
            {
                return null;
            }

            var tripView = inflater.Inflate(Resource.Layout.trip_details_edit, container, false);

            /* Get widgets */
            _applyButton = tripView.FindViewById<Button>(Resource.Id.apply_trip_edit_button);
            _cancelButton = tripView.FindViewById<Button>(Resource.Id.cancel_trip_edit_button);
            _startDate = tripView.FindViewById<EditText>(Resource.Id.start_date);
            _startTime = tripView.FindViewById<EditText>(Resource.Id.start_time);
            _finishDate = tripView.FindViewById<EditText>(Resource.Id.finish_date);
            _finishTime = tripView.FindViewById<EditText>(Resource.Id.finish_time);
            _fragmentLabel = tripView.FindViewById<TextView>(Resource.Id.trip_edit_label);

            /* Set button click handlers */
            _applyButton.Click += OnApplyButtonClicked;
            _cancelButton.Click += OnCancelButtonClicked;
            _startDate.Click += OnStartDateClicked;
            _startTime.Click += OnStartTimeClicked;
            _finishDate.Click += OnFinishDateClicked;
            _finishTime.Click += OnFinishTimeClicked;

            /* If trip is set, retrieve it data */
            if (TripId > 0)
            {
                /* Get data */
                _trip = InteractiveTimetable.Current.HospitalTripManager.GetHospitalTrip(TripId);

                /* Set data to fields */
                _startDate.Text = _trip.StartDate.ToString(DateFormat);
                _startTime.Text = _trip.StartDate.ToString(TimeFormat);
                _finishDate.Text = _trip.FinishDate.ToString(DateFormat);
                _finishTime.Text = _trip.FinishDate.ToString(TimeFormat);

                /* Adjust widgets */
                _fragmentLabel.Text = GetString(Resource.String.edit_trip);
                _applyButton.Text = GetString(Resource.String.edit_button);

                /* Set current date and time */
                _currentStartDate = _trip.StartDate;
                _currentFinishDate = _trip.FinishDate;
            }
            else
            {
                /* Set current date and time */
                _currentStartDate = DateTime.Now;
                _currentFinishDate = DateTime.Now.AddDays(7);


                /* Adjust widgets */
                _fragmentLabel.Text = GetString(Resource.String.add_trip);
                _applyButton.Text = GetString(Resource.String.add_button);
            }

            /* Set listeners to track data changing */
            _startDate.TextChanged += OnTripDataChanged;
            _startTime.TextChanged += OnTripDataChanged;
            _finishDate.TextChanged += OnTripDataChanged;
            _finishTime.TextChanged += OnTripDataChanged;

            /* Set flags */
            _dataWasChanged = false;

            return tripView;
        }

        public override void OnDestroy()
        {
            /* Reset listeners */
            _applyButton.Click -= OnApplyButtonClicked;
            _cancelButton.Click -= OnCancelButtonClicked;
            _startDate.Click -= OnStartDateClicked;
            _startTime.Click -= OnStartTimeClicked;
            _finishDate.Click -= OnFinishDateClicked;
            _finishTime.Click -= OnFinishTimeClicked;

            base.OnDestroy();
        }

        private void OnTripDataChanged(object sender, TextChangedEventArgs e)
        {
            _dataWasChanged = true;
        }

        private void OnFinishTimeClicked(object sender, EventArgs e)
        {
            var fragment = TimePickerFragment.NewInstance(
                _currentFinishDate,
                delegate (DateTime time)
                {
                    var timeSpan = new TimeSpan(time.Hour, time.Minute, time.Second);
                    _currentFinishDate = _currentFinishDate.Date + timeSpan;
                    _finishTime.Text = _currentFinishDate.ToString(TimeFormat);
                });

            fragment.Show(FragmentManager, TimePickerFragment.FragmentTag);
        }

        private void OnFinishDateClicked(object sender, EventArgs e)
        {
            var fragment = DatePickerFragment.NewInstance(
                _currentFinishDate,
                delegate (DateTime date)
                {
                    var time = new TimeSpan(
                        _currentFinishDate.Hour,
                        _currentFinishDate.Minute,
                        _currentFinishDate.Second
                    );
                    _currentFinishDate = date.Date + time;
                    _finishDate.Text = date.ToString(DateFormat);
                });

            fragment.Show(FragmentManager, DatePickerFragment.FragmentTag);
        }

        private void OnStartTimeClicked(object sender, EventArgs e)
        {
            var fragment = TimePickerFragment.NewInstance(
                _currentStartDate,
                delegate(DateTime time)
                {
                    var timeSpan = new TimeSpan(time.Hour, time.Minute, time.Second);
                    _currentStartDate = _currentStartDate.Date + timeSpan;
                    _startTime.Text = _currentStartDate.ToString(TimeFormat);
                });

            fragment.Show(FragmentManager, TimePickerFragment.FragmentTag);
        }

        private void OnStartDateClicked(object sender, EventArgs e)
        {
            var fragment = DatePickerFragment.NewInstance(
                _currentStartDate,
                delegate(DateTime date)
                {
                    var time = new TimeSpan(
                        _currentStartDate.Hour,
                        _currentStartDate.Minute,
                        _currentStartDate.Second
                    );
                    _currentStartDate = date.Date + time;
                    _startDate.Text = date.ToString(DateFormat);
                });

            fragment.Show(FragmentManager, DatePickerFragment.FragmentTag);
        }

        private void OnCancelButtonClicked(object sender, EventArgs e)
        {
            CloseFragment();
        }

        private void OnApplyButtonClicked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Other Methods
        private void CloseFragment()
        {
            Activity.FragmentManager.PopBackStackImmediate();
            if (Activity.CurrentFocus != null)
            {
                InteractiveTimetable.Current.HideKeyboard(Activity.CurrentFocus.WindowToken);
            }
        }
        #endregion

        #endregion

    }
}