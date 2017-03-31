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
    public class DatePickerFragment : DialogFragment,
                                      DatePickerDialog.IOnDateSetListener
    {
        #region Constants
        public static readonly string FragmentTag = "date_picker_fragment";
        #endregion

        #region Delegates
        private Action<DateTime> _dateSelectedHandler = delegate { };
        #endregion

        #region Internal Variables
        private DateTime _currentDate;
        #endregion

        #region Methods

        #region Construct Methods
        public static DatePickerFragment NewInstance(
            DateTime currentDate,
            Action<DateTime> onDateSelected)
        {
            DatePickerFragment fragment = new DatePickerFragment();
            fragment._dateSelectedHandler = onDateSelected;
            fragment._currentDate = currentDate;

            return fragment;
        }
        #endregion

        #region Event Handlers
        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            DatePickerDialog dialog = new DatePickerDialog(Activity,
                                                           this,
                                                           _currentDate.Year,
                                                           _currentDate.Month - 1,
                                                           _currentDate.Day);
            dialog.SetCanceledOnTouchOutside(true);

            return dialog;
        }

        public void OnDateSet(DatePicker view, int year, int month, int day)
        {
            DateTime selectedDate = new DateTime(year, month + 1, day);
            _dateSelectedHandler(selectedDate);
        }

        public override void OnCancel(IDialogInterface dialog)
        {
            base.OnCancel(dialog);
            _dateSelectedHandler(_currentDate);
        }
        #endregion

        #endregion
    }
}