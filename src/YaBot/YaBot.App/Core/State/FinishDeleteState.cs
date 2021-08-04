namespace YaBot.App.Core.State
{
    using System;
    using Extensions;

    public sealed class FinishDeleteState : IState
    {
        private readonly Action<int> deletePlace;
        private readonly IWords error;
        private readonly IWords success;

        public FinishDeleteState(IWords error, IWords success, Action<int> deletePlace)
        {
            this.deletePlace = deletePlace;
            this.error = error;
            this.success = success;
        }

        public bool IsInput(Input input)
        {
            return true;
        }

        public (Output, IState) Process(Input input)
        {
            //TODO : validation

            if (int.TryParse(input.Message.Text, out var index).Not())
                return (error.ToError("Неправильный формат. Введите только индекс").ToOutput(), this);

            deletePlace(index);

            return (success.ToRandom().ToOutput(), null);
        }

        public IState Reset()
        {
            return this;
        }
    }
}