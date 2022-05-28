using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.Maps;
using Xamarin.Forms;

using Position = Xamarin.Forms.Maps.Position;
using Distance = Xamarin.Forms.Maps.Distance;

namespace VKR.Models
{
    public class Marker
    {
        public int uid;
        public Pin pin;
        public Circle circle;
        //public GeofenceRegion region;

        public Marker()
        {
            uid = -1;
            pin = new Pin();
            circle = new Circle();
            circle.StrokeColor = Color.FromHex("#88FF0000");
            circle.StrokeWidth = 8;
            circle.FillColor = Color.FromHex("#88FFC0CB");
            pin.Type = PinType.Place;
        }

        public Marker(Distance radius, string title, string text) 
        {
            uid = -1;
            pin = new Pin();
            circle = new Circle();
            circle.Radius = radius;
            circle.StrokeColor = Color.FromHex("#88FF0000");
            circle.StrokeWidth = 8;
            circle.FillColor = Color.FromHex("#88FFC0CB");
            pin.Label = title;
            pin.Address = text;
            pin.Type = PinType.Place;
        }

        public void setPosition(Position position) 
        {
            pin.Position = position;
            circle.Center = position;
        }

/*        public void setRegion(GeofenceRegion region)
        {
            this.region = region
        }*/

        [JsonConstructor]
        public Marker(int uid, Pin pin, Circle circle/*, GeofenceRegion region*/) 
        {
            this.uid = uid;
            this.pin = pin;
            this.circle = circle;
            circle.StrokeColor = Color.FromHex("#88FF0000");
            circle.StrokeWidth = 8;
            circle.FillColor = Color.FromHex("#88FFC0CB");
            pin.Type = PinType.Place;
            //this.region = region;
        }
    }
}
