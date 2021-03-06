﻿using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;

namespace Utilities.Reactive
{
    /// <summary>
    /// Represents ReactiveCommand&lt;object&gt;
    /// </summary>
    public class ReactiveCommand : ReactiveCommand<object>
    {
        /// <summary>
        /// CanExecute is always true. When disposed CanExecute change false called on UIDispatcherScheduler.
        /// </summary>
        public ReactiveCommand()
        {
        }

        /// <summary>
        /// CanExecute is always true. When disposed CanExecute change false called on scheduler.
        /// </summary>
        public ReactiveCommand(IScheduler scheduler)
            : base(scheduler)
        {
        }

        /// <summary>
        /// CanExecuteChanged is called from canExecute sequence on UIDispatcherScheduler.
        /// </summary>
        public ReactiveCommand(IObservable<bool> canExecuteSource, bool initialValue = true)
            : base(canExecuteSource, initialValue)
        {
        }

        /// <summary>
        /// CanExecuteChanged is called from canExecute sequence on scheduler.
        /// </summary>
        public ReactiveCommand(IObservable<bool> canExecuteSource, IScheduler scheduler, bool initialValue = true)
            : base(canExecuteSource, scheduler, initialValue)
        {
        }

        /// <summary>Push null to subscribers.</summary>
        public void Execute()
        {
            Execute(null);
        }
    }

    public class ReactiveCommand<T> : IObservable<T>, ICommand, IDisposable
    {
        private readonly IDisposable _canExecuteSubscription;
        private readonly IScheduler _scheduler;
        private readonly Subject<T> _trigger = new Subject<T>();
        private bool _isCanExecute;
        private bool _isDisposed;

        /// <summary>
        /// CanExecute is always true. When disposed CanExecute change false called on UIDispatcherScheduler.
        /// </summary>
        public ReactiveCommand()
            : this(Observable.Never<bool>())
        {
        }

        /// <summary>
        /// CanExecute is always true. When disposed CanExecute change false called on scheduler.
        /// </summary>
        public ReactiveCommand(IScheduler scheduler)
            : this(Observable.Never<bool>(), scheduler)
        {
        }

        /// <summary>
        /// CanExecuteChanged is called from canExecute sequence on UIDispatcherScheduler.
        /// </summary>
        public ReactiveCommand(IObservable<bool> canExecuteSource, bool initialValue = true)
            : this(canExecuteSource, UIDispatcherScheduler.Default, initialValue)
        {
        }

        /// <summary>
        /// CanExecuteChanged is called from canExecute sequence on scheduler.
        /// </summary>
        public ReactiveCommand(IObservable<bool> canExecuteSource, IScheduler scheduler, bool initialValue = true)
        {
            _isCanExecute = initialValue;
            _scheduler = scheduler;
            _canExecuteSubscription = canExecuteSource
                .DistinctUntilChanged()
                .ObserveOn(scheduler)
                .Subscribe(canExecute =>
                {
                    _isCanExecute = canExecute;
                    var handler = CanExecuteChanged;
                    if (handler != null)
                    {
                        handler(this, EventArgs.Empty);
                    }
                });
        }

        public event EventHandler CanExecuteChanged;

        /// <summary>Return current canExecute status. parameter is ignored.</summary>
        bool ICommand.CanExecute(object parameter)
        {
            return _isCanExecute;
        }

        /// <summary>Push parameter to subscribers.</summary>
        void ICommand.Execute(object parameter)
        {
            if (_isCanExecute)
            {
                _trigger.OnNext((T) parameter);
            }
        }

        /// <summary>
        /// Stop all subscription and lock CanExecute is false.
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed) return;

            _isDisposed = true;
            _trigger.OnCompleted();
            _trigger.Dispose();
            _canExecuteSubscription.Dispose();

            if (_isCanExecute)
            {
                _isCanExecute = false;
                _scheduler.Schedule(() =>
                {
                    _isCanExecute = false;
                    var handler = CanExecuteChanged;
                    if (handler != null) handler(this, EventArgs.Empty);
                });
            }
        }

        /// <summary>Subscribe execute.</summary>
        public IDisposable Subscribe(IObserver<T> observer)
        {
            return _trigger.Subscribe(observer);
        }

        /// <summary>Return current canExecute status.</summary>
        public bool CanExecute()
        {
            return _isCanExecute;
        }

        /// <summary>Push parameter to subscribers.</summary>
        public void Execute(T parameter)
        {
            _trigger.OnNext(parameter);
        }
    }

    public static class ReactiveCommandExtensions
    {
        /// <summary>
        /// CanExecuteChanged is called from canExecute sequence on UIDispatcherScheduler.
        /// </summary>
        public static ReactiveCommand ToReactiveCommand(this IObservable<bool> canExecuteSource,
            bool initialValue = true)
        {
            return new ReactiveCommand(canExecuteSource, initialValue);
        }

        /// <summary>
        /// CanExecuteChanged is called from canExecute sequence on scheduler.
        /// </summary>
        public static ReactiveCommand ToReactiveCommand(this IObservable<bool> canExecuteSource, IScheduler scheduler,
            bool initialValue = true)
        {
            return new ReactiveCommand(canExecuteSource, scheduler, initialValue);
        }

        /// <summary>
        /// CanExecuteChanged is called from canExecute sequence on UIDispatcherScheduler.
        /// </summary>
        public static ReactiveCommand<T> ToReactiveCommand<T>(this IObservable<bool> canExecuteSource,
            bool initialValue = true)
        {
            return new ReactiveCommand<T>(canExecuteSource, initialValue);
        }

        /// <summary>
        /// CanExecuteChanged is called from canExecute sequence on scheduler.
        /// </summary>
        public static ReactiveCommand<T> ToReactiveCommand<T>(this IObservable<bool> canExecuteSource,
            IScheduler scheduler, bool initialValue = true)
        {
            return new ReactiveCommand<T>(canExecuteSource, scheduler, initialValue);
        }
    }
}