using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;

namespace SgupsPlanner.Core.Extensions
{
    public class AsyncObservableCollection<T> : ObservableCollection<T>
    {
        private readonly SynchronizationContext _synchronizationContext = SynchronizationContext.Current;

        public AsyncObservableCollection()
        {
        }

        public AsyncObservableCollection(IEnumerable<T> list)
            : base(list)
        {
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (SynchronizationContext.Current == _synchronizationContext)
            {
                // Вызов уведомления в текущем потоке
                RaiseCollectionChanged(e);
            }
            else
            {
                // Вызов уведомления в потоке создателя
                _synchronizationContext.Send(RaiseCollectionChanged, e);
            }
        }

        private void RaiseCollectionChanged(object param)
        {
            // Мы находимся в потоке создателей, вызываем базовую реализацию напрямую
            base.OnCollectionChanged((NotifyCollectionChangedEventArgs)param);
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (SynchronizationContext.Current == _synchronizationContext)
            {
                // Вызов уведомления в текущем потоке
                RaisePropertyChanged(e);
            }
            else
            {
                // Вызов уведомления в потоке создателя
                _synchronizationContext.Send(RaisePropertyChanged, e);
            }
        }

        private void RaisePropertyChanged(object param)
        {
            // В потоке создателя вызываем базовую реализацию
            base.OnPropertyChanged((PropertyChangedEventArgs)param);
        }
    }
}
