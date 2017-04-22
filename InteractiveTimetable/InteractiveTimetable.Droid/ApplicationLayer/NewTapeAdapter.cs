using System;
using System.Collections.Generic;
using Android.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using InteractiveTimetable.BusinessLayer.Models;

namespace InteractiveTimetable.Droid.ApplicationLayer
{
    class NewTapeAdapter : RecyclerView.Adapter
    {
        #region Events
        public event Action<int> ItemClick;
        #endregion

        #region Properties
        public NewTapeItemViewHolder CurrentCard { get; set; }
        public IList<Card> TapeItems { get; set; }
        public override int ItemCount => TapeItems.Count;
        #endregion

        #region Internal Variables
        private Activity _context;
        private int _currentCardPosition;
        #endregion

        #region Methods

        #region Construct Methods
        public NewTapeAdapter(Activity context, IList<Card> tapeItems)
        {
            _context = context;
            if (tapeItems.Count == 0)
            {
                var card = new Card()
                {
                    Id = 0
                };

                TapeItems = new List<Card>()
                {
                    card, card, card, card, card, card, card, card, card, card,
                };
            }
            else
            {
                TapeItems = tapeItems;
            }
            _currentCardPosition = 0;
        }
        #endregion

        #region Event Handlers
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var viewHolder = holder as NewTapeItemViewHolder;

            if (viewHolder != null)
            {
                var itemAtPosition = TapeItems[position];

                if (itemAtPosition.Id <= 0)
                {
                    /* Empty item */
                    viewHolder.ItemImage.SetImageResource(Resource.Drawable.empty_new_tape_item);
                    viewHolder.ItemFrame.SetBackgroundResource(0);
                }
                else
                {
                    /* Set card image */
                    var imageSize = ImageHelper.ConvertDpToPixels(
                        140,
                        InteractiveTimetable.Current.ScreenDensity
                    );
                    var bitmap = itemAtPosition.PhotoPath.LoadAndResizeBitmap(imageSize, imageSize);
                    viewHolder.ItemImage.SetImageBitmap(bitmap);
                }

                /* Set common properties */
                viewHolder.TapeItemId = itemAtPosition.Id;
                viewHolder.PositionInList = position;

                /* Set current card */
                if (_currentCardPosition == position)
                {
                    SetCurrentCard(viewHolder);
                }
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var inflater = LayoutInflater.From(_context);
            var view = inflater.Inflate(Resource.Layout.new_tape_item, parent, false);

            var holder = new NewTapeItemViewHolder(view, OnItemClick, OnDeleteButtonClicked);
            return holder;
        }

        private void OnDeleteButtonClicked(int positionInList)
        {
            /* Delete from data set */
            TapeItems.RemoveAt(positionInList);

            /* Notify adapter */
            NotifyItemRemoved(positionInList);
            NotifyItemRangeChanged(positionInList, TapeItems.Count);
        }

        private void OnItemClick(NewTapeItemViewHolder viewHolder)
        {
            SetCurrentCard(viewHolder);
        }
        #endregion

        #region Other Methods
        public void SetCurrentCard(NewTapeItemViewHolder viewHolder)
        {
            /* Put off green frame from old CurrentCard */
            if (CurrentCard != null)
            {
                CurrentCard.ItemFrame.SetBackgroundResource(0);
            }

            /* New CurrentCard */
            CurrentCard = viewHolder;
            _currentCardPosition = CurrentCard.PositionInList;

            /* Green frame with paddings */
            CurrentCard.ItemFrame.SetBackgroundResource(Resource.Drawable.green_round_corner_frame);
            var paddingInDp = 5;
            var paddingInPx = ImageHelper.ConvertDpToPixels(
                paddingInDp,
                InteractiveTimetable.Current.ScreenDensity
            );
            CurrentCard.ItemFrame.SetPadding(paddingInPx, paddingInPx, paddingInPx, paddingInPx);
        }

        public void SetActivityCard(int cardId, ImageView cardImage)
        {
            /* Change card in data set */
            var card = InteractiveTimetable.Current.ScheduleManager.Cards.GetCard(cardId);
            TapeItems[CurrentCard.PositionInList] = card;
            
            /* Set Current Card */
            CurrentCard.ItemImage.SetImageDrawable(cardImage.Drawable);
            CurrentCard.TapeItemId = cardId;
        }

        public void AddNewTapeItem()
        {
            /* Add card to data set */
            var card = new Card()
            {
                Id = 0
            };
            TapeItems.Add(card);

            /* Notify adapter */
            NotifyItemInserted(TapeItems.Count);
        }
        #endregion

        #endregion
    }
}