using System;
using System.Threading;
using Microsoft.Xna.Framework;

namespace com.xexuxjy.magiccarpet.threading
{
    public class DoubleBuffer
    {
        private ChangeBuffer[] buffers;
        private volatile int currentUpdateBuffer;
        private volatile int currentRenderBuffer;
        private volatile GameTime gameTime;

        private AutoResetEvent renderFrameStart;
        private AutoResetEvent renderFrameEnd;
        private AutoResetEvent updateFrameStart;
        private AutoResetEvent updateFrameEnd;

        public int ChangeMessageCount { get; set; }

        public DoubleBuffer()
        {
            //create the buffers
            buffers = new ChangeBuffer[2];
            buffers[0] = new ChangeBuffer();
            buffers[1] = new ChangeBuffer();

            //create the WaitHandlers
            renderFrameStart = new AutoResetEvent(false);
            renderFrameEnd = new AutoResetEvent(false);
            updateFrameStart = new AutoResetEvent(false);
            updateFrameEnd = new AutoResetEvent(false);

            //reset the values
            Reset();
        }

        public void Reset()
        {
            //reset the buffer indexes
            currentUpdateBuffer = 0;
            currentRenderBuffer = 1;

            //set all events to non-signaled
            renderFrameStart.Reset();
            renderFrameEnd.Reset();
            updateFrameStart.Reset();
            updateFrameEnd.Reset();
        }

        public void CleanUp()
        {
            //relese system resources
            renderFrameStart.Close();
            renderFrameEnd.Close();
            updateFrameStart.Close();
            updateFrameEnd.Close();
        }


        public void StartUpdateProcessing(out ChangeBuffer updateBuffer, out GameTime gameTime)
        {
            //wait for start signal
            updateFrameStart.WaitOne();
            Thread.MemoryBarrier();
            //get the update buffer
            updateBuffer = buffers[currentUpdateBuffer];
            //get the game time
            gameTime = this.gameTime;
        }

        public void StartRenderProcessing(out ChangeBuffer renderBuffer, out GameTime gameTime)
        {
            //wait for start signal
            renderFrameStart.WaitOne();
            Thread.MemoryBarrier();
            //get the render buffer
            renderBuffer = buffers[currentRenderBuffer];
            //ret the game time
            gameTime = this.gameTime;
        }

        public void StartPhysicsProcessing(out ChangeBuffer renderBuffer, out GameTime gameTime)
        {
            //wait for start signal
            renderFrameStart.WaitOne();
            Thread.MemoryBarrier();
            //get the render buffer
            renderBuffer = buffers[currentRenderBuffer];
            //ret the game time
            gameTime = this.gameTime;
        }



        public void SubmitUpdate()
        {
            Thread.MemoryBarrier();
            //update is done
            updateFrameEnd.Set();
        }
        public void SubmitRender()
        {
            Thread.MemoryBarrier();
            //render is done
            renderFrameEnd.Set();
        }

        private void SwapBuffers()
        {
            currentRenderBuffer = currentUpdateBuffer;
            currentUpdateBuffer = (currentUpdateBuffer + 1) % 2;
            ChangeMessageCount = buffers[currentRenderBuffer].Messages.Count;
        }

        public void GlobalStartFrame(GameTime gameTime)
        {
            this.gameTime = gameTime;
            SwapBuffers();

            //signal the render and update threads to start processing
            renderFrameStart.Set();
            updateFrameStart.Set();
        }

        public void GlobalSynchronize()
        {
            renderFrameEnd.WaitOne();
            updateFrameEnd.WaitOne();
        }

    }
}
