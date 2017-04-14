using System;
using System.Collections.Generic;
using Android.App;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using InteractiveTimetable.BusinessLayer.Models;

namespace InteractiveTimetable.Droid.ApplicationLayer
{
    class TimetableTapeListAdapter : RecyclerView.Adapter
    {
        #region Events
        public event Action<View, int, int> ItemClick;
        public event Action<View, int, int> ItemLongClick;
        #endregion

        #region Properties
        public IList<Card> TapeItems { get; set; }
        public override int ItemCount => TapeItems.Count;
        #endregion

        #region Internal Variables
        private Activity _context;
        #endregion

        #region Methods

        #region Construct Methods
        public TimetableTapeListAdapter(Activity context, IList<Card> tapeItems)
        {
            _context = context;
            TapeItems = tapeItems;
        }
        #endregion

        #region Event Handlers
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var tapeItemAtPosition = TapeItems[position];
            var viewHolder = holder as TimetableTapeItemViewHolder;

            if (viewHolder != null)
            {
                // TODO: Change to normal load - viewHolder.ItemImage.SetImageURI(Android.Net.Uri.Parse(tapeItemAtPosition.PhotoPath));
                var imageSize = ImageHelper.ConvertDpToPixels(
                    140,
                    InteractiveTimetable.Current.ScreenDensity
                );
                var bitmap = tapeItemAtPosition.PhotoPath.LoadAndResizeBitmap(imageSize, imageSize);
                viewHolder.ItemImage.SetImageBitmap(bitmap);

                /* Set frame for motivation goal card */
                bool isGoalCard =
                        InteractiveTimetable.Current.ScheduleManager.Cards.CardTypes.
                                             IsMotivationGoalCardType(tapeItemAtPosition.CardTypeId);
                if (isGoalCard)
                {
                    AdjustCardFrame(viewHolder, false);
                }
                else
                {
                    AdjustCardFrame(viewHolder, true);
                }

                viewHolder.TapeItemId = tapeItemAtPosition.Id;
                viewHolder.PositionInList = position;
            }
        }

        public override RecyclerView.ViewHolder
            OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var inflater = LayoutInflater.From(_context);
            var view = inflater.Inflate(Resource.Layout.timetable_tape_item, parent, false);

            var holder = new TimetableTapeItemViewHolder(view, OnItemClick, OnItemLongClick);
            return holder;
        }

        private void OnItemLongClick(View clickedView, int tapeItemId, int positionInList)
        {
            ItemLongClick?.Invoke(clickedView, tapeItemId, positionInList);
            // TODO: Cancel activity completion
        }

        private void OnItemClick(View clickedView, int tapeItemId, int positionInList)
        {
            ItemClick?.Invoke(clickedView, tapeItemId, positionInList);
            // TODO: Activity completed - show animation 
        }
        #endregion

        #region Other Methods
        private void AdjustCardFrame(TimetableTapeItemViewHolder viewHolder, bool isHidden)
        {
            if (!isHidden)
            {
                viewHolder.ItemFrame.SetBackgroundResource(Resource.Drawable.red_round_corner_frame);

                int paddingDp = 5;
                int paddingPx = ImageHelper.ConvertDpToPixels(
                    paddingDp,
                    InteractiveTimetable.Current.ScreenDensity
                );
                viewHolder.ItemFrame.SetPadding(
                    paddingPx,
                    paddingPx,
                    paddingPx,
                    paddingPx
                );
            }
            else
            {
                viewHolder.ItemFrame.SetBackgroundColor(Color.Transparent);
                viewHolder.ItemFrame.SetPadding(0, 0, 0, 0);
            }
        }
        #endregion

        #endregion
    }
}