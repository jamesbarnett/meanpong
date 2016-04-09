using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MeanPong
{
    public class DrawLogger
    {
        private GameTime _lastUpdate;
        private float _interval;

        public DrawLogger() {}

        public void Initialize(GameTime gameTime, float interval)
        {
            _lastUpdate = gameTime;
            _interval = interval;
        }

        public void Log(GameTime gameTime, string msg)
        {
            if (IsUpdateDue(gameTime)) Debug.WriteLine(msg);
        }

        public void Log(GameTime gameTime, string fmt, params object[] args)
        {
            if (IsUpdateDue(gameTime)) Debug.WriteLine(string.Format(fmt, args));
        }

        private bool IsUpdateDue(GameTime gameTime)
        {
            bool val = _lastUpdate.ElapsedGameTime.TotalMilliseconds + gameTime.ElapsedGameTime.TotalMilliseconds < _interval;
            if (val)
            {
                _lastUpdate = gameTime;
            }

            return val;
        }
    }
}
