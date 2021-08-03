namespace YaBot.App.Core.State
{
    using Castle.Core.Internal;
    using Database;
    using Extensions;
    using Telegram.Bot.Types;

    internal sealed class FinishCreatePlaceState : IState
    {
        private readonly Context context; // TODO : to interface

        public FinishCreatePlaceState(Context context)
        {
            this.context = context;
        }

        public bool IsInput(Message message)
        {
            // TODO : is text with image
            return !message.Text.IsNullOrEmpty();
        }

        public (Answer, IState) Process(Message message)
        {
            new Place {Name = message.Text}
                ._(context.Places.Add)
                ._(_ => context.SaveChanges());

            return ("Новое место сохранено".ToAnswer(), null); // (TODO, TODO)
        }

        public IState Reset()
        {
            return this;
        }
    }
}