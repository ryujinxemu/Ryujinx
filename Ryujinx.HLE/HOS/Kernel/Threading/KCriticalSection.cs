using System.Threading;

namespace Ryujinx.HLE.HOS.Kernel.Threading
{
    class KCriticalSection
    {
        private readonly Horizon _system;

        public object LockObj { get; private set; }

        private int _recursionCount;

        public KCriticalSection(Horizon system)
        {
            _system = system;

            LockObj = new object();
        }

        public void Enter()
        {
            Monitor.Enter(LockObj);

            _recursionCount++;
        }

        public void Leave()
        {
            if (_recursionCount == 0)
            {
                return;
            }

            bool doContextSwitch = false;

            if (--_recursionCount == 0)
            {
                if (_system.Scheduler.ThreadReselectionRequested)
                {
                    _system.Scheduler.SelectThreads();
                }

                Monitor.Exit(LockObj);

                if (_system.Scheduler.MultiCoreScheduling)
                {
                    lock (_system.Scheduler.CoreContexts)
                    {
                        foreach (var coreContext in _system.Scheduler.CoreContexts)
                        {
                            if (!coreContext.ContextSwitchNeeded)
                            {
                                continue;
                            }

                            KThread currentThread = coreContext.CurrentThread;

                            if (currentThread == null)
                            {
                                // Nothing is running, we can perform the context switch immediately.
                                coreContext.ContextSwitch();
                            }
                            else if (currentThread.IsCurrentHostThread())
                            {
                                // Thread running on the current core, context switch will block.
                                doContextSwitch = true;
                            }
                            else
                            {
                                // Thread running on another core, request a interrupt.
                                currentThread.Context.RequestInterrupt();
                            }
                        }
                    }
                }
                else
                {
                    doContextSwitch = true;
                }
            }
            else
            {
                Monitor.Exit(LockObj);
            }

            if (doContextSwitch)
            {
                _system.Scheduler.ContextSwitch();
            }
        }
    }
}