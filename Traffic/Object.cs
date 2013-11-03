using System.Collections.Generic;

namespace Traffic
{
    class Object
    {
        public List <Object> Components { get; private set; }

        //------------------------------------------------------------------
        public Object ()
        {
            Components = new List <Object> ();
        }

        //------------------------------------------------------------------
        public virtual void Setup ()
        {
            Components.ForEach (item => item.Setup ());
        }

        //------------------------------------------------------------------
        public virtual void Update ()
        {
            Components.ForEach (item => item.Update ());
        }

        //------------------------------------------------------------------
        public virtual void Draw ()
        {
            Components.ForEach (item => item.Draw ());
        }

        //-----------------------------------------------------------------
        private void Add (Object item)
        {
            Components.Add (item);
        }

        //-----------------------------------------------------------------
        private void Remove (Object item)
        {
            Components.Remove (item);
        }
    }
}