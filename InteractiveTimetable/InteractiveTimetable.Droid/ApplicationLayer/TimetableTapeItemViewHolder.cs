using System;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace InteractiveTimetable.Droid.ApplicationLayer
{
    class TimetableTapeItemViewHolder : RecyclerView.ViewHolder
    {
        #region Properties
        public ImageView ItemImage { get; }
        public FrameLayout ItemFrame { get; }
        public int TapeItemId { get; set; }
        public int PositionInList { get; set; }
        #endregion

        public TimetableTapeItemViewHolder(
            View itemView,
            Action<View, int, int> listenetForClick,
            Action<View, int, int> listenerForLongClick)
            : base(itemView)
        {
            ItemImage = itemView.FindViewById<ImageView>(Resource.Id.tape_item_image);
            ItemFrame = itemView.FindViewById<FrameLayout>(Resource.Id.tape_item_frame);

            itemView.Click += (sender, e) =>
                listenetForClick(itemView, TapeItemId, PositionInList);

            itemView.LongClick += (sender, e) =>
                listenerForLongClick(itemView, TapeItemId, PositionInList);
        }
    }
}