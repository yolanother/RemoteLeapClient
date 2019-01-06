using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace DoubTech.Util {
    public abstract class BackgroundableMonoBehavior : MonoBehaviour {

        private List<ThreadStart> mainThreadQueue = new List<ThreadStart>();

        public void RunOnMain(ThreadStart d) {
            lock (mainThreadQueue) {
                mainThreadQueue.Add(d);
            }
        }

        public Thread RunOnBackground(ThreadStart d, string threadName = "") {
            Thread thread = new Thread(() => {
                try {
                    d();
                } catch (Exception e) {
                    RunOnMain(() => {
                        OnExceptionThrown(e, threadName);
                    });
                }
            });
            thread.Start();
            return thread;
        }

        virtual protected void Update() {
            List<ThreadStart> pending = new List<ThreadStart>();
            lock (mainThreadQueue) {
                pending.AddRange(mainThreadQueue);
                mainThreadQueue.Clear();
            }
            foreach (ThreadStart d in pending) {
                d();
            }
        }

        protected virtual void OnExceptionThrown(Exception e, string threadName) {
            throw e;
        }
    }
}