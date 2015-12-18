/*
 * Name: Steven Johnston
 * File: MyTimer.cs
 * Assignment: Mystifiy #04
 * Date: 10/23/2015
 * Description: A generic timer class. Uses a delay(milliseconds) fuction(delegate) and object(parameters), to replicate a timer.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Conestoga_Frenzy
{
    class MyTimer
    {
        /// <summary>
        /// delegete to define how the function is called
        /// </summary>
        /// <param name="e">The e.</param>
        public delegate void func(object e);
        /// <summary>
        /// The delay amount
        /// </summary>
        private int delayAmount;

        /// <summary>
        /// Gets or sets the delay amount.
        /// </summary>
        /// <value>
        /// The delay amount in milliseconds.
        /// </value>
        public int DelayAmount
        {
            get { return delayAmount; }
            set { delayAmount = value; }
        }
        /// <summary>
        /// The thread to run the fuction on
        /// </summary>
        public Thread thread;
        /// <summary>
        /// fuction to run
        /// </summary>
        public func myFunc;
        /// <summary>
        /// parmaeteres to be using in function call
        /// </summary>
        public object param;
        /// <summary>
        /// bool to indicate if the timer should/is paused
        /// </summary>
        public bool paused = false;
        /// <summary>
        /// bool to indicate that the timer should end
        /// </summary>
        bool killThread = false;
        /// <summary>
        /// Used to pause and resume timer
        /// </summary>
        ManualResetEvent mrse = new ManualResetEvent(false);

        /// <summary>
        /// Initializes a new instance of the <see cref="MyTimer"/> class.
        /// </summary>
        /// <param name="delay">The delay.</param>
        /// <param name="newFunc">The new function.</param>
        /// <param name="param">The parameters.</param>
        public MyTimer(int delay, func newFunc, object param)
        {
            delayAmount = delay;
            myFunc = newFunc;
            this.param = param;
            //start timer now
            start();
        }
        /// <summary>
        /// Finalizes an instance of the <see cref="MyTimer"/> class. to set delegate to null
        /// </summary>
        ~MyTimer()
        {
            myFunc = null;
        }
        /// <summary>
        /// Delays this instance the amount that is specified
        /// </summary>
        public void delay()
        {
            Thread.Sleep(delayAmount);
        }
        /// <summary>
        /// Starts this instance. Sets the loop function to a thread;
        /// </summary>
        public void start()
        {
            thread = new Thread(() => loop());
            thread.Start();
        }
        /// <summary>
        /// Loops this instance. calling the delegate function the the delay function. untill paused/killed
        /// </summary>
        public void loop()
        {
            for (; !killThread;)
            {
                if (paused)
                {
                    mrse.WaitOne();
                }
                //call delegate function
                myFunc(param);
                //delay this thread
                delay();
            }
        }
        /// <summary>
        /// Pauses this instance.
        /// </summary>
        public void pause()
        {
            paused = true;
            mrse.Reset();
        }
        /// <summary>
        /// Resumes this instance.
        /// </summary>
        public void resume()
        {
            paused = false;
            mrse.Set();
        }
        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void stop()
        {
            killThread = true;
        }
    }
}