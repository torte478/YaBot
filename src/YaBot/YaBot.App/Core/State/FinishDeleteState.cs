namespace YaBot.App.Core.State
{
    using System;
    using Extensions;

    public sealed class FinishDeleteState : IState
    {
        private readonly Action<int> deletePlace; // TODO : to Func<int, bool>

        public FinishDeleteState(Action<int> deletePlace)
        {
            this.deletePlace = deletePlace;
        }

        public bool IsInput(Input input)
        {
            return true;
        }

        public (Output, IState) Process(Input input)
        {
            //TODO : validation

            if (int.TryParse(input.Message.Text, out var index).Not())
                return ("Неправильный формат. Введите только индекс".ToOutput(), this);

            deletePlace(index);

            return ("Место удалено. Внимание, индексы мест могли сместиться".ToOutput(), null);
        }

        public IState Reset()
        {
            return this;
        }
    }
}