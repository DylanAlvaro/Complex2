using UnityEngine;

namespace Math
{
    public class Line
    {
        private Orientation _orientation;
        private Vector2Int coordinates;

        public Line(Orientation orientation, Vector2Int coordinates)
        {
            this._orientation = orientation;
            this.coordinates = coordinates;
        }
        
        public Orientation Orientation
        {
            get => _orientation;
            set => _orientation = value;
        }
        
        public Vector2Int Coordinates
        {
            get => coordinates;
            set => coordinates = value;
        }
    }

    public enum Orientation
    {
        Horizontal = 0,
        Vertical = 1
    }
}